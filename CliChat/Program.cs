using Business;
using Business.Exceptions;
using Business.Models;
using CliChat;
using CliChat.Hubs;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Web.Http;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(System.Net.IPAddress.Parse("0.0.0.0"), 5000, conf =>
        {
            conf.UseHttps();
        });
    });
}

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
    };
});
builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.RegisterDBServices(builder.Configuration);
builder.Services.RegisterBusinessServices(builder.Configuration);

var app = builder.Build();

app.UseCors(x =>
{
    x.SetIsOriginAllowed(_ => true);
    x.AllowAnyMethod();
    x.AllowAnyHeader();
    x.AllowCredentials();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        // using static System.Net.Mime.MediaTypeNames;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        ErrorModel errResp;

        if (exceptionHandlerPathFeature?.Error is ClientSideException err)
        {
            errResp = new ErrorModel()
            {
                Message = err.Message,
                StatusCode = err.StatusCode,
                Suggestions = err.Suggestions
            };

            context.Response.StatusCode = (int)errResp.StatusCode;
        }
        else
        {
            errResp = new ErrorModel()
            {
                Message = "Server side error",
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Suggestions = "Please try again later, or contact with our administration"
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        await context.Response.WriteAsJsonAsync(errResp);
    });
});

app.Run();


// // TODO
//Global 2 bana mncae
//1.    message queueing
//2.    error handling in Api
//et e hedo

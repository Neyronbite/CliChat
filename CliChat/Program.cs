using Business;
using Business.Exceptions;
using Business.Models;
using CliChat;
using CliChat.Filters;
using CliChat.Hubs;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Http;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{

    var certPath = Environment.GetEnvironmentVariable("CERT_PATH");
    certPath = certPath == null ? Environment.GetEnvironmentVariable("CERT_PATH", EnvironmentVariableTarget.Machine) : certPath;
    var certPassword = Environment.GetEnvironmentVariable("CERT_PASSWORD");
    certPassword = certPassword == null ? Environment.GetEnvironmentVariable("CERT_PASSWORD", EnvironmentVariableTarget.Machine) : certPassword;

    if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword))
    {
        var cert = new X509Certificate2(certPath, certPassword);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(System.Net.IPAddress.Parse("0.0.0.0"), 443, conf =>
            {
                conf.UseHttps(cert);
            });
        });
    }
    else
    {
        throw new InvalidOperationException("Certificate path or password (CERT_PASSWORD, CERT_PATH) environment variables are missing.");
    }
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

builder.Services.AddMemoryCache();

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<RateLimitFilterAttribute>();
});
//builder.Services.AddControllers();

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

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

// Just an html, where you need to describe connection methods
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Environment.GetEnvironmentVariable("STATIC_PATH") == null 
        ? Environment.GetEnvironmentVariable("STATIC_PATH", EnvironmentVariableTarget.Machine)
        : Environment.GetEnvironmentVariable("STATIC_PATH")),
    RequestPath = new PathString("")
});
// redirecting / to /index.html
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/index.html");
        return;
    }
    await next();
});

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
                Title = err.Message,
                Status = err.StatusCode,
                Errors = new()
            };

            errResp.Errors.Add(err.Field, new[] { err.Message });

            context.Response.StatusCode = (int)errResp.Status;
        }
        else
        {
            errResp = new ErrorModel()
            {
                Title = "Server side error",
                Status = System.Net.HttpStatusCode.InternalServerError,
                Errors = new()
            };

            errResp.Errors.Add("Server", new[] { "Something went wrong on server" });

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        await context.Response.WriteAsJsonAsync(errResp);
    });
});

app.Run();


// // TODO
//Global 2 bana mncae
//1.    message queueing enabled in message model
//2.    ssltls certificates
//3.    UI commands starting with /
//4.    temporary groups
//5.    siktir UI alerts, luchshe sax messageov arvi
//6.    better logging 
//et e hedo

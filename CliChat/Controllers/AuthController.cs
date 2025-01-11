using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CliChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public AuthController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            //TODO validate with attribute
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.Check(loginDto);

            var token = GenerateJsonWebToken(user, 8); // Token valid for 8 hours

            return Ok(new { Token = token });
        }
        [HttpPut]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            //TODO validate with attribute
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.Register(registerDto);

            var token = GenerateJsonWebToken(user, 8); // Token valid for 8 hours

            return Ok(new { Token = token });
        }

        private string GenerateJsonWebToken(LoginDto? userInfo, int hours)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["JwtSettings:Issuer"],
                _config["JwtSettings:Audience"],
                new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, userInfo!.Id.ToString()),
                    new(JwtRegisteredClaimNames.Sid, userInfo!.Id.ToString()),
                    //TODO change LoginDto 
                    new(JwtRegisteredClaimNames.Name, userInfo!.Username)
                },
                expires: DateTime.Now.AddHours(hours),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

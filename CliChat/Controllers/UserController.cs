using Business.Interfaces;
using Business.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CliChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        IUserService _userService;
        public UserController(IUserService userService)
        {
             _userService = userService;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var userInfo = await _userService.GetInfo(username);
            return Ok(userInfo);
        }
        [HttpDelete()]
        public async Task<IActionResult> Delete()
        {
            await _userService.Delete(User.GetClaim(JwtRegisteredClaimNames.Name));
            return Ok();
        }
    }
}

using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var userInfo = _userService.GetInfo(username);
            return Ok(userInfo);
        }
    }
}

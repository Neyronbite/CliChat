using Business.Interfaces;
using Business.Models;
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
    public class MessageController : ControllerBase
    {
        IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<List<MessageModel>> Get()
        {
            var username = User.GetClaim(JwtRegisteredClaimNames.Name);
            var messages = await _messageService.GetQueued(username);

            return messages;
        }
    }
}

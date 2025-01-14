using Business.Interfaces;
using Business.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CliChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        /// <summary>
        /// string is users unique name (Username), 
        /// and in hashset we add users connectionid and all other usefull info
        /// </summary>
        private readonly ConnectionMapping<string> _userMapping;

        public ChatHub(ConnectionMapping<string> userMapping)
        {
            _userMapping = userMapping;
        }

        /// <summary>
        /// this function is for clients symetric key exchange process
        /// it just transfers objects that sent, and nothing else
        /// </summary>
        /// <param name="data">data to be transfered</param>
        /// <param name="to">addressant user's username</param>
        /// <returns></returns>
        public async Task KeyExchange(object data, string to)
        {
            await DataTransferBetweenUsers("KeyExchange", data, to);
        }

        /// <summary>
        /// main message sending logic
        /// </summary>
        /// <param name="message">string message</param>
        /// <param name="to">addressant user's username</param>
        /// <returns></returns>
        public async Task SendMessage(string message, string to)
        {
            //TODO handle message queues if offline
            await DataTransferBetweenUsers("ReceiveMessage", message, to);
        }

        /// <summary>
        /// on connection, we identify user by jwt token
        /// and adding his connection id to mappings
        /// so we can eazily get all users id's with their usernames
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var name = Context.User.GetClaim(JwtRegisteredClaimNames.Name);
            _userMapping.Add(name, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// deleting all users data from mapping
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string name = Context.User.GetClaim(JwtRegisteredClaimNames.Name);

            _userMapping.Remove(name, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// method to send errors
        /// it sends errors to "ReceiveError" function on client side
        /// </summary>
        /// <param name="err"></param>
        private async void SendError(string connectionId, Exception err)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveError", Context.User.GetClaim(JwtRegisteredClaimNames.Name), err.Message);
        }

        /// <summary>
        /// main data transfer logic
        /// we identify user by jwt token
        /// checking if user exists, and online
        /// and sending data to execute remoteFunction
        /// </summary>
        /// <param name="remoteFunction">function name on clients side</param>
        /// <param name="data">data to be transfered</param>
        /// <param name="toUsername">addresant user's username</param>
        /// <param name="handleOfflineCase">if user is offline, it executes this function or by default will send error</param>
        /// <returns></returns>
        private async Task DataTransferBetweenUsers(string remoteFunction, object data, string toUsername, Action handleOfflineCase = null)
        {
            //getting username from jwt
            var name = Context.User.GetClaim(JwtRegisteredClaimNames.Name);

            //getting to connection id from ConnectionMappings dictionary
            var connectionIds = _userMapping.GetConnections(toUsername);

            //handling offline case
            if (connectionIds == null || connectionIds.Count() == 0)
            {
                if (handleOfflineCase != null)
                {
                    handleOfflineCase();
                }
                else
                {
                    foreach (var cId in _userMapping.GetConnections(name))
                    {
                        SendError(cId, new Exception("user is offline"));
                    }
                }
            }

            //sending message to all connected id's (devices)
            foreach (var connectionId in connectionIds)
            {
                await Clients.Client(connectionId).SendAsync(remoteFunction, Context.User.GetClaim(JwtRegisteredClaimNames.Name), data);
            }
        }
    }
}

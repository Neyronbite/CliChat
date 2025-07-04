﻿using Business.Interfaces;
using Business.Models;
using Business.Services;
using Business.Utils;
using Data.Entities;
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
        private readonly ConnectionMappingService<string> _userMapping;
        private readonly ConnectionMappingService<long> _groupMapping;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        // getting username from jwt
        private string CurrentUsername => Context.User.GetClaim(JwtRegisteredClaimNames.Name);
        // getting current session connections
        private IEnumerable<string> CurrentConnections => _userMapping.GetConnections(CurrentUsername);

        public ChatHub(ConnectionMappingService<string> userMapping, ConnectionMappingService<long> groupMapping, IUserService userService, IMessageService messageService)
        {
            _userMapping = userMapping;
            _userService = userService;
            _messageService = messageService;
            _groupMapping = groupMapping;
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
            if (await _userService.UserExist(to))
            {
                await DataTransferBetweenUsers("ReceiveMessage", message, to, async () =>
                {
                    // Queueing message
                    await _messageService.Queue(
                        new Business.Models.MessageModel()
                        {
                            Message = message,
                            To = to,
                            From = CurrentUsername
                        });

                    SendError(CurrentConnections, new Exception($"User {to} is offline, queueing message"));
                });
            }
            else if (long.TryParse(to, out long groupId) && _groupMapping.GetConnections(groupId).Any())
            {
                // identifying group members
                // TODO I need to change this function name
                var usernames = _groupMapping.GetConnections(groupId);

                if (!usernames.Contains(CurrentUsername))
                {
                    SendError(CurrentConnections, new Exception($"group {to} does not exist or you are not a group member"));
                    return;
                }

                // sending group message as standart message
                // but with custom from user format
                // groupId:CurrentUsername
                var fromUsername = $"{groupId}:{CurrentUsername}";

                // sending message to group members
                foreach (var username in usernames)
                {
                    if (username == CurrentUsername)
                    {
                        continue;
                    }
                    await DataTransferBetweenUsers("ReceiveMessage", message, username, fromUsername, async () =>
                    {
                        // Queueing message
                        await _messageService.Queue(
                            new Business.Models.MessageModel()
                            {
                                Message = message,
                                To = username,
                                From = groupId.ToString()
                                // TODO maybe add isGroupMessage to message model, and fromUsername IDK
                            });

                        SendError(CurrentConnections, new Exception($"User {username} is offline, queueing message"));
                    });
                }
            }
            else
            {
                SendError(CurrentConnections, new Exception($"the user {to} does not exist or is offline"));
            }
        }

        /// <summary>
        /// Creates a temporary group
        /// Sends Exchange requests to users
        /// </summary>
        /// <param name="users">usernames of that group</param>
        /// <returns></returns>
        public async Task CreateGroup(string[] users)
        {
            long groupId = Random.Shared.NextInt64();
            _groupMapping.Add(groupId, CurrentUsername);

            // Notifying owner, that group was created
            var ownerExchObj = new GroupKeyExchangeModel()
            {
                OwnerUsername = CurrentUsername,
                ServerRequest = true,
                GroupId = groupId.ToString()
            };
            await DataTransferBetweenUsers("CreateGroup", ownerExchObj, CurrentUsername, specifyFromUser: false);

            foreach (var user in users)
            {
                _groupMapping.Add(groupId, user);

                var exchObj = new GroupKeyExchangeModel()
                {
                    OwnerUsername = CurrentUsername,
                    To = user,
                    From = CurrentUsername,
                    ServerRequest = true,
                    GroupId = groupId.ToString()
                };
                // GroupKeyExchange
                await DataTransferBetweenUsers("GroupKeyExchange", exchObj, user, specifyFromUser: false);
            }
        }

        /// <summary>
        /// Handles key exchange process between group members and owner
        /// </summary>
        /// <param name="data">key exchange model</param>
        /// <returns></returns>
        public async Task GroupKeyExchange(GroupKeyExchangeModel data)
        {
            long groupId = long.Parse(data.GroupId);
            var groupMembers = _groupMapping.GetConnections(groupId);

            if (!groupMembers.Contains(CurrentUsername))
            {
                SendError(CurrentConnections, new Exception("what are you trying to do???"));
                return;
            }

            await DataTransferBetweenUsers("GroupKeyExchange", data, data.To, specifyFromUser: false);
        }

        /// <summary>
        /// on connection, we identify user by jwt token
        /// and adding his connection id to mappings
        /// so we can eazily get all users id's with their usernames
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var name = CurrentUsername;
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
            string name = CurrentUsername;

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
            await Clients.Client(connectionId).SendAsync("ReceiveError", CurrentUsername, err.Message);
        }
        private async void SendError(IEnumerable<string> connectionIds, Exception err)
        {
            foreach (var connectionId in connectionIds)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveError", CurrentUsername, err.Message);
            }
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
        private async Task DataTransferBetweenUsers(string remoteFunction, object data, string toUsername, string fromUsername, Func<Task> handleOfflineCase = null, bool specifyFromUser = true)
        {
            //getting to connection id from ConnectionMappings dictionary
            var connectionIds = _userMapping.GetConnections(toUsername);

            //handling offline case
            if (connectionIds == null || connectionIds.Count() == 0)
            {
                if (handleOfflineCase != null)
                {
                    await handleOfflineCase();
                }
                else
                {
                    SendError(CurrentConnections, new Exception($"the user {toUsername} does not exist or is offline"));
                }
            }

            //sending message to all connected id's (devices)
            foreach (var connectionId in connectionIds)
            {
                // I decited to do this grdon
                // If we manually write specifyFromUser as false
                // that means we want to send group exchange message
                if (specifyFromUser)
                {
                    await Clients.Client(connectionId).SendAsync(remoteFunction, fromUsername, data);
                }
                else
                {
                    await Clients.Client(connectionId).SendAsync(remoteFunction, data);
                }
            }
        }
        private async Task DataTransferBetweenUsers(string remoteFunction, object data, string toUsername, Func<Task> handleOfflineCase = null, bool specifyFromUser = true)
        {
            await DataTransferBetweenUsers(remoteFunction, data, toUsername, CurrentUsername, handleOfflineCase, specifyFromUser);
        }
    }
}

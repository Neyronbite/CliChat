using Business.Exceptions;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class MessageService : IMessageService
    {
        IUnitOfWork _unitOfWork;
        IUserService _userService;

        public MessageService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<List<MessageModel>> GetQueued(string username)
        {
            User userEntity;

            try
            {
                userEntity = await _unitOfWork.UserRepository.GetFirst(u => u.Username == username, u => u.ReceivedMessages);
            }
            catch (Exception)
            {
                throw new UserDoesNotExistException();
            }

            var messages = userEntity.ReceivedMessages.Select(m => new MessageModel()
            {
                From = m.From,
                To = m.To,
                Message = m.MessageEncrypted
            }).ToList();

            foreach (var message in userEntity.ReceivedMessages)
            {
                _unitOfWork.MessageRepository.Delete(message);
            }

            _unitOfWork.Commit();

            return messages;
        }

        public async Task Queue(MessageModel message)
        {
            var from = await _unitOfWork.UserRepository.GetFirst(u => u.Username == message.From);
            var to = await _unitOfWork.UserRepository.GetFirst(u => u.Username == message.To);

            var messageEntity = new Message()
            {
                FromId = from.Id,
                ToId = to.Id,
                From = message.From,
                To = message.To,
                MessageEncrypted = message.Message
            };

            _unitOfWork.MessageRepository.Insert(messageEntity);

            _unitOfWork.Commit();
        }
    }
}

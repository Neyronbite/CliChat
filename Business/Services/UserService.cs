using Business.Exceptions;
using Business.Interfaces;
using Business.Models;
using Business.Utils;
using Data.Entities;
using Data.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class UserService : IUserService
    {
        IUnitOfWork _unitOfWork;
        IConfiguration _config;
        ConnectionMapping<string> _userMappings;

        public UserService(IUnitOfWork unitOfWork, IConfiguration config, ConnectionMapping<string> userMappings)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _userMappings = userMappings;
        }

        public async Task<LoginDto> Check(LoginDto loginDto)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetFirst(u => u.Username == loginDto.Username);

                if (!user.Password.CheckHash(loginDto.Password))
                {
                    throw new InvalidOperationException();
                }

                return new LoginDto() { Username = user.Username, Id = user.Id, Password = "passed" };
            }
            catch (InvalidOperationException)
            {
                throw new WronCredentialsException();
            }
        }

        public async Task<UserInfo> GetInfo(string username)
        {
            User userEntity;
            try
            {
                userEntity = await _unitOfWork.UserRepository.GetFirst(u => u.Username == username);
            }
            catch (Exception)
            {
                throw new UserDoesNotExistException();
            }

            var userInfo = new UserInfo()
            {
                Username = userEntity.Username,
                PublicKey = userEntity.PublicKey
            };

            // if is connected to chat hub, then true
            userInfo.IsOnline = _userMappings.GetConnections(username).Any();

            return userInfo;
        }

        public async Task<LoginDto> Register(RegisterDto register)
        {
            var entitiyList = await _unitOfWork.UserRepository.Get(u => u.Username == register.Username, showDeleted: true);

            if (entitiyList.Count != 0)
            {
                throw new UserExistException();
            }
         
            var user = new User();
            user.Username = register.Username;
            user.Password = register.Password.Hash();
            user.PublicKey = register.PublicKey;

            _unitOfWork.UserRepository.Insert(user);

            _unitOfWork.Commit();

            return new LoginDto() { Username = user.Username, Id = user.Id, Password = "passed" };
        }
    }
}

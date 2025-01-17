using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageModel>> GetQueued(string username);
        Task Queue(MessageModel message);
    }
}

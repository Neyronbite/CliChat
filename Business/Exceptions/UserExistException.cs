using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Exceptions
{
    public class UserExistException : ClientSideException
    {
        public UserExistException() : base(System.Net.HttpStatusCode.Conflict, "User already exist")
        {
        }
    }
}

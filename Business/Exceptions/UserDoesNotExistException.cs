using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Exceptions
{
    public class UserDoesNotExistException : ClientSideException
    {
        public UserDoesNotExistException() : base(System.Net.HttpStatusCode.NotFound, "User Does not exist", "User")
        {
        }
    }
}

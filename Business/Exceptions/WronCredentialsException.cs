using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Exceptions
{
    public class WronCredentialsException : ClientSideException
    {
        public WronCredentialsException() : base(System.Net.HttpStatusCode.Conflict, "Wrong Credentials")
        {
        }
    }
}

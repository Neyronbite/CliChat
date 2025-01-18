using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Exceptions
{
    public class TooManyRequestsException : ClientSideException
    {
        public TooManyRequestsException() : base(System.Net.HttpStatusCode.TooManyRequests, "Rate limit exceeded. Try again later.", "Request")
        {
        }
    }
}

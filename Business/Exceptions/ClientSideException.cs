using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Exceptions
{
    public class ClientSideException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Field { get; set; }
        public ClientSideException(HttpStatusCode statusCode, string message, string field) : base(message)
        {
            StatusCode = statusCode;
            Field = field;
        }
    }
}

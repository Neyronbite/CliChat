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
        public string? Suggestions { get; set; }
        public ClientSideException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
        public ClientSideException(HttpStatusCode statusCode, string message, string suggestions) : base(message)
        {
            StatusCode = statusCode;
            Suggestions = suggestions;
        }
    }
}

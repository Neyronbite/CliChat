using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class ErrorModel
    {
        // TODO I dont understand how modelstate validation works,
        // and why I can't throw new exception inside ActionFilter 
        // Modelstate errors response sends before triggering OnActionExecuting
        public HttpStatusCode Status { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}

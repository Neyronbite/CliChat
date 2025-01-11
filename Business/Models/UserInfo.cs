using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string PublicKey { get; set; }
        public bool IsOnline { get; set; }
    }
}

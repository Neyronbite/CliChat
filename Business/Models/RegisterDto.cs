using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class RegisterDto
    {
        [MaxLength(50)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$",
            ErrorMessage = "Username must contain only letters, digits or '-', '_' symbols")]
        [Required]
        public string Username { get; set; }
        [MaxLength(100)]
        [MinLength(3)]
        [Required]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^[-A-Za-z0-9+/]*={0,3}$",
            ErrorMessage = "Public key must be base 64 encoded string")]
        public string PublicKey { get; set; }
    }
}

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
        //TODO add error messages vor ModelState
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
        public string PublicKey { get; set; }
    }
}

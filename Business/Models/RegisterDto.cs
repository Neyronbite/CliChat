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
        [MinLength(4)]
        [Required]
        public string Username { get; set; }
        [MaxLength(100)]
        [MinLength(6)]
        [Required]
        public string Password { get; set; }
        [MaxLength(100)]
        [Required]
        public string PublicKey { get; set; }
    }
}

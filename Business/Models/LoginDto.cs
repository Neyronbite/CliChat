using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class LoginDto
    {
        //TODO add error messages vor ModelState
        public int Id { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(100)]
        [MinLength(6)]
        [Required]
        public string Password { get; set; }
    }
}

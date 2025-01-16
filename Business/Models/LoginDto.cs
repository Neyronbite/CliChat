using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class LoginDto
    {
        //TODO add error messages vor ModelState
        public int Id { get; set; }
        [MaxLength(50)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$", 
            ErrorMessage = "Username must contain only letters, digits or '-', '_' symbols")]
        public string Username { get; set; }
        [MaxLength(100)]
        [MinLength(3)]
        [Required]
        public string Password { get; set; }
    }
}

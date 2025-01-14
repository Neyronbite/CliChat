using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    //Data Source=.\ChatDb.db;Version=3;
    public class User : BaseEntity
    {
        [MaxLength(50)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$")]
        public string Username { get; set; }
        [MaxLength(100)]
        [MinLength(3)]
        [Required]
        public string Password { get; set; }
        [Required]
        public string PublicKey { get; set; }
    }
}

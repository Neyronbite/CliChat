using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Index(nameof(Username), IsUnique = true)]
    public class User : BaseEntity
    {
        [MaxLength(50)]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z]+([._]?[a-zA-Z0-9]+)*$")]
        public string Username { get; set; }
        [MaxLength(100)]
        [MinLength(3)]
        [Required]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^[-A-Za-z0-9+/]*={0,3}$")]
        public string PublicKey { get; set; }

        //public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
    }
}

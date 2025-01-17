using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Index(nameof(ToId))]
    public class Message : BaseEntity
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string MessageEncrypted { get; set; }

        public User FromUser { get; set; }
        public User ToUser { get; set; }
    }
}

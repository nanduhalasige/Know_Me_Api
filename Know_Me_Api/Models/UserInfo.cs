using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Know_Me_Api.Models
{
    public class UserInfo
    {
        [Key]
        public Guid userId { get; set; }
        [Required,MaxLength(20)]
        public string firstName { get; set; }
        [Required, MaxLength(20)]
        public string lastName { get; set; }
        [Required]
        public string email { get; set; }
        [Required, MaxLength(20)]
        public string userName { get; set; }
        public string password { get; set; }
        public bool? IsExternal { get; set; }
        public bool isActive { get; set; }
        public int? roleId { get; set; }


        public virtual Role Role { get; set; }

    }
}

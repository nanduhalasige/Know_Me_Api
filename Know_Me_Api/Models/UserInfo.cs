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
        [Required(ErrorMessage ="First Name is required"),MaxLength(20)]
        public string firstName { get; set; }
        [Required(ErrorMessage = "Last Name is required"), MaxLength(20)]
        public string lastName { get; set; }
        [Required(ErrorMessage = "Email Name is required")]
        public string email { get; set; }
        [Required(ErrorMessage = "Username is required"), MaxLength(20)]
        public string userName { get; set; }
        [Required(ErrorMessage = "Password is required"), MaxLength(20)]
        public string password { get; set; }
        public bool isActive { get; set; }
        public int? roleId { get; set; }


        public virtual Role Role { get; set; }

    }
}

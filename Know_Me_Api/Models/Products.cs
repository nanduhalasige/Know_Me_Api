using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Know_Me_Api.Models
{
    public class Products
    {
        [Key]
        public Guid productId { get; set; }
        [Required]
        public string prodName { get; set; }
        public int quantity { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
    }
}
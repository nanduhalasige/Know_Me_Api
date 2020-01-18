using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Know_Me_Api.Models
{
    public class WareHouseProducts
    {
        [Key]
        public int Id { get; set; }
        public int wareHouseId { get; set; }
        public string productId { get; set; }
        public int quantity { get; set; }
    }
}

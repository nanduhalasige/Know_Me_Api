using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Know_Me_Api.Models
{
    public class WareHouse
    {
        [Key]
        public int WareHouseId { get; set; }
        public string WhName { get; set; }
    }
}

﻿using System;
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
        public string manufacturerName { get; set; }
        [Required]
        public string modelName { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
    }
}
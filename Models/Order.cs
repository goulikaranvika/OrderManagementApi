using System;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementApi.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        
        public decimal TotalAmount { get; set; }

       
        public string UserId { get; set; } = string.Empty;
    }
}


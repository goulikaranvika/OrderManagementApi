using System.ComponentModel.DataAnnotations;

namespace OrderManagementApi.DTOs
{
    public class OrderDto
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }
}

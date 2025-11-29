using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.DTOs;
using OrderManagementApi.Models;
using System.Security.Claims;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Helper method to get logged-in user's UserId from JWT
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        // ✅ GET: api/orders
        // Returns all orders of the logged-in user
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return Ok(orders);
        }

        // ✅ GET: api/orders/{id}
        // Returns a specific order of the logged-in user
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = GetUserId();

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // ✅ POST: api/orders
        // Creates a new order for the logged-in user
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();

            var order = new Order
            {
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                TotalAmount = dto.Quantity * dto.UnitPrice,
                UserId = userId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // ✅ PUT: api/orders/{id}
        // Updates an existing order of the logged-in user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            order.ProductName = dto.ProductName;
            order.Quantity = dto.Quantity;
            order.UnitPrice = dto.UnitPrice;
            order.TotalAmount = dto.Quantity * dto.UnitPrice;

            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // ✅ DELETE: api/orders/{id}
        // Deletes an order of the logged-in user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var userId = GetUserId();

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

using System.Security.Claims;
using ComputerStore.DTO;
using ComputerStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext context;

        public OrderController(AppDbContext context)
        {
            this.context = context;
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lấy danh sách đơn hàng (Admin)",
            Description = "Admin có thể xem tất cả các đơn hàng và hỗ trợ phân trang."
        )]
        public IActionResult GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Số trang và kích thước trang phải lớn hơn 0.");
            }

            var query = context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            var totalItems = query.Count();
            var orders = query
                .Skip((page - 1) * pageSize) // Bỏ qua các đơn hàng của các trang trước
                .Take(pageSize)             // Lấy số lượng đơn hàng theo kích thước trang
                .Select(o => new
                {
                    o.Id,
                    User = new
                    {
                        o.User.Id,
                        o.User.Email,
                        o.User.Name,
                        o.User.Phone,
                        o.User.Address,
                    },
                    o.TotalAmount,
                    o.OrderDate,
                    o.Status,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.Product.Name,
                        oi.Quantity,
                        oi.Price
                    })
                })
                .ToList();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                orders
            });
        }

        [Authorize]
        [HttpGet("my-orders")]
        [SwaggerOperation(
            Summary = "Lấy danh sách đơn hàng của người dùng",
            Description = "Người dùng có thể xem danh sách đơn hàng của chính mình và hỗ trợ phân trang."
        )]
        public IActionResult GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Số trang và kích thước trang phải lớn hơn 0.");
            }

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Không tìm thấy thông tin người dùng.");
            }

            var userId = int.Parse(userIdClaim.Value);

            var query = context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            var totalItems = query.Count();
            var orders = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    o.Status,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.Product.Name,
                        oi.Quantity,
                        oi.Price
                    })
                })
                .ToList();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                orders
            });
        }

        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Lấy chi tiết đơn hàng",
            Description = "Admin có thể xem chi tiết bất kỳ đơn hàng nào. Người dùng chỉ có thể xem đơn hàng của chính mình."
        )]
        public IActionResult GetOrderById(int id)
        {
            var order = context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Không tìm thấy thông tin người dùng.");
            }

            var userId = int.Parse(userIdClaim.Value);

            if (HttpContext.User.IsInRole(Role.ADMIN) || order.UserId == userId)
            {
                return Ok(new
                {
                    order.Id,
                    order.User.Email,
                    order.TotalAmount,
                    order.OrderDate,
                    order.Status,
                    Items = order.OrderItems.Select(oi => new
                    {
                        oi.Product.Name,
                        oi.Quantity,
                        oi.Price
                    })
                });
            }

            return Forbid("Bạn không có quyền truy cập đơn hàng này.");
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Cập nhật trạng thái đơn hàng (Admin)",
            Description = "Admin có thể cập nhật trạng thái của bất kỳ đơn hàng nào."
        )]
        public IActionResult UpdateOrderStatus(int id, [FromBody] string status)
        {
            var order = context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            order.Status = status;
            context.Orders.Update(order);
            context.SaveChanges();

            return Ok(new
            {
                message = "Cập nhật trạng thái đơn hàng thành công.",
                order
            });
        }

        [Authorize]
        [HttpPost("create")]
        [SwaggerOperation(
            Summary = "Đặt hàng",
            Description = "Người dùng có thể tạo một đơn hàng mới với danh sách sản phẩm và số lượng."
        )]
        public IActionResult CreateOrder([FromBody] CreateOrderDTO orderRequest)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Không tìm thấy thông tin người dùng.");
            }
            if (orderRequest.Items.Any(p => p.Quantity <= 0))
            {
                return BadRequest("Số lượng sản phẩm không hợp lệ. Vui lòng kiểm tra lại.");
            }
            var userId = int.Parse(userIdClaim.Value);

            if (orderRequest.Items == null || !orderRequest.Items.Any())
            {
                return BadRequest("Danh sách sản phẩm không được để trống.");
            }

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();
            foreach (var item in orderRequest.Items)
            {
                var product = context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Sản phẩm với ID {item.ProductId} không tồn tại.");
                }

                if (product.Quantity < item.Quantity)
                {
                    return BadRequest($"Sản phẩm {product.Name} không đủ số lượng trong kho.");
                }

                var itemPrice = product.Price * (1 - product.DiscountPercent / 100);
                totalAmount += itemPrice * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = itemPrice
                });

                product.Quantity -= item.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                OrderDate = DateTime.UtcNow,
                Status = "Chờ xử lý",
                OrderItems = orderItems
            };

            context.Orders.Add(order);
            context.SaveChanges();

            return Ok(new
            {
                message = "Đặt hàng thành công.",
                order
            });
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Xóa đơn hàng (Admin)",
            Description = "Admin có thể xóa bất kỳ đơn hàng nào."
        )]
        public IActionResult DeleteOrder(int id)
        {
            var order = context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }
            context.OrderItems.RemoveRange(order.OrderItems);
            context.Orders.Remove(order);
            context.SaveChanges();
            return Ok(new
            {
                message = "Xóa đơn hàng thành công.",
                orderId = id
            });
        }
    }
}
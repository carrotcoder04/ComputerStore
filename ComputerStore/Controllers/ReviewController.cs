using System.Security.Claims;
using ComputerStore.Models;
using ComputerStore.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("api/review")]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext context;

        public ReviewController(AppDbContext context)
        {
            this.context = context;
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lấy tất cả đánh giá (Admin)",
            Description = "Admin có thể lấy danh sách tất cả đánh giá trong hệ thống và hỗ trợ phân trang."
        )]
        public IActionResult GetAllReviews([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Số trang và kích thước trang phải lớn hơn 0.");
            }

            var query = context.Reviews.AsQueryable();
            var totalItems = query.Count();
            var reviews = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    r.Id,
                    r.UserId,
                    r.ProductId,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                })
                .ToList();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                reviews
            });
        }
        [HttpGet("product/{productId}")]
        [SwaggerOperation(
            Summary = "Lấy danh sách đánh giá của sản phẩm",
            Description = "Trả về danh sách các đánh giá của một sản phẩm dựa trên mã sản phẩm và hỗ trợ phân trang."
        )]
        public IActionResult GetReviewsByProduct(int productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Số trang và kích thước trang phải lớn hơn 0.");
            }

            var query = context.Reviews.Where(r => r.ProductId == productId).AsQueryable();

            var totalItems = query.Count();
            var reviews = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    UserId = r.UserId,
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    Comment = r.Comment
                })
                .ToList();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                reviews
            });
        }

        [Authorize]
        [HttpPost("create")]
        [SwaggerOperation(
            Summary = "Thêm đánh giá mới",
            Description = "Người dùng có thể thêm đánh giá mới cho một sản phẩm."
        )]
        public IActionResult AddReview([FromBody] ReviewDTO reviewDto)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Không tìm thấy thông tin người dùng.");
            }

            var review = new Review
            {
                UserId = int.Parse(userIdClaim.Value),
                ProductId = reviewDto.ProductId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            context.Reviews.Add(review);
            context.SaveChanges();

            return Ok(new
            {
                message = "Thêm đánh giá thành công.",
                review
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Cập nhật đánh giá",
            Description = "Người dùng có thể cập nhật đánh giá của chính mình."
        )]
        public IActionResult UpdateReview(int id, [FromBody] ReviewDTO reviewDto)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Không tìm thấy thông tin người dùng.");
            }

            var review = context.Reviews.FirstOrDefault(r => r.Id == id && r.UserId == int.Parse(userIdClaim.Value));
            if (review == null)
            {
                return NotFound("Đánh giá không tồn tại hoặc bạn không có quyền chỉnh sửa.");
            }

            review.Rating = reviewDto.Rating;
            review.Comment = reviewDto.Comment;
            context.Reviews.Update(review);
            context.SaveChanges();

            return Ok(new
            {
                message = "Cập nhật đánh giá thành công.",
                review
            });
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Xóa đánh giá (Admin)",
            Description = "Admin có thể xóa bất kỳ đánh giá nào."
        )]
        public IActionResult DeleteReview(int id)
        {
            var review = context.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
            {
                return NotFound("Đánh giá không tồn tại.");
            }

            context.Reviews.Remove(review);
            context.SaveChanges();

            return Ok(new
            {
                message = "Xóa đánh giá thành công.",
                review
            });
        }
    }
}
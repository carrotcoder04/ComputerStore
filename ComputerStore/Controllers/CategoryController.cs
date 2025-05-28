using ComputerStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext context;

        public CategoryController(AppDbContext context)
        {
            this.context = context;
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpPost("create")]
        [SwaggerOperation(
            Summary = "Tạo danh mục mới (Admin)",
            Description = "Thêm một danh mục mới vào cơ sở dữ liệu. Chỉ dành cho quản trị viên."
        )]
        public IActionResult CreateCategory([FromBody] string categoryName)
        {
            if (context.Categories.Any(c => c.Name == categoryName))
            {
                return BadRequest("Danh mục đã tồn tại!");
            }

            var category = context.Categories.Add(new Category()
            {
                Name = categoryName
            });

            context.SaveChanges();
            return Ok(category.Entity);
        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lấy tất cả danh mục",
            Description = "Trả về danh sách tất cả các danh mục."
        )]
        public IActionResult GetAllCategories()
        {
            var categories = context.Categories.ToList();
            return Ok(categories);
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Xóa danh mục (Admin)",
            Description = "Admin có thể xóa một danh mục dựa trên mã danh mục."
        )]
        public IActionResult DeleteCategory(int id)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }

            // Kiểm tra nếu danh mục có sản phẩm liên kết
            if (context.Products.Any(p => p.CategoryId == id))
            {
                return BadRequest("Không thể xóa danh mục vì có sản phẩm liên kết.");
            }

            context.Categories.Remove(category);
            context.SaveChanges();

            return Ok(new
            {
                message = "Xóa danh mục thành công.",
                categoryId = id
            });
        }
    }
}

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
            Summary = "Tạo danh mục mới",
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
    }
}

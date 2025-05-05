using ComputerStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult CreateCategory([FromBody] string categoryName)
        {
            if (context.Categories.Any(c => c.Name == categoryName))
            {
                return BadRequest("Category existed!");
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

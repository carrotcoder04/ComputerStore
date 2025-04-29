using ComputerStore.Models;
using ComputerStore.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext context;
        public ProductsController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lấy tất cả sản phẩm",
            Description = "Trả về danh sách tất cả sản phẩm theo danh mục."
        )]
        public IActionResult GetAll()
        {
            var categories = context.Categories.Select(c => new
            {
                c.Id,
                c.Name,
                Products = c.Products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Quantity,
                    p.DiscountPercent,
                    p.PromotionEndDate
                }).ToArray()
            }).ToArray();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Lấy sản phẩm theo mã sản phẩm",
            Description = "Trả về sản phẩm theo mã tìm kiếm."
        )]
        public IActionResult GetById(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            return Ok(product);
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpPost("createcategory")]
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
        [Authorize(Roles = Role.ADMIN)]
        [HttpPost("createproduct")]
        public IActionResult Create([FromBody] CreateProductDTO product)
        {
            if (!context.Categories.Any(c => c.Id == product.CategoryId))
            {
                return BadRequest("Category doesn't exist!");
            }
            var newProduct = context.Products.Add(new Product()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                DiscountPercent = product.DiscountPercent,
                PromotionEndDate = product.PromotionEndDate,
                CategoryId = product.CategoryId,
                WarrentyPeriod = product.WarrentyPeriod,
            });
            context.SaveChanges();
            return Ok(newProduct.Entity);
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] string name)
        {
            return Ok($"Updated {id}: {name}");
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Deleted product {id}");
        }
    }
}

using ComputerStore.Models;
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
        public IActionResult GetById(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            return Ok(product);
        }
        [HttpPost]
        public IActionResult Create([FromBody] (int Time, string Name) body)
        {
            return Ok($"Created: {body.Name} with time {body.Time}");
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] string name)
        {
            return Ok($"Updated {id}: {name}");
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Deleted product {id}");
        }
        [HttpPost("seed")]
        public async Task<IActionResult> SeedData()
        {
            var products = new List<Product>
            {
                new Product { Name = "Laptop A", Price = 1000, CategoryId = 1 },
                new Product { Name = "Laptop B", Price = 1500, CategoryId = 1},
                new Product { Name = "Laptop C", Price = 2000, CategoryId = 1 }
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
            return Ok("Seeded successfully");
        }
    }
}

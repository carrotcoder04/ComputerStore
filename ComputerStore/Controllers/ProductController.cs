using ComputerStore.DTO;
using ComputerStore.Models;
using ComputerStore.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext context;
        public ProductController(AppDbContext context)
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
        [HttpPost("create")]
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
            if(!string.IsNullOrEmpty(product.Base64Image))
            {
                string fileName = $"{newProduct.Entity.Id}.jpg";
                Resources.SaveBase64Image(product.Base64Image, fileName);
                newProduct.Entity.Image = fileName;
            }
            context.SaveChanges();
            return Ok(newProduct.Entity);
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateProductDTO updateProduct)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if(product == null)
            {
                return BadRequest();
            }
            product.Name = updateProduct.Name;
            product.Description = updateProduct.Description;
            product.Price = updateProduct.Price;
            product.Quantity = updateProduct.Quantity;
            product.DiscountPercent = updateProduct.DiscountPercent;
            product.CategoryId = updateProduct.CategoryId;
            if (!string.IsNullOrEmpty(updateProduct.Base64Image))
            {
                string fileName = $"{product.Id}.jpg";
                Resources.SaveBase64Image(updateProduct.Base64Image, fileName);
            }
            context.SaveChanges();
            return Ok(product);
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return BadRequest();
            }
            var del = context.Products.Remove(product);
            return Ok(del.Entity);
        }
    }
}

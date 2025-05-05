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
            Description = "Trả về danh sách tất cả sản phẩm được nhóm theo danh mục."
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
                    p.PromotionEndDate,
                    p.Image
                }).ToArray()
            }).ToArray();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Lấy sản phẩm theo mã",
            Description = "Trả về thông tin chi tiết của sản phẩm dựa trên mã sản phẩm được cung cấp."
        )]
        public IActionResult GetById(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            return Ok(product);
        }

        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Tìm kiếm sản phẩm",
            Description = "Tìm kiếm sản phẩm theo danh mục và từ khóa."
        )]
        public IActionResult Search([FromQuery] int? categoryId, [FromQuery] string? keyword)
        {
            var query = context.Products.AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword));
            }
            var products = query.ToList();
            return Ok(products);
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpPost("create")]
        [SwaggerOperation(
            Summary = "Tạo sản phẩm mới",
            Description = "Thêm một sản phẩm mới vào cơ sở dữ liệu và liên kết với danh mục hiện có."
        )]
        public IActionResult Create([FromBody] CreateProductDTO product)
        {
            if (!context.Categories.Any(c => c.Id == product.CategoryId))
            {
                return BadRequest("Danh mục không tồn tại!");
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

            if (!string.IsNullOrEmpty(product.Base64Image))
            {
                string fileName = $"{newProduct.Entity.Id}.jpg";
                Resources.SaveBase64Image(product.Base64Image, fileName);
                newProduct.Entity.Image = fileName;
            }

            context.SaveChanges();
            return Ok(new
            {
                message = "Thêm sản phẩm thành công.",
                product = newProduct.Entity
            });
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Cập nhật sản phẩm",
            Description = "Cập nhật thông tin của sản phẩm dựa trên mã sản phẩm được cung cấp."
        )]
        public IActionResult Update(int id, [FromBody] UpdateProductDTO updateProduct)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return BadRequest("Sản phẩm không tồn tại.");
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
            return Ok(new
            {
                message = "Cập nhật sản phẩm thành công.",
                product = product
            });
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Xóa sản phẩm",
            Description = "Xóa sản phẩm khỏi cơ sở dữ liệu dựa trên mã sản phẩm được cung cấp."
        )]
        public IActionResult Delete(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return BadRequest("Sản phẩm không tồn tại.");
            }

            var del = context.Products.Remove(product);
            context.SaveChanges();
            return Ok(new
            {
                message = "Xóa sản phẩm thành công.",
                product = del.Entity
            });
        }
    }
}

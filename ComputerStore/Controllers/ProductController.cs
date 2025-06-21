using System.Security.Claims;
using ComputerStore.DTO;
using ComputerStore.Models;
using ComputerStore.Requests;
using ComputerStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : BaseController
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpPost("create")]
        [SwaggerOperation(
            Summary = "Tạo sản phẩm mới (Admin)",
            Description = "Thêm một sản phẩm mới vào cơ sở dữ liệu và liên kết với danh mục hiện có. Chỉ dành cho Admin."
        )]
        public async Task<IActionResult> Create([FromForm] CreateProductRequest product)
        {
            try
            {
                var newProduct = await _service.CreateProduct(UserId, product);
                return Success(new
                {
                    Message = "Tạo sản phẩm thành công",
                    Product = newProduct
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lấy tất cả sản phẩm",
            Description = "Trả về danh sách tất cả sản phẩm được nhóm theo danh mục và hỗ trợ phân trang."
        )]
        public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    throw new("Số trang và kích thước trang phải lớn hơn 0.");
                }
                var (products, total) = _service.GetAllProducts(page, pageSize);
                return Success(new
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(total / (float)pageSize),
                    Products = products
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Lấy sản phẩm theo mã",
            Description = "Trả về thông tin chi tiết của sản phẩm dựa trên mã sản phẩm được cung cấp."
        )]
        public IActionResult GetProductById(int id)
        {
            try
            {
                var product = _service.GetProductById(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Tìm kiếm sản phẩm",
            Description = "Tìm kiếm sản phẩm theo danh mục, từ khóa và hỗ trợ phân trang."
        )]
        public IActionResult Search([FromQuery] int? categoryId, [FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _service.QueryProducts(categoryId, keyword);
                var total = query.Count();
                var products = query.Skip((page - 1) * pageSize).Take(pageSize);
                return Success(new
                    {
                        Total = total,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling(total / (float)pageSize),
                        Products = products
                    }
                );
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpPut]
        [SwaggerOperation(
            Summary = "Cập nhật sản phẩm (Admin)",
            Description = "Cập nhật thông tin của sản phẩm dựa trên mã sản phẩm được cung cấp. Chỉ dành cho Admin."
        )]
        public async Task<IActionResult> Update([FromForm] UpdateProductRequest productRequest)
        {
            try
            {
                var newProduct = await _service.UpdateProduct(UserId, productRequest);
                return Success(new
                {
                    Message = "Cập nhật sản phẩm thành công",
                    Product = newProduct
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Xóa sản phẩm (Admin)",
            Description = "Xóa sản phẩm khỏi cơ sở dữ liệu dựa trên mã sản phẩm được cung cấp. Chỉ dành cho Admin."
        )]
        public IActionResult Delete(int id)
        {
            try
            {
                var oldProduct = _service.DeleteProduct(id);
                return Success(new
                {
                    Message = "Xóa sản phẩm thành công.",
                    Product = oldProduct
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        [HttpGet("groupby-category")]
        [SwaggerOperation(
            Summary = "Liệt kê tất cả sản phẩm theo danh mục",
            Description = "Trả về danh sách tất cả sản phẩm được nhóm theo danh mục."
        )]
        public IActionResult GetProductsByCategory()
        {
            var result = _service.GetProductsGroupedByCategory();
            return Success(result);
        }
    }
}

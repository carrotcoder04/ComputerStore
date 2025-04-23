using System.Collections.Generic;
using ComputerStore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ComputerStore.Controllers
{
    public class ProductsController : ControllerBase
    {
        [HttpGet("alls")]
        [SwaggerOperation(
            Summary = "Lấy tất cả sản phẩm",
            Description = "Trả về danh sách tất cả sản phẩm trong cơ sở dữ liệu."
        )]
        public IActionResult GetAll()
        {
            List<Product> products = new List<Product>();
            products.Add(new Product() {
                Name = "PC Gaming",
                Price = 12030302,
                CreateAt = DateTime.Now,
            });
            products.Add(new Product() {
                Name = "PC Văn phòng",
                Price = 272737823,
                CreateAt = DateTime.Now,
            });
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok($"Product {id}");
        }
        [HttpPost]
        public IActionResult Create([FromBody] string name)
        {
            return Ok($"Created: {name}");
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
    }
}

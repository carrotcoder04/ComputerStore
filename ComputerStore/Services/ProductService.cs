using ComputerStore.Models;
using ComputerStore.Requests;
using Microsoft.EntityFrameworkCore;

namespace ComputerStore.Services
{
    public class ProductService : BaseService
    {
        private DbSet<Product> Products => Context.Products;
        private DbSet<Category> Categories => Context.Categories;


        public ProductService(AppDbContext context) : base(context)
        {

        }
        public IQueryable<Product> QueryProducts(int? categoryId, string? keyword)
        {
            var query = Products.AsQueryable();
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.Name.ToLower().Contains(keyword.ToLower()));
            return query;
        }
        private bool IsCategoryExist(int id)
        {
            return Categories.Any(c => c.Id == id);
        }
        public Product GetProductById(int id)
        {
            return Products.FirstOrDefault(p => p.Id== id) ?? throw new($"Sản phẩm {id} không tồn tại");
        }
        public async Task<Product> CreateProduct(string userId, CreateProductRequest productRequest)
        {
            if (!IsCategoryExist(productRequest.CategoryId))
            {
                throw new($"Danh mục {productRequest.CategoryId} không tồn tại");
            }
            string image = await Helper.SaveImageHash(userId, productRequest.Image);
            var newProduct = Products.Add(new Product()
            {
                Name = productRequest.Name,
                Description = productRequest.Description,
                Price = productRequest.Price,
                Quantity = productRequest.Quantity,
                DiscountPercent = productRequest.DiscountPercent,
                PromotionEndDate = productRequest.PromotionEndDate,
                CategoryId = productRequest.CategoryId,
                WarrentyPeriod = productRequest.WarrentyPeriod,
                Image = image
            });
            Context.SaveChanges();
            return newProduct.Entity;
        }
        public async Task<Product> UpdateProduct(string userId, UpdateProductRequest productRequest)
        {
            var product = GetProductById(productRequest.Id);
            if (!IsCategoryExist(productRequest.CategoryId))
            {
                throw new($"Danh mục {productRequest.CategoryId} không tồn tại");
            }
            string image = await Helper.SaveImageHash(userId, productRequest.Image);
            Helper.DeleteFile(product.Image);
            product.Name = productRequest.Name;
            product.Description = productRequest.Description;
            product.Quantity = productRequest.Quantity;
            product.Price = productRequest.Price;
            product.DiscountPercent = productRequest.DiscountPercent;
            product.PromotionEndDate = productRequest.PromotionEndDate;
            product.CategoryId = productRequest.CategoryId;
            product.WarrentyPeriod = productRequest.WarrentyPeriod;
            product.Image = image;
            var newProduct = Products.Update(product);
            Context.SaveChanges();
            return newProduct.Entity;
        }
        public Product DeleteProduct(int id)
        {
            var product = GetProductById(id);
            var oldProduct = Products.Remove(product).Entity;
            Helper.DeleteFile(oldProduct.Image); 
            Context.SaveChanges();
            return oldProduct;
        }
        public IEnumerable<object> GetProductsGroupedByCategory()
        {
            return Categories
                .Include(c => c.Products)
                .Select(c => new
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    Products = c.Products.Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Price,
                        p.Quantity,
                        p.DiscountPercent,
                        p.PromotionEndDate,
                        p.Image
                    })
                });
        }
        public (IEnumerable<Product> Products, int Total) GetAllProducts(int page, int pageSize)
        {
            var query = Products.AsQueryable();
            var total = query.Count();
            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return (products, total);
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Requests
{
    public record BaseProductRequest
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Số lượng không được để trống")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Giá không được để trống")]
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime PromotionEndDate { get; set; }
        public int CategoryId { get; set; }
        public int WarrentyPeriod { get; set; }
        [Required(ErrorMessage = "Hình ảnh không được để trống")]
        public IFormFile Image { get; set; }
    }
}

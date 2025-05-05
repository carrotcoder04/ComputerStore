namespace ComputerStore.DTO
{
    public class UpdateProductDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime PromotionEndDate { get; set; }
        public int CategoryId { get; set; }
        public int WarrentyPeriod { get; set; }
        public string? Base64Image { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace ComputerStore.Models
{

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime PromotionEndDate { get; set; }
        [JsonIgnore]
        public int CategoryId { get; set; }
        public int WarrentyPeriod { get; set; }
        public DateTime CreateAt { get; set; }
    }
}

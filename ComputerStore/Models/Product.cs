using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace ComputerStore.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
        [Column("discount_percent")]
        public int DiscountPercent { get; set; }
        [Column("promotion_end_date")]
        public DateTime? PromotionEndDate { get; set; }
        [ForeignKey("Category")]
        [Column("category_id")]
        public int CategoryId { get; set; }
        [Column("warranty_period")]
        public int WarrentyPeriod { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("image")]
        public string? Image { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
    }
}

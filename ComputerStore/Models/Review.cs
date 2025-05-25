using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ComputerStore.Models
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("rating")]
        public int Rating { get; set; }
        [Column("comment")]
        public string? Comment { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }
        [ForeignKey("ProductId")]
        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
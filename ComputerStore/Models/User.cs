using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerStore.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("gender")]
        public string Gender { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("role")]
        public string Role { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        public object WithoutPassword()
        {
            return new
            {
                Id = this.Id,
                Email = this.Email,
                Name = this.Name,
                Phone = this.Phone,
                Gender = this.Gender,
                Address = this.Address,
                Role = this.Role
            };
        }
    }
    public class Role
    {
        public const string ADMIN = "admin";
        public const string USER = "user";
    }
}
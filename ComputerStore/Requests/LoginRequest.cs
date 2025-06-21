using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Requests
{
    public record LoginRequest
    {
        [Required(ErrorMessage = "Không thể bỏ trống email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Không thể bỏ trống mật khẩu")]
        public string Password { get; set; }
    }
}
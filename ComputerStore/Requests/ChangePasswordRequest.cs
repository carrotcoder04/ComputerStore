using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Requests
{
    public record ChangePasswordRequest
    {
        [Required(ErrorMessage = "Không thể bỏ trống mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Không thể bỏ trống mật khẩu mới")]
        public string NewPassword { get; set; }

    }
}

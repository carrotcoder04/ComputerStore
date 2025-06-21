using System.ComponentModel.DataAnnotations;
namespace ComputerStore.Requests
{
    public record UpdateUserRequest
    {
        [Required(ErrorMessage = "Không thể bỏ trống tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Không thể bỏ trống số điện thoại")]
        public string Phone { get; set; }
        [RegularExpression("^(Nam|Nữ)$", ErrorMessage = "Giới tính chỉ được là 'Nam', 'Nữ'")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Không thể bỏ trống địa chỉ")]
        public string Address { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Requests
{
    public record UpdateProductRequest : BaseProductRequest
    {
        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public int Id { get; set; }
    }
}

using ComputerStore.DTO;
using ComputerStore.Models;
using ComputerStore.Requests;
using ComputerStore.Services;

namespace ComputerStore.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;

    [Route("api/user")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserService _service;
        public UserController(UserService service) => _service = service;

        private IActionResult UpdateUser(int id, UpdateUserRequest userRequest)
        {
            try
            {
                var user = _service.UpdateUser(id, userRequest);
                return Success(new
                {
                    Message = "Cập nhật thông tin thành công",
                    User = user.WithoutPassword()
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        [Authorize]
        [HttpPut("update")]
        [SwaggerOperation(
            Summary = "Cập nhật thông tin người dùng",
            Description = "Cập nhật thông tin người dùng dựa trên ID lấy từ cookie."
        )]
        public IActionResult UpdateUser([FromBody] UpdateUserRequest userRequest)
        {
            return UpdateUser(int.Parse(UserId), userRequest);
        }

        [Authorize]
        [HttpPut("change-password")]
        [SwaggerOperation(
            Summary = "Đổi mật khẩu",
            Description = "Cho phép người dùng đổi mật khẩu của họ."
        )]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest userRequest)
        {
            try
            {
                _service.ChangePassword(int.Parse(UserId),userRequest);
                return Success(new
                {
                    Message = "Đổi mật khẩu thành công."
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpGet("all")]
        [SwaggerOperation(
            Summary = "Lấy tất cả thông tin người dùng (Admin)",
            Description = "Admin có thể lấy danh sách tất cả người dùng trong hệ thống và hỗ trợ phân trang."
        )]
        public IActionResult GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    throw new("Số trang và kích thước trang phải lớn hơn 0.");
                }
                var (users, total) = _service.GetAllUsers(page, pageSize);
                return Ok(new
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                    User = users.Select(u => u.WithoutPassword())
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpPut("admin-update/{id}")]
        [SwaggerOperation(
            Summary = "Cập nhật thông tin người dùng (Admin)",
            Description = "Admin có thể cập nhật thông tin của bất kỳ người dùng nào."
        )]
        public IActionResult AdminUpdateUser(int id, [FromBody] UpdateUserRequest userRequest)
        {
            return UpdateUser(id, userRequest);
        }
        [Authorize(Roles = Role.ADMIN)]
        [HttpDelete("admin-delete/{id}")]
        [SwaggerOperation(
            Summary = "Xóa người dùng (Admin)",
            Description = "Admin có thể xóa bất kỳ người dùng nào."
        )]
        public IActionResult AdminDeleteUser(int id)
        {
            try
            {
                _service.DeleteUser(id);
                return Ok(new { Message = "Xóa người dùng thành công." });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
}
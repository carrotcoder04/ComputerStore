namespace ComputerStore.Controllers
{
    using System.Security.Claims;
    using ComputerStore.DTO;
    using ComputerStore.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;

    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext context;

        public UserController(AppDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpPut("update")]
        [SwaggerOperation(
            Summary = "Cập nhật thông tin người dùng",
            Description = "Cập nhật thông tin người dùng dựa trên ID lấy từ cookie."
        )]
        public IActionResult UpdateUser([FromBody] UpdateUserDTO updateRequest)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Không tìm thấy thông tin người dùng trong cookie.");
            }

            var user = context.Users.FirstOrDefault(u => u.Email == userIdClaim.Value);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }
            user.Name = updateRequest.Name ?? user.Name;
            user.Phone = updateRequest.Phone ?? user.Phone;
            user.Gender = updateRequest.Gender ?? user.Gender;
            user.Address = updateRequest.Address ?? user.Address;
            context.Users.Update(user);
            context.SaveChanges();
            return Ok(new
            {
                message = "Cập nhật thông tin thành công.",
                user
            });
        }

        [Authorize]
        [HttpPut("change-password")]
        [SwaggerOperation(
            Summary = "Đổi mật khẩu",
            Description = "Cho phép người dùng đổi mật khẩu của họ."
        )]
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO changePasswordRequest)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Không tìm thấy thông tin người dùng trong cookie.");
            }
            var user = context.Users.FirstOrDefault(u => u.Email == userIdClaim.Value);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }
            if (user.Password != changePasswordRequest.OldPassword)
            {
                return BadRequest("Mật khẩu cũ không chính xác.");
            }
            user.Password = changePasswordRequest.NewPassword;
            context.Users.Update(user);
            context.SaveChanges();
            return Ok(new
            {
                message = "Đổi mật khẩu thành công."
            });
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpGet("all")]
        [SwaggerOperation(
            Summary = "Lấy tất cả thông tin người dùng (Admin)",
            Description = "Admin có thể lấy danh sách tất cả người dùng trong hệ thống."
        )]
        public IActionResult GetAllUsers()
        {
            var users = context.Users.ToList();
            return Ok(users);
        }

        [Authorize(Roles = Role.ADMIN)]
        [HttpPut("admin-update/{id}")]
        [SwaggerOperation(
            Summary = "Cập nhật thông tin người dùng (Admin)",
            Description = "Admin có thể cập nhật thông tin của bất kỳ người dùng nào."
        )]
        public IActionResult AdminUpdateUser(int id, [FromBody] UpdateUserDTO updateRequest)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            user.Name = updateRequest.Name ?? user.Name;
            user.Phone = updateRequest.Phone ?? user.Phone;
            user.Gender = updateRequest.Gender ?? user.Gender;
            user.Address = updateRequest.Address ?? user.Address;
            context.Users.Update(user);
            context.SaveChanges();

            return Ok(new
            {
                message = "Cập nhật thông tin người dùng thành công.",
                user
            });
        }
    }
}
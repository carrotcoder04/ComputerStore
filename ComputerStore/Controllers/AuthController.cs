using System.Security.Claims;
using ComputerStore.Models;
using ComputerStore.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics;
using ComputerStore.Services;
using ComputerStore.Requests;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        private readonly UserService _service;
        public AuthController(UserService service) => _service = service;
        private async Task SignInCookie(int userId, string role)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, Program.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(Program.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            });
        }
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Đăng nhập",
            Description = "Xác thực thông tin đăng nhập của người dùng và tạo phiên làm việc."
        )]
        public async Task<IActionResult> Login([FromBody] LoginRequest userRequest)
        {
            try
            {
                var user = _service.CheckUserLogin(userRequest);
                await SignInCookie(user.Id, user.Role);
                return Success(new
                {
                    Message = "Đăng nhập thành công",
                    User = user.WithoutPassword()
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        [SwaggerOperation(
            Summary = "Đăng xuất",
            Description = "Đăng xuất người dùng khỏi hệ thống và kết thúc phiên làm việc."
        )]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Program.AuthenticationScheme);
            return Success(new
            {
                Message = "Đăng xuất thành công.",
            });
        }

        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Đăng ký tài khoản",
            Description = "Tạo tài khoản mới cho người dùng với thông tin được cung cấp."
        )]
        public async Task<IActionResult> Register([FromBody] RegisterRequest userRequest)
        {
            try
            {
                var user = _service.Register(userRequest);
                await SignInCookie(user.Id, user.Role);
                return Success(new
                {
                    Message = "Đăng kí thành công",
                    User = user.WithoutPassword()
                });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
}
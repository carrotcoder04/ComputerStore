using System.Security.Claims;
using ComputerStore.Models;
using ComputerStore.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext context;

        public AuthController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Đăng nhập",
            Description = "Xác thực thông tin đăng nhập của người dùng và tạo phiên làm việc."
        )]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);
            if (user == null)
            {
                return Unauthorized("Thông tin đăng nhập không hợp lệ.");
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, Program.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(Program.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            });

            return Ok(user);
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
            return Ok(new
            {
                message = "Đăng xuất thành công.",
            });
        }

        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Đăng ký tài khoản",
            Description = "Tạo tài khoản mới cho người dùng với thông tin được cung cấp."
        )]
        public IActionResult Register([FromBody] RegisterDTO registerRequest)
        {
            if (context.Users.Any(u => u.Email == registerRequest.Email))
            {
                return BadRequest("Email đã tồn tại.");
            }

            var user = new User
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password,
                Name = registerRequest.Name,
                Phone = registerRequest.Phone,
                Gender = registerRequest.Gender,
                Address = registerRequest.Address,
                Role = Role.USER,
                CreatedAt = DateTime.Now
            };
            context.Users.Add(user);
            context.SaveChanges();
            return Ok(user);
        }
    }
}
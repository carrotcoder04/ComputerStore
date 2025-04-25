using System.Security.Claims;
using ComputerStore.Models;
using ComputerStore.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }
            var claims = new List<Claim> {
                new Claim("email", user.Email),
                new Claim("role", user.Role)
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
        [Authorize(AuthenticationSchemes = Program.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var email = User.FindFirst("email")?.Value;
            var role = User.FindFirst("role")?.Value;
            await HttpContext.SignOutAsync(Program.AuthenticationScheme);
            return Ok(new
            {
                message = "Logged out successfully",
                email = email,
                role = role
            });
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (context.Users.Any(u => u.Email == registerRequest.Email))
            {
                return BadRequest("Email already exists");
            }
            var user = new User
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password,
                Name = registerRequest.Name,
                Phone = registerRequest.Phone,
                Gender = registerRequest.Gender,
                Address = registerRequest.Address,
                Role = "user",
            };
            context.Users.Add(user);
            context.SaveChanges();
            return Ok(user);
        }
    }
}
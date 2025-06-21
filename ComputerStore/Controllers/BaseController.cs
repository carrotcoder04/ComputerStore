using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ComputerStore.Controllers
{
    public class BaseController : ControllerBase
    {
        protected string UserId => HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        protected IActionResult Success(object data) => Ok(data);
        protected IActionResult Error(string message) => BadRequest(new
        {
            Errors = new
            {
                Message = message
            }
        });

        protected IActionResult Error(Exception ex) => Error(ex.Message);
    }
}

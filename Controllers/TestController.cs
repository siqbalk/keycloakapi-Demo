using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public() => Ok("Public endpoint");

        [Authorize]
        [HttpGet("secure")]
        public IActionResult Secure() => Ok("Authenticated users only");

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin() => Ok("Admin only");

        [Authorize(Roles = "User")]
        [HttpGet("user")]
        public IActionResult User() => Ok("User only");
    }
}

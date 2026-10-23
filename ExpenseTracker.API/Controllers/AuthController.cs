using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.DTOs;


namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.DTOs;
using ExpenseTracker.API.Data;
using Microsoft.AspNetCore.Identity;
using ExpenseTracker.API.Services;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _tokenService;

        public AuthController(AppDbContext context, JwtService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            Console.WriteLine("Register method called");
            string username = request.Username;
            string email = request.Email;
            string password = request.Password;
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username, email, and password are required.");
            }
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if(user != null)
            {
                return Conflict("Email is already registered.");
            }
            _context.Users.Add(new Models.User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = new PasswordHasher<object>().HashPassword(null!, password)
            });
            _context.SaveChanges();
            return Ok(new { Message = $"User {username} registered successfully." });

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var passwordHasher = new PasswordHasher<object>();
            string email = request.Email;
            string password = request.Password;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Email and password are required.");
            }
            // Check Database for user is existing or not
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            // If user exists, verify password
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            // Check password
            var result = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);
            if(result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }
            
            // If password is correct, generate JWT token and return it
            var token = _tokenService.GenerateToken(user);
            return Ok(new { Message = $"Login {email} successful.",Token = token });
        }
    }
}

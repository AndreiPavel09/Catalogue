// Backend/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Data;   // Pt ApplicationDbContext
using Backend.Models; // Pt User
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Doar pt [AllowAnonymous]
using System; // Pt Guid
using Microsoft.EntityFrameworkCore; // Pt FirstOrDefaultAsync și Set<T>()
// using Backend.Services; // <-- Eliminăm complet IUserService din controller

// Adaugă using pentru librăria de hashing (ex: BCrypt.Net-Next)
// using BCrypt.Net;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 

        public AuthController(ApplicationDbContext context) 
        {
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new { Message = "Username and Password are required." });
            }

            var user = await _context.Set<User>()
                                     .FirstOrDefaultAsync(u => u.Username.ToLower() == loginDto.Username.ToLower());

            if (user == null)
            {
                return Ok(new { IsSuccess = false, ErrorMessage = "Invalid credentials." });

            }

            bool isPasswordValid = VerifyPasswordHash(loginDto.Password, user.Password); // Apel metodă locală/helper

            if (!isPasswordValid)
            {
                return Ok(new { IsSuccess = false, ErrorMessage = "Invalid credentials." });

            }

            var response = new
            {
                IsSuccess = true,
                Role = user.UserRole,
                ErrorMessage = (string?)null
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            Console.WriteLine($"Logout requested by client (Token in header: {Request.Headers["X-Auth-Token"].FirstOrDefault()})");
            return Ok(new { Message = "Logout requested by client." });
        }

        private bool VerifyPasswordHash(string providedPassword, string storedPasswordHash)
        {
           
            Console.WriteLine($"WARNING: Using INSECURE plaintext password comparison in AuthController!");
            return providedPassword == storedPasswordHash;
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Data;
using Backend.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Doar pt [AllowAnonymous]
using Microsoft.EntityFrameworkCore;
using System; // Pt Guid

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                return BadRequest(new LoginResponseDto { IsSuccess = false, ErrorMessage = "Username and Password are required." });
            }

            var user = await _context.Set<User>()
                                     .FirstOrDefaultAsync(u => u.Username.ToLower() == loginDto.Username.ToLower());

            // !!! COMPARAȚIE NESIGURĂ ÎN TEXT SIMPLU !!!
            if (user == null || user.Password != loginDto.Password) // Compară parolele direct (NESIGUR!)
            {
                // Returnează Unauthorized, dar cu un corp specific pt frontend
                return Unauthorized(new LoginResponseDto { IsSuccess = false, ErrorMessage = "Invalid credentials." });
            }
            // !!! ------------------------------------ !!!


            // Generează un "token" fals doar ca să existe ceva
            var fakeToken = Guid.NewGuid().ToString("N");

            var response = new LoginResponseDto
            {
                IsSuccess = true,
                Token = fakeToken, // Trimite token-ul fals
                UserId = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.UserRole // Trimite rolul
            };

            return Ok(response); // Returnează 200 OK cu datele
        }


        // Poți elimina sau lăsa acest endpoint, nu are funcționalitate reală acum
        [HttpPost("logout")]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            Console.WriteLine("Logout endpoint called (no action taken).");
            return Ok(new { Message = "Logout called." });
        }

        // Eliminăm metoda de verificare hash
    }
}
using Backend.Models;

namespace Backend.DTOs
{
    public class LoginResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserRole Role { get; set; }
    }
}

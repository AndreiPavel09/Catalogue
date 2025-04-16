using Backend.Models;

namespace Backend.DTOs
{
    public class LoginResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}

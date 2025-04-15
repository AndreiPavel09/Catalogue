// FrontendAdmin/DTOs/UserDtos.cs
using System.ComponentModel.DataAnnotations;

namespace Frontend.DTOs // Ensure this namespace is correct
{
    // DTO for user creation (Matches Backend.DTOs.CreateUserDto)
    public class CreateUserDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Username is too long (50 char max).")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "First name is too long (100 char max).")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Last name is too long (100 char max).")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // "Student" or "Teacher"
    }

    // DTO for user response (Matches Backend.DTOs.UserDto)
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
    }
    // Add UpdateUserDto if needed later
}
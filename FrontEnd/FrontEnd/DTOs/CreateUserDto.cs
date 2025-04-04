using System.ComponentModel.DataAnnotations;

namespace FrontEnd.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = string.Empty; // Add password confirmation in UI if needed

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required] // This will be set programmatically based on the section (Teacher/Student)
        public string Role { get; set; } = string.Empty;
    }
}

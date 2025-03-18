using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    // DTO for user creation
    public class CreateUserDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Student" or "Teacher"
    }

    // DTO for user response (excludes password)
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }

    // DTO for user update
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
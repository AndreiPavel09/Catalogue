using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    // Base User class that will be inherited by Student and Teacher
    [Table("Users")]
    public abstract class User
    {
        [Key]
        public int Id { get; set; }

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
        public string Password { get; set; }

        [Required]
        public UserRole UserRole { get; set; }

        // Full name property for convenience
        public string FullName => $"{FirstName} {LastName}";
    }


    // Enum for user roles
    public enum UserRole
    {
        Student,
        Teacher,
        Admin
    }
}
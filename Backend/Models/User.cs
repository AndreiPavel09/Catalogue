using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    // Base User class that will be inherited by Student and Teacher
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
        public UserRole Role { get; set; }

        // Full name property for convenience
        public string FullName => $"{FirstName} {LastName}";
    }

    // Student class inheriting from User
    public class Student : User
    {
        public Student()
        {
            Role = UserRole.Student;
        }

        // Additional student-specific properties can be added here
    }

    //Teacher class inheriting from User
    public class Teacher : User
    {
        public Teacher()
        {
            Role = UserRole.Teacher;
        }

        // Additional teacher-specific properties can be added here
    }

    // Enum for user roles
    public enum UserRole
    {
        Student,
        Teacher,
        Admin
    }
}
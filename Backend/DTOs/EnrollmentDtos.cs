// Backend/DTOs/EnrollmentDto.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class EnrollmentDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }
    }

    // Optional: DTO for displaying enrollments
    public class EnrollmentDetailsDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
        // Add other details if needed
    }
}
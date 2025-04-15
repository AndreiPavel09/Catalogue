// FrontendAdmin/DTOs/GradeDtos.cs
using System.ComponentModel.DataAnnotations;

namespace Frontend.DTOs // Or Frontend.DTOs if that's your project's namespace
{
    public class GradeDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        [Range(typeof(decimal), "0.0", "10.0", ErrorMessage = "Grade must be between 0 and 10.")]
        public decimal Value { get; set; }

        // Optional: Add Student and Course names for display
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
    }

    public class CreateGradeDto
    {
        [Required(ErrorMessage = "Student is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Student ID.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Course is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Course ID.")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Grade Value is required.")]
        [Range(typeof(decimal), "0.0", "10.0", ErrorMessage = "Grade must be between 0 and 10.")]
        public decimal Value { get; set; }
    }

    public class UpdateGradeDto
    {
        [Required(ErrorMessage = "Grade Value is required.")]
        [Range(typeof(decimal), "0.0", "10.0", ErrorMessage = "Grade must be between 0 and 10.")]
        public decimal? Value { get; set; } // Keep nullable to match backend PUT body expectation
    }
}
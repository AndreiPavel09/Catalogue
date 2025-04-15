// FrontendAdmin/DTOs/CourseDtos.cs
using System.ComponentModel.DataAnnotations;

namespace Frontend.DTOs // Or Frontend.DTOs if that's your project's namespace
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        // Optional: Add Teacher Name if you plan to display it easily
        // public string? TeacherName { get; set; } // You'd need to populate this in the service/backend
    }

    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Course Name is required.")]
        [StringLength(100, ErrorMessage = "Course Name cannot exceed 100 characters.")]
        public string CourseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Teacher must be selected.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Teacher ID.")] // Basic validation
        public int TeacherId { get; set; }
    }

    // UpdateCourseDto might not be directly used by AdminController but could be useful later
    // public class UpdateCourseDto
    // {
    //     [Required]
    //     [StringLength(100)]
    //     public string CourseName { get; set; } = string.Empty;
    //
    //     [Required]
    //     public int TeacherId { get; set; }
    // }
}
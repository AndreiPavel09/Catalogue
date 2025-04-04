using System.ComponentModel.DataAnnotations;

namespace FrontEnd.DTOs
{
    public class CreateCourseDto
    {
        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid teacher.")]
        public int TeacherId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FrontEnd.DTOs
{
    public class CreateGradeDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid student.")]
        public int StudentId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid course.")]
        public int CourseId { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Grade value must be between 0 and 100.")] // Adjust range if needed
        public decimal Value { get; set; }
    }
}

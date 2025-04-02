namespace Backend.DTOs
{
    using System.ComponentModel.DataAnnotations; // For validation attributes

    public class AssignGradeDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [Range(0, 10)] // Example range, adjust as needed
        public decimal Value { get; set; }
    }
}

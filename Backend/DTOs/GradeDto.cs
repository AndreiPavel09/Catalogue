using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class GradeDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Grade Value is required for display/edit.")] // Keep validation if binding directly
        [Range(typeof(decimal), "0.0", "10.0", ErrorMessage = "Grade must be between 0 and 10.")]
        public decimal Value { get; set; } // Make non-nullable if always expected for edit form

        // --- ADD THESE TWO LINES ---
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
    }

    public class CreateGradeDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "10")]

        public decimal Value { get; set; }

    }

    public class UpdateGradeDto
    {
        [Required(ErrorMessage = "Grade Value is required.")] // Make required for update
        [Range(typeof(decimal), "0.0", "10.0", ErrorMessage = "Grade must be between 0 and 10.")]
        public decimal? Value { get; set; } // Keep nullable for flexibility, but make Required for validation
    }
}
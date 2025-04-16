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

    public class BulkCreateGradesRequestDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CourseId { get; set; } // All grades in this request belong to this course

        [Required]
        [MinLength(1, ErrorMessage = "At least one grade entry is required.")]
        public List<StudentGradeEntryDto> GradeEntries { get; set; } = new List<StudentGradeEntryDto>();
    }

    // Represents a single student's grade within the bulk request
    public class StudentGradeEntryDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Grade Value is required.")]
        [Range(typeof(decimal), "0.0", "10.0", ErrorMessage = "Grade must be between 0 and 10.")]
        public decimal Value { get; set; }
        // Add optional fields like AssignmentName, Date if needed later
    }

    // Optional: DTO for the response (e.g., list of created grades or summary)
    public class BulkCreateGradesResponseDto
    {
        public int CourseId { get; set; }
        public int GradesSuccessfullyAdded { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        // public List<GradeDto> AddedGrades { get; set; } // Optionally return created grades
    }
}
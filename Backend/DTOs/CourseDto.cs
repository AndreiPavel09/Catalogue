using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public required string CourseName { get; set; }
        public int TeacherId { get; set; }
    }

    public class CreateCourseDto
    {
        [Required]
        [StringLength(100)]
        public required string CourseName { get; set; }

        [Required]
        public int TeacherId { get; set; }
    }

    public class UpdateCourseDto
    {
        [Required]
        [StringLength(100)]
        public required string CourseName { get; set; }

        [Required]
        public int TeacherId { get; set; }
    }
}
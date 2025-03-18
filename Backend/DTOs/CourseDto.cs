using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    // Course DTOs
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TeacherId { get; set; }
    }

    public class CreateCourseDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public int TeacherId { get; set; }
    }

    public class UpdateCourseDto
    {
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int? TeacherId { get; set; }
    }
}
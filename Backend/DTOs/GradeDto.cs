using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class GradeDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        
        [Range(typeof(decimal), "0", "10")]

        public decimal Value { get; set; }
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
        [Range(typeof(decimal), "0", "10")]
        public decimal? Value { get; set; }

    }
}
using Backend.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Value { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Navigation properties
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
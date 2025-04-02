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

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [Required]
        [Range(0,10)]
        public decimal Value { get; set; }
        
    }
}
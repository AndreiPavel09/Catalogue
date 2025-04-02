using Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]   
        public virtual Teacher? Teacher { get; set; }

        public virtual ICollection<StudentCourse> StudentCourses { get; set; }= new List<StudentCourse>();
   
    }
}
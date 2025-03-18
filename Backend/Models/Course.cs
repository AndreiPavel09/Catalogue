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
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }

        // Navigation property for students enrolled in the course
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }

        // Navigation property for grades in the course
        public virtual ICollection<Grade> Grades { get; set; }
    }

    // Join entity for many-to-many relationship between students and courses
    public class StudentCourse
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public DateTime EnrollmentDate { get; set; }
    }
}
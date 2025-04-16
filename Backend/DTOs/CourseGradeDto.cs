namespace Backend.DTOs
{
    public class CourseGradeDto
    {
        public int CourseId { get; set; }
        public string? CourseName { get; set; }

        public int GradeId { get; set; } 
        public decimal GradeValue { get; set; }
       
    }
}

namespace FrontEnd.DTOs
{
    public class GradeDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public decimal Value { get; set; }
        // Optional: Add StudentName/CourseName if backend returns them
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
    }
}

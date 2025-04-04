namespace FrontEnd.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        // Optional: Add TeacherName if you modify backend to return it for display
        public string? TeacherName { get; set; }
    }
}

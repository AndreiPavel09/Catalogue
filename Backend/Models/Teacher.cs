namespace Backend.Models
{
    public class Teacher : User
    {
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public Teacher()
        {
            UserRole = UserRole.Teacher;
        }

        // Additional teacher-specific properties can be added here
    }
}

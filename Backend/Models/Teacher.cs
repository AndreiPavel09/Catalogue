namespace Backend.Models
{
    public class Teacher : User
    {
        public Teacher()
        {
            Role = UserRole.Teacher;
        }

        // Additional teacher-specific properties can be added here
    }
}

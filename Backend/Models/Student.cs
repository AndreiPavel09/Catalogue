namespace Backend.Models
{
    public class Student:User
    {
        public Student() {

            UserRole = UserRole.Student;
        }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    }
}

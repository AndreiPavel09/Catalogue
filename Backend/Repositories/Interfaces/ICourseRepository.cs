using Backend.Models;

namespace Backend.Repositories.Interfaces
{
        public interface ICourseRepository
        {
            Task<List<Course>> GetAllCoursesAsync();
            Task<Course?> GetCourseByIdAsync(int id);
            Task<Course> CreateCourseAsync(Course course);
            Task UpdateCourseAsync(Course course);
            Task<bool> DeleteCourseAsync(int id);
        }
}

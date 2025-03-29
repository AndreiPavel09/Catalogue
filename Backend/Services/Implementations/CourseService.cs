using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public Task<Course> CreateCourseAsync(Course course)
        {
            return _courseRepository.CreateCourseAsync(course);

        }

        public Task<bool> DeleteCourseAsync(int id)
        {
            return _courseRepository.DeleteCourseAsync(id);
        }

        public Task<List<Course>> GetAllCoursesAsync()
        {
            return _courseRepository.GetAllCoursesAsync();

        }

        public Task<Course?> GetCourseByIdAsync(int id)
        {
            return _courseRepository.GetCourseByIdAsync(id);

        }

        public Task UpdateCourseAsync(Course course)
        {
            return _courseRepository.UpdateCourseAsync(course);
        }
    }
}

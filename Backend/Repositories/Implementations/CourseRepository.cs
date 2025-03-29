using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations
{

    public class CourseRepository : ICourseRepository
        {
            private readonly ApplicationDbContext _context;

            public CourseRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Course>> GetAllCoursesAsync()
            {
                return await _context.Courses.ToListAsync();
            }

            public async Task<Course?> GetCourseByIdAsync(int id)
            {
                return await _context.Courses.FindAsync(id);
            }

            public async Task<Course> CreateCourseAsync(Course course)
            {
                await _context.Courses.AddAsync(course);
                await _context.SaveChangesAsync();
                return course;
            }

            public async Task UpdateCourseAsync(Course course)
            {
                _context.Entry(course).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            public async Task<bool> DeleteCourseAsync(int id)
            {
                var course = await GetCourseByIdAsync(id);
                if (course != null)
                {
                    _context.Courses.Remove(course);
                    await _context.SaveChangesAsync();
                    return true;
                }
            
                return false;
            }
        }
}

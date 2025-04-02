using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;

        public TeacherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TeacherExistsAsync(int teacherId)
        {
            // Check specifically for a Teacher user type
            return await _context.Teachers.AnyAsync(t => t.Id == teacherId);
        }

        // --- Private helper to verify teacher teaches the course ---
        private async Task<Course> ValidateTeacherTeachesCourseAsync(int teacherId, int courseId)
        {
            var course = await _context.Courses
                .AsNoTracking() // Read-only check
                .FirstOrDefaultAsync(c => c.Id == courseId && c.TeacherId == teacherId);

            if (course == null)
            {
                // Throw a specific exception that the controller can catch
                // Option 1: KeyNotFoundException (general purpose "not found")
                // throw new KeyNotFoundException($"Course with ID {courseId} not found or not taught by teacher {teacherId}.");

                // Option 2: UnauthorizedAccessException (semantically indicates permission issue)
                throw new UnauthorizedAccessException($"Teacher {teacherId} is not authorized to manage course {courseId}.");
            }
            return course; // Return the validated course (without tracking)
        }

        public async Task<IEnumerable<CourseDto>> GetTeacherCoursesAsync(int teacherId)
        {
            // Optional: Check if teacher exists first
            if (!await TeacherExistsAsync(teacherId))
            {
                throw new KeyNotFoundException($"Teacher with ID {teacherId} not found.");
            }

            return await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    CourseName = c.CourseName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsInCourseAsync(int teacherId, int courseId)
        {
            // 1. Validate Teacher teaches this course (throws if not authorized)
            await ValidateTeacherTeachesCourseAsync(teacherId, courseId);

            // 2. Get enrolled students
            return await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Include(sc => sc.Student)
                .Select(sc => new StudentDto
                {
                    Id = sc.Student.Id,
                    Username = sc.Student.Username,
                    FirstName = sc.Student.FirstName,
                    LastName = sc.Student.LastName
                })
                .ToListAsync();
        }

        public async Task<GradeDto> AssignOrUpdateGradeAsync(int teacherId, AssignGradeDto assignGradeDto)
        {
            // 1. Validate Teacher teaches this course (throws if not authorized)
            var course = await ValidateTeacherTeachesCourseAsync(teacherId, assignGradeDto.CourseId);

            // 2. Validate student exists and is enrolled in this *specific* course
            var studentEnrollment = await _context.StudentCourses
                .Include(sc => sc.Student) // Need student info for the return DTO
                .AsNoTracking()
                .FirstOrDefaultAsync(sc => sc.StudentId == assignGradeDto.StudentId && sc.CourseId == assignGradeDto.CourseId);

            if (studentEnrollment == null)
            {
                // Use ArgumentException for invalid input data related to the DTO
                throw new ArgumentException($"Student with ID {assignGradeDto.StudentId} is not enrolled in course {assignGradeDto.CourseId}.");
            }

            // 3. Find existing grade or create new
            var existingGrade = await _context.Grades
                .FirstOrDefaultAsync(g => g.StudentId == assignGradeDto.StudentId && g.CourseId == assignGradeDto.CourseId);

            Grade gradeToProcess;

            if (existingGrade != null)
            {
                // Update
                existingGrade.Value = assignGradeDto.Value;
                _context.Grades.Update(existingGrade);
                gradeToProcess = existingGrade;
            }
            else
            {
                // Create
                var newGrade = new Grade
                {
                    StudentId = assignGradeDto.StudentId,
                    CourseId = assignGradeDto.CourseId,
                    Value = assignGradeDto.Value
                };
                _context.Grades.Add(newGrade);
                gradeToProcess = newGrade;
            }

            await _context.SaveChangesAsync();

            // 4. Map to DTO for return
            return new GradeDto
            {
                Id = gradeToProcess.Id,
                StudentId = gradeToProcess.StudentId,
                CourseId = gradeToProcess.CourseId,
                Value = gradeToProcess.Value
            };
        }

        public async Task<IEnumerable<GradeDto>> GetGradesForCourseAsync(int teacherId, int courseId)
        {
            // 1. Validate Teacher teaches this course (throws if not authorized)
            var course = await ValidateTeacherTeachesCourseAsync(teacherId, courseId);

            // 2. Get grades for this course
            return await _context.Grades
                .Where(g => g.CourseId == courseId)
                .Include(g => g.Student)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    StudentId = g.StudentId,
                    CourseId = g.CourseId,
                    Value = g.Value
                })
                .ToListAsync();
        }
    }
}

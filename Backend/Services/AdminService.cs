using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;
using Backend.Repositories;
using Backend.DTOs;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public interface IAdminService
    {
        // Teacher management
        Task<UserDto> AddTeacherAsync(CreateUserDto teacherDto);
        Task DeleteTeacherAsync(int teacherId);
        Task<List<UserDto>> ViewTeachersAsync();

        // Course management
        Task<CourseDto> AddCourseAsync(CreateCourseDto courseDto);
        Task DeleteCourseAsync(int courseId);
        Task<List<CourseDto>> ViewCoursesAsync();

        // Student management
        Task<UserDto> AddStudentAsync(CreateUserDto studentDto);
        Task DeleteStudentAsync(int studentId);
        Task<List<UserDto>> ViewStudentsAsync();

        // Grade management
        Task<GradeDto> AddGradeAsync(CreateGradeDto gradeDto);
        Task DeleteGradeAsync(int gradeId);
        Task<List<GradeDto>> ViewGradesAsync();
    }

    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IGradeRepository _gradeRepository;
        private IUserRepository object1;
        private ICourseRepository object2;
        private IGradeRepository object3;
        private readonly ApplicationDbContext _context;

        public AdminService(
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            IGradeRepository gradeRepository,
            ApplicationDbContext context)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Teacher management
        public async Task<UserDto> AddTeacherAsync(CreateUserDto teacherDto)
        {
            // Validate role
            if (!teacherDto.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Invalid role for teacher creation");

            // Check if username already exists
            bool usernameExists = await _userRepository.UsernameExistsAsync(teacherDto.Username);
            if (usernameExists)
                throw new InvalidOperationException("Username already exists");

            // Create teacher
            var teacher = new Teacher
            {
                Username = teacherDto.Username,
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                Password = teacherDto.Password // In a real app, you would hash this
            };

            // Save to database
            await _userRepository.CreateUserAsync(teacher);

            // Return DTO
            return new UserDto
            {
                Id = teacher.Id,
                Username = teacher.Username,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Role = teacher.UserRole.ToString()
            };
        }

        public async Task DeleteTeacherAsync(int teacherId)
        {
            // 1. Find the user and verify they are a teacher
            var teacher = await _userRepository.GetUserByIdAsync(teacherId);
            if (teacher == null || teacher.UserRole != UserRole.Teacher)
            {
                // Use a specific exception or rely on controller to return NotFound
                throw new InvalidOperationException("Teacher not found."); // Or return false/null if preferred pattern
            }

            // --- CHECK FOR ASSIGNED COURSES ---
            // Get all courses (consider optimizing this if you have thousands of courses,
            // e.g., by adding a specific repository method like HasCourses(teacherId))
            var courses = await _courseRepository.GetAllCoursesAsync();
            bool hasCourses = courses.Any(c => c.TeacherId == teacherId);

            if (hasCourses)
            {
                // 2. If teacher has courses, throw a specific error message
                // This message will be shown to the admin user.
                throw new InvalidOperationException($"Cannot delete teacher '{teacher.FullName}' (ID: {teacherId}) because they are assigned to one or more courses. Please reassign or delete their courses first.");
            }
            // --- END CHECK ---

            // 3. If no courses are assigned, proceed with deleting the user
            await _userRepository.DeleteUserAsync(teacherId);
        }

        public async Task<List<UserDto>> ViewTeachersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var teacherDtos = new List<UserDto>();

            foreach (var user in users)
            {
                if (user.UserRole == UserRole.Teacher)
                {
                    teacherDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.UserRole.ToString()
                    });
                }
            }

            return teacherDtos;
        }

        // Course management
        public async Task<CourseDto> AddCourseAsync(CreateCourseDto courseDto)
        {
            // Verify the teacher exists
            var teacher = await _userRepository.GetUserByIdAsync(courseDto.TeacherId);
            if (teacher == null || teacher.UserRole != UserRole.Teacher)
                throw new InvalidOperationException("Teacher not found");

            // Create course
            var course = new Course
            {
                CourseName = courseDto.CourseName,
                TeacherId = courseDto.TeacherId
            };

            // Save to database
            await _courseRepository.CreateCourseAsync(course);

            // Return DTO
            return new CourseDto
            {
                Id = course.Id,
                CourseName = course.CourseName,
                TeacherId = course.TeacherId
            };
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
                throw new InvalidOperationException("Course not found");

            await _courseRepository.DeleteCourseAsync(courseId);
        }

        public async Task<List<CourseDto>> ViewCoursesAsync()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            var courseDtos = new List<CourseDto>();

            foreach (var course in courses)
            {
                courseDtos.Add(new CourseDto
                {
                    Id = course.Id,
                    CourseName = course.CourseName,
                    TeacherId = course.TeacherId
                });
            }

            return courseDtos;
        }

        // Student management
        public async Task<UserDto> AddStudentAsync(CreateUserDto studentDto)
        {
            // Validate role
            if (!studentDto.Role.Equals("Student", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Invalid role for student creation");

            // Check if username already exists
            bool usernameExists = await _userRepository.UsernameExistsAsync(studentDto.Username);
            if (usernameExists)
                throw new InvalidOperationException("Username already exists");

            // Create student
            var student = new Student
            {
                Username = studentDto.Username,
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Password = studentDto.Password // In a real app, you would hash this
            };

            // Save to database
            await _userRepository.CreateUserAsync(student);

            // Return DTO
            return new UserDto
            {
                Id = student.Id,
                Username = student.Username,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Role = student.UserRole.ToString()
            };
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            // 1. Find the user and verify they are a student
            var student = await _userRepository.GetUserByIdAsync(studentId);
            if (student == null || student.UserRole != UserRole.Student)
            {
                throw new InvalidOperationException("Student not found.");
            }

            // --- CHECK FOR DEPENDENCIES ---

            // Check for existing grades
            bool hasGrades = await _gradeRepository.HasGradesForStudentAsync(studentId);
            if (hasGrades)
            {
                throw new InvalidOperationException($"Cannot delete student '{student.FullName}' (ID: {studentId}) because they have existing grades. Please delete their grades first.");
            }

            // Check for existing course enrollments (StudentCourses)
            bool isEnrolled = await _context.StudentCourses.AnyAsync(sc => sc.StudentId == studentId);
            if (isEnrolled)
            {
                throw new InvalidOperationException($"Cannot delete student '{student.FullName}' (ID: {studentId}) because they are enrolled in one or more courses. Please remove them from courses first.");
                // Note: You might need a way to *unenroll* students rather than deleting StudentCourse records directly depending on your logic.
                // For now, we just prevent the student delete.
            }

            // --- END CHECK ---

            // 3. If no dependencies, proceed with deleting the user
            await _userRepository.DeleteUserAsync(studentId);
        }

        public async Task<List<UserDto>> ViewStudentsAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var studentDtos = new List<UserDto>();

            foreach (var user in users)
            {
                if (user.UserRole == UserRole.Student)
                {
                    studentDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.UserRole.ToString()
                    });
                }
            }

            return studentDtos;
        }

        // Grade management
        public async Task<GradeDto> AddGradeAsync(CreateGradeDto gradeDto)
        {
            var grade = new Grade
            {
                StudentId = gradeDto.StudentId,
                CourseId = gradeDto.CourseId,
                Value = gradeDto.Value,
            };

            var createdGrade = await _gradeRepository.CreateGradeAsync(grade);

            var resultDto = new GradeDto
            {
                Id = createdGrade.Id,
                StudentId = createdGrade.StudentId,
                CourseId = createdGrade.CourseId,
                Value = createdGrade.Value,
            };

            return resultDto;
        }

        public async Task DeleteGradeAsync(int gradeId)
        {
           await _gradeRepository.DeleteGradeAsync(gradeId);
        }

        public async Task<List<GradeDto>> ViewGradesAsync()
        {
            var grades = await _gradeRepository.GetAllGradesAsync();
            var gradeDtos = new List<GradeDto>();

            foreach (var grade in grades)
            {
                gradeDtos.Add(new GradeDto
                {
                    Id = grade.Id,
                    StudentId = grade.StudentId,
                    CourseId = grade.CourseId,
                    Value = grade.Value,
                });
            }

            return gradeDtos;
        }
    }
}
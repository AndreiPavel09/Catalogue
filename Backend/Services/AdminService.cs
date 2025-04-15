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
        Task UpdateGradeAsync(int gradeId, UpdateGradeDto gradeDto);

        Task EnrollStudentAsync(EnrollmentDto enrollmentDto);
        Task UnenrollStudentAsync(int studentId, int courseId);
        Task<List<EnrollmentDetailsDto>> GetEnrollmentsAsync(); // To view existing enrollments
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

        public async Task UpdateGradeAsync(int gradeId, UpdateGradeDto gradeDto)
        {
            // 1. Validate DTO value (optional here, but good practice)
            if (!gradeDto.Value.HasValue)
            {
                throw new ArgumentException("Grade value is required for update.", nameof(gradeDto.Value));
            }

            // 2. Get the existing grade
            // Use the repository method that includes related data if needed later,
            // but for just updating value, FindAsync might suffice if UpdateGradeAsync handles it.
            // Let's assume GetGradeByIdAsync fetches the entity for update.
            var grade = await _gradeRepository.GetGradeByIdAsync(gradeId);

            // 3. Validate: Must exist
            if (grade == null)
            {
                throw new InvalidOperationException("Grade not found.");
            }

            // 4. Update the value if it has changed
            if (grade.Value != gradeDto.Value.Value) // Compare with the non-nullable value
            {
                grade.Value = gradeDto.Value.Value; // Assign the non-nullable value
                                                    // 5. Call repository update
                                                    // Ensure GradeRepository.UpdateGradeAsync marks the entity as modified and saves changes.
                await _gradeRepository.UpdateGradeAsync(grade);
            }
            // else: No change needed if value is the same
        }

        public async Task EnrollStudentAsync(EnrollmentDto enrollmentDto)
        {
            // 1. Validate Student exists and is a Student
            var student = await _userRepository.GetUserByIdAsync(enrollmentDto.StudentId);
            if (student == null || student.UserRole != UserRole.Student)
                throw new InvalidOperationException("Student not found or invalid user type.");

            // 2. Validate Course exists
            var course = await _courseRepository.GetCourseByIdAsync(enrollmentDto.CourseId);
            if (course == null)
                throw new InvalidOperationException("Course not found.");

            // 3. Check if already enrolled
            bool alreadyEnrolled = await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == enrollmentDto.StudentId && sc.CourseId == enrollmentDto.CourseId);
            if (alreadyEnrolled)
                throw new InvalidOperationException($"Student '{student.FullName}' is already enrolled in course '{course.CourseName}'.");

            // 4. Create and save enrollment
            var enrollment = new StudentCourse
            {
                StudentId = enrollmentDto.StudentId,
                CourseId = enrollmentDto.CourseId
            };
            _context.StudentCourses.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task UnenrollStudentAsync(int studentId, int courseId)
        {
            // 1. Find the enrollment record
            var enrollment = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            // 2. Validate it exists
            if (enrollment == null)
                throw new InvalidOperationException("Enrollment record not found.");

            // 3. Remove and save
            _context.StudentCourses.Remove(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EnrollmentDetailsDto>> GetEnrollmentsAsync()
        {
            // 1. Fetch the raw StudentCourse entities including related data from the DB
            var studentCoursesFromDb = await _context.StudentCourses
                .Include(sc => sc.Student) // Eager load Student (User) data
                .Include(sc => sc.Course)  // Eager load Course data
                                           // Optional: You could apply simple filtering here if needed (e.g., .Where(sc => sc.Course.IsActive))
                .ToListAsync(); // Execute the database query

            // 2. Project the results into DTOs in memory using LINQ to Objects
            var enrollmentDtos = studentCoursesFromDb
                .Select(sc => new EnrollmentDetailsDto // This Select now operates on the in-memory list
                {
                    StudentId = sc.StudentId,
                    CourseId = sc.CourseId,
                    StudentName = sc.Student?.FullName ?? "N/A", // Safely access potentially null Student
                    CourseName = sc.Course?.CourseName ?? "N/A"   // Safely access potentially null Course
                })
                .OrderBy(dto => dto.CourseName).ThenBy(dto => dto.StudentName) // Ordering happens in memory
                .ToList(); // Final list of DTOs

            return enrollmentDtos;
        }
    }

}
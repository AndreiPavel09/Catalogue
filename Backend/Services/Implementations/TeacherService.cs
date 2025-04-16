using Backend.Data;
using Backend.DTOs;
using Backend.DTOs.Backend.DTOs;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGradeRepository _gradeRepository;
        private readonly IStudentRepository _studentRepository;

        public TeacherService(ApplicationDbContext context, IGradeRepository gradeRepository, IStudentRepository studentRepository)
        {
            _context = context;
            _gradeRepository = gradeRepository;
            _studentRepository = studentRepository;
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

        public async Task<IEnumerable<StudentDTO>> GetStudentsInCourseAsync(int teacherId, int courseId)
        {
            // 1. Validate Teacher teaches this course (throws if not authorized)
            await ValidateTeacherTeachesCourseAsync(teacherId, courseId);

            // 2. Get enrolled students
            return await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Include(sc => sc.Student)
                .Select(sc => new StudentDTO
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
                await _gradeRepository.UpdateGradeAsync(existingGrade);
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
                await _gradeRepository.CreateGradeAsync(newGrade);
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

        public async Task AddStudentToCourseAsync(int teacherId, int studentId, int courseId)
        {
            // 1. Validate Teacher Teaches this Course (throws if not authorized or course not found)
            await ValidateTeacherTeachesCourseAsync(teacherId, courseId);

            // 2. Validate Student Exists and is a Student
            var student = await _studentRepository.GetStudentByIdAsync(studentId);
            if (student == null) // Check role
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found or is not a valid student.");
            }

            // 3. Check if already enrolled
            bool alreadyEnrolled = await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);
            if (alreadyEnrolled)
            {
                // Using ArgumentException as it relates to input validity for the operation
                throw new ArgumentException($"Student (ID: {studentId}) is already enrolled in this course (ID: {courseId}).");
            }

            // 4. Create and save enrollment
            var enrollment = new StudentCourse { StudentId = studentId, CourseId = courseId };
            _context.StudentCourses.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveStudentFromCourseAsync(int teacherId, int studentId, int courseId)
        {
            // 1. Validate Teacher Teaches this Course (throws if not authorized or course not found)
            await ValidateTeacherTeachesCourseAsync(teacherId, courseId);

            // 2. Find the enrollment record
            // No need to validate student exists separately here, finding the record implies they were enrolled.
            var enrollment = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            // 3. Validate enrollment exists
            if (enrollment == null)
            {
                // Use KeyNotFoundException as the specific enrollment link is missing
                throw new KeyNotFoundException($"Student (ID: {studentId}) is not currently enrolled in this course (ID: {courseId}).");
            }

            // 4. Remove and save
            _context.StudentCourses.Remove(enrollment);
            await _context.SaveChangesAsync();

            // Optional: Consider deleting student's grades for this course upon unenrollment?
            // This depends on requirements. Be careful with data deletion.
            // var gradesToDelete = await _context.Grades
            //     .Where(g => g.StudentId == studentId && g.CourseId == courseId)
            //     .ToListAsync();
            // if (gradesToDelete.Any()) {
            //     _context.Grades.RemoveRange(gradesToDelete);
            //     await _context.SaveChangesAsync();
            // }
        }

        public async Task<BulkCreateGradesResponseDto> AddGradesBulkAsync(int teacherId, BulkCreateGradesRequestDto request)
        {
            var response = new BulkCreateGradesResponseDto { CourseId = request.CourseId };

            // 1. Validate Teacher Teaches this Course
            try
            {
                await ValidateTeacherTeachesCourseAsync(teacherId, request.CourseId);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is KeyNotFoundException)
            {
                response.Errors.Add($"Authorization failed: {ex.Message}");
                return response; // Stop processing if teacher can't access course
            }

            // 2. Get list of students actually enrolled in THIS course (for validation)
            //    Use AsNoTracking for read-only check. Fetch only IDs for efficiency.
            var enrolledStudentIds = await _context.StudentCourses
                                             .Where(sc => sc.CourseId == request.CourseId)
                                             .Select(sc => sc.StudentId)
                                             .ToHashSetAsync(); // Efficient lookup

            var gradesToAdd = new List<Grade>();

            // 3. Process each entry in the request
            foreach (var entry in request.GradeEntries)
            {
                // a. Validate student ID exists (using UserManager for consistency)
                var studentExists = await _studentRepository.GetStudentByIdAsync(entry.StudentId) != null;
                if (!studentExists)
                {
                    response.Errors.Add($"Student ID {entry.StudentId}: Does not exist.");
                    continue; // Skip this entry
                }

                // b. Validate student is enrolled in THIS course
                if (!enrolledStudentIds.Contains(entry.StudentId))
                {
                    response.Errors.Add($"Student ID {entry.StudentId}: Not enrolled in course ID {request.CourseId}.");
                    continue; // Skip this entry
                }

                // c. Validate grade value (already done by DTO attributes, but can double-check)
                if (entry.Value < 0 || entry.Value > 10)
                {
                    response.Errors.Add($"Student ID {entry.StudentId}: Invalid grade value {entry.Value}. Must be 0-10.");
                    continue; // Skip this entry
                }

                // d. Optional: Check if grade already exists for this student/course
                //    Decide whether to update or skip/error if exists. Let's assume we ADD (error if exists).
                bool gradeExists = await _context.Grades
                                            .AnyAsync(g => g.StudentId == entry.StudentId && g.CourseId == request.CourseId);
                if (gradeExists)
                {
                    response.Errors.Add($"Student ID {entry.StudentId}: Already has a grade for this course. Use update function instead.");
                    continue; // Skip this entry
                }

                // e. If all checks pass, create Grade entity
                gradesToAdd.Add(new Grade
                {
                    StudentId = entry.StudentId,
                    CourseId = request.CourseId,
                    Value = entry.Value
                    // Set other properties like DateEntered if applicable
                });
            }

            // 4. Add valid grades to the context and save
            if (gradesToAdd.Any())
            {
                try
                {
                    _context.Grades.AddRange(gradesToAdd); // Efficiently add multiple entities
                    await _context.SaveChangesAsync();
                    response.GradesSuccessfullyAdded = gradesToAdd.Count;
                    // Optionally populate response.AddedGrades here if needed
                }
                catch (Exception ex)
                {
                    // Handle potential DbUpdateException, log details
                    Console.WriteLine($"Error saving bulk grades: {ex}");
                    response.Errors.Add($"An error occurred while saving the grades: {ex.Message}");
                    // Indicate potentially partial success if needed
                    response.GradesSuccessfullyAdded = 0; // Or track which ones failed before AddRange
                }
            }

            return response;
        }
    }
}

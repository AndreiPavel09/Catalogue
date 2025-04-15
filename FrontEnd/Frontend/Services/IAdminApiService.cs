// FrontendAdmin/Services/IAdminApiService.cs
using Frontend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontend.Services // Ensure namespace is correct
{
    public interface IAdminApiService
    {
        Task<List<UserDto>?> GetTeachersAsync();
        Task<UserDto?> AddTeacherAsync(CreateUserDto teacherDto);
        Task<(bool Success, string? ErrorMessage)> DeleteTeacherAsync(int teacherId);

        Task<List<CourseDto>?> GetCoursesAsync();
        Task<CourseDto?> AddCourseAsync(CreateCourseDto courseDto);
        Task<(bool Success, string? ErrorMessage)> DeleteCourseAsync(int courseId);

        Task<List<UserDto>?> GetStudentsAsync();
        Task<UserDto?> AddStudentAsync(CreateUserDto studentDto);
        Task<(bool Success, string? ErrorMessage)> DeleteStudentAsync(int studentId);

        Task<List<GradeDto>?> GetGradesAsync();
        Task<GradeDto?> AddGradeAsync(CreateGradeDto gradeDto);
        Task<(bool Success, string? ErrorMessage)> DeleteGradeAsync(int gradeId);
    }
}
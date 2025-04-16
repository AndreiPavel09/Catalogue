using Frontend.DTOs;

namespace Frontend.Services
{
    public interface ITeacherApiService
    {
        Task<List<CourseDto>?> GetMyCoursesAsync();
        Task<List<UserDto>?> GetStudentsInCourseAsync(int courseId); // Assuming UserDto for students
        Task<List<GradeDto>?> GetGradesForCourseAsync(int courseId);
        // Return tuple indicating success, the resulting grade, and potential error message
        Task<(bool Success, GradeDto? ResultGrade, string? ErrorMessage)> AssignOrUpdateGradeAsync(AssignGradeDto gradeDto);

        Task<(bool Success, string? ErrorMessage)> AddStudentToCourseAsync(int courseId, int studentId);
        Task<(bool Success, string? ErrorMessage)> RemoveStudentFromCourseAsync(int courseId, int studentId);
        Task<List<UserDto>?> GetAllStudentsAsync();

        Task<BulkCreateGradesResponseDto?> AddGradesBulkAsync(int courseId, List<StudentGradeEntryDto> gradeEntries);
    }
}

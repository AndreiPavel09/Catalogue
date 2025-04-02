using Backend.DTOs;

namespace Backend.Services.Interfaces
{
    public interface ITeacherService
    {
        // Gets courses specifically taught by the given teacherId
        Task<IEnumerable<CourseDto>> GetTeacherCoursesAsync(int teacherId);

        // Gets students enrolled in a specific course, *if* taught by the given teacherId
        Task<IEnumerable<StudentDto>> GetStudentsInCourseAsync(int teacherId, int courseId);

        // Assigns or updates a grade for a student in a course, *if* taught by the given teacherId
        Task<GradeDto> AssignOrUpdateGradeAsync(int teacherId, AssignGradeDto gradeDto);

        // Gets all grades for a specific course, *if* taught by the given teacherId
        Task<IEnumerable<GradeDto>> GetGradesForCourseAsync(int teacherId, int courseId);

        // Optional: Helper to check if a teacher exists (might be useful)
        Task<bool> TeacherExistsAsync(int teacherId);
    }
}

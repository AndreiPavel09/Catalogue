using Backend.DTOs;
using Backend.Models;

namespace Backend.Services.Interfaces
{
    public interface IGradeService
    {
        Task<Grade> CreateGradeAsync(Grade grade);
        Task<Grade> GetGradeByIdAsync(int id);
        Task<IEnumerable<Grade>> GetAllGradesAsync();
        Task<Grade> UpdateGradeAsync(Grade grade);
        Task<bool> DeleteGradeAsync(int id);
        Task<IEnumerable<CourseGradeDto>> GetCourseGradesForStudentAsync(int studentId);
        Task<decimal?> CalculateAverageGradeForStudentAsync(int studentId);
    }
}

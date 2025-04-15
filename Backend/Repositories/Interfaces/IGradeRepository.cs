using Backend.Models;

namespace Backend.Repositories.Interfaces
{
    public interface IGradeRepository
    {
      
            Task<List<Grade>> GetAllGradesAsync();
            Task<Grade?> GetGradeByIdAsync(int id);
            Task<List<Grade>> GetGradesByStudentIdAsync(int studentId);
            Task<List<Grade>> GetGradesByCourseIdAsync(int courseId);
            Task<Grade> CreateGradeAsync(Grade grade);
            Task UpdateGradeAsync(Grade grade);
            Task<bool> DeleteGradeAsync(int id);
            Task<bool> HasGradesForStudentAsync(int studentId);
    }
}

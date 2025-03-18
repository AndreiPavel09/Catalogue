using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public interface IGradeRepository
    {
        Task<List<Grade>> GetAllGradesAsync();
        Task<Grade> GetGradeByIdAsync(int id);
        Task<List<Grade>> GetGradesByStudentIdAsync(int studentId);
        Task<List<Grade>> GetGradesByCourseIdAsync(int courseId);
        Task<Grade> CreateGradeAsync(Grade grade);
        Task UpdateGradeAsync(Grade grade);
        Task DeleteGradeAsync(int id);
    }

    public class GradeRepository : IGradeRepository
    {
        private readonly ApplicationDbContext _context;

        public GradeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Grade>> GetAllGradesAsync()
        {
            return await _context.Grades.ToListAsync();
        }

        public async Task<Grade> GetGradeByIdAsync(int id)
        {
            return await _context.Grades.FindAsync(id);
        }

        public async Task<List<Grade>> GetGradesByStudentIdAsync(int studentId)
        {
            return await _context.Grades
                .Where(g => g.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<List<Grade>> GetGradesByCourseIdAsync(int courseId)
        {
            return await _context.Grades
                .Where(g => g.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<Grade> CreateGradeAsync(Grade grade)
        {
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return grade;
        }

        public async Task UpdateGradeAsync(Grade grade)
        {
            _context.Entry(grade).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGradeAsync(int id)
        {
            var grade = await GetGradeByIdAsync(id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();
            }
        }
    }
}
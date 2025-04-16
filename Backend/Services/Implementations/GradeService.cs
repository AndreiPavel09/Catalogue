using Backend.DTOs;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations
{
    
        public class GradeService : IGradeService
        {
            private readonly IGradeRepository _gradeRepository;

            public GradeService(IGradeRepository gradeRepository)
            {
                _gradeRepository = gradeRepository;
            }

            public async Task<Grade> CreateGradeAsync(Grade grade)
            {
            return await _gradeRepository.CreateGradeAsync(grade);
            }

            public async Task<Grade> GetGradeByIdAsync(int id)
            {
            return await _gradeRepository.GetGradeByIdAsync(id);
            }

            public async Task<IEnumerable<Grade>> GetAllGradesAsync()
            {
            return await _gradeRepository.GetAllGradesAsync();
            }

            public async Task<Grade> UpdateGradeAsync(Grade grade)
            {
               await _gradeRepository.UpdateGradeAsync(grade);
                return grade;    
            }

            public async Task<bool> DeleteGradeAsync(int id)
            {
                return await _gradeRepository.DeleteGradeAsync(id);
                    
            }
        public async Task<IEnumerable<CourseGradeDto>> GetCourseGradesForStudentAsync(int studentId)
        {
            return await _gradeRepository.GetCourseGradesForStudentAsync(studentId);
        }
        public async Task<decimal?> CalculateAverageGradeForStudentAsync(int studentId)
        {
            var gradeValues = await _gradeRepository.GetGradeValuesForStudentAsync(studentId);

            if (gradeValues == null || !gradeValues.Any())
            {
                return null; 
            }

            try 
            {
                decimal average = gradeValues.Average();
                return Math.Round(average, 2); 
            }
            catch (InvalidOperationException) 
            {
                return null;
            }
        }
    }


    }

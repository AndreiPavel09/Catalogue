using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations
{
        public class StudentRepository : IStudentRepository
        {
            private readonly ApplicationDbContext _context;

            public StudentRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Student>> GetAllStudentsAsync()
            {
                return await _context.Students.ToListAsync();
            }

            public async Task<Student> GetStudentByIdAsync(int id)
            {
                return await _context.Students.FindAsync(id);
            }

            public async Task<Student> CreateStudentAsync(Student student)
            {
                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();
                return student;
            }

            public async Task UpdateStudentAsync(Student student)
            {
                _context.Entry(student).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            public async Task<bool> DeleteStudentAsync(int id)
            {
                var student = await GetStudentByIdAsync(id);
                if (student == null)
                {
                    return false;
                }
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return true;
            }
        }
}

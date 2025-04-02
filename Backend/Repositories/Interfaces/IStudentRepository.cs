using Backend.Models;

namespace Backend.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student> GetStudentByIdAsync(int id);
        Task<Student> CreateStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(int id);
    }
}

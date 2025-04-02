using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public Task<List<Student>> GetAllStudentsAsync()
        {
            return _studentRepository.GetAllStudentsAsync();
        }

        public Task<Student> GetStudentByIdAsync(int id)
        {
            return _studentRepository.GetStudentByIdAsync(id);
        }

        public Task<Student> CreateStudentAsync(Student student)
        {
            return _studentRepository.CreateStudentAsync(student);
        }

        public Task UpdateStudentAsync(Student student)
        {
            return _studentRepository.UpdateStudentAsync(student);
        }

        public Task<bool> DeleteStudentAsync(int id)
        {
            return _studentRepository.DeleteStudentAsync(id);
        }
    }
}

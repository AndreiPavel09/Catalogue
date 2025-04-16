using Backend.DTOs;
using Backend.DTOs.Backend.DTOs;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var studentDTOs = new List<StudentDTO>();

            foreach (var student in students)
            {
                studentDTOs.Add(new StudentDTO
                {
                    Id = student.Id,
                    Username = student.Username,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Password = student.Password
                });
            }

            return Ok(studentDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var studentDTO = new StudentDTO
            {
                Id = student.Id,
                Username = student.Username,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Password = student.Password
            };

            return Ok(studentDTO);
        }

        [HttpPost]
        public async Task<ActionResult<StudentDTO>> CreateStudent([FromBody] CreateStudentDTO createStudentDto)
        {
            var student = new Student
            {
                Username = createStudentDto.Username,
                Password = createStudentDto.Password, 
                FirstName = createStudentDto.FirstName,
                LastName = createStudentDto.LastName
            };

            var createdStudent = await _studentService.CreateStudentAsync(student);

            var studentDTO = new StudentDTO
            {
                Id = createdStudent.Id,
                Username = createdStudent.Username,
                FirstName = createdStudent.FirstName,
                LastName = createdStudent.LastName,
                Password = createdStudent.Password
            };

            return CreatedAtAction(nameof(GetStudent), new { id = studentDTO.Id }, studentDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDTO updateStudentDto)
        {
            var existingStudent = await _studentService.GetStudentByIdAsync(id);
            if (existingStudent == null)
            {
                return NotFound();
            }

            existingStudent.Username = updateStudentDto.Username;
            existingStudent.Password = updateStudentDto.Password;
            existingStudent.FirstName = updateStudentDto.FirstName;
            existingStudent.LastName = updateStudentDto.LastName;

            await _studentService.UpdateStudentAsync(existingStudent);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

       

    }
}

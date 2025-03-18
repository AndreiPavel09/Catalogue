using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Teacher management
        [HttpPost("teachers")]
        public async Task<ActionResult<UserDto>> AddTeacher(CreateUserDto teacherDto)
        {
            try
            {
                var teacher = await _adminService.AddTeacherAsync(teacherDto);
                return CreatedAtAction(nameof(ViewTeachers), null, teacher);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("teachers/{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            try
            {
                await _adminService.DeleteTeacherAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("teachers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> ViewTeachers()
        {
            var teachers = await _adminService.ViewTeachersAsync();
            return Ok(teachers);
        }

        // Course management
        [HttpPost("courses")]
        public async Task<ActionResult<CourseDto>> AddCourse(CreateCourseDto courseDto)
        {
            try
            {
                var course = await _adminService.AddCourseAsync(courseDto);
                return CreatedAtAction(nameof(ViewCourses), null, course);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                await _adminService.DeleteCourseAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("courses")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> ViewCourses()
        {
            var courses = await _adminService.ViewCoursesAsync();
            return Ok(courses);
        }

        // Student management
        [HttpPost("students")]
        public async Task<ActionResult<UserDto>> AddStudent(CreateUserDto studentDto)
        {
            try
            {
                var student = await _adminService.AddStudentAsync(studentDto);
                return CreatedAtAction(nameof(ViewStudents), null, student);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("students/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                await _adminService.DeleteStudentAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("students")]
        public async Task<ActionResult<IEnumerable<UserDto>>> ViewStudents()
        {
            var students = await _adminService.ViewStudentsAsync();
            return Ok(students);
        }

        // Grade management
        [HttpPost("grades")]
        public async Task<ActionResult<GradeDto>> AddGrade(CreateGradeDto gradeDto)
        {
            try
            {
                var grade = await _adminService.AddGradeAsync(gradeDto);
                return CreatedAtAction(nameof(ViewGrades), null, grade);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("grades/{id}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            try
            {
                await _adminService.DeleteGradeAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("grades")]
        public async Task<ActionResult<IEnumerable<GradeDto>>> ViewGrades()
        {
            var grades = await _adminService.ViewGradesAsync();
            return Ok(grades);
        }
    }
}
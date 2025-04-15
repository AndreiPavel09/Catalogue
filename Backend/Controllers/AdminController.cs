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
                return NoContent(); // 204 Success, no content to return
            }
            catch (InvalidOperationException ex)
            {
                // Check if the exception is the specific one for assigned courses
                if (ex.Message.StartsWith("Cannot delete teacher"))
                {
                    // Return 400 Bad Request with the specific error message
                    return BadRequest(new { message = ex.Message });
                }
                else // Assume it's the "Teacher not found" case from the service
                {
                    return NotFound(new { message = ex.Message }); // 404 Not Found
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Catch unexpected database errors during the delete (though our check should prevent the common ones)
                // Log the detailed exception (dbEx) for server-side debugging
                Console.WriteLine($"Database Error Deleting Teacher: {dbEx}"); // Basic logging
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "A database error occurred while attempting to delete the teacher." });
            }
            catch (Exception ex) // Catch any other unexpected errors
            {
                // Log the detailed exception (ex) for server-side debugging
                Console.WriteLine($"Generic Error Deleting Teacher: {ex}"); // Basic logging
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
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
                return NoContent(); // 204 Success
            }
            catch (InvalidOperationException ex)
            {
                // Check if it's our specific 'cannot delete' messages or 'not found'
                if (ex.Message.StartsWith("Cannot delete student"))
                {
                    // Return 400 Bad Request with the specific error message
                    return BadRequest(new { message = ex.Message });
                }
                else // Assume "Student not found"
                {
                    return NotFound(new { message = ex.Message }); // 404 Not Found
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Catch unexpected DB issues
            {
                Console.WriteLine($"Database Error Deleting Student: {dbEx}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "A database error occurred." });
            }
            catch (Exception ex) // Catch any other errors
            {
                Console.WriteLine($"Generic Error Deleting Student: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
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
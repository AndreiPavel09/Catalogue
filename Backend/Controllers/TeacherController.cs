using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization; // Required for authorization
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims; // Required for getting user ID
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/teachers/{teacherId}")] // Add teacherId to the base route
    [ApiController]
    // [Authorize(Roles = "Teacher")] // REMOVED - No authentication assumed
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService) // Inject the service interface
        {
            _teacherService = teacherService;
        }

        // --- REMOVED TryGetTeacherId helper ---

        // GET: api/teachers/{teacherId}/courses
        [HttpGet("courses")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetMyCourses(int teacherId)
        {
            try
            {
                // Optional: Check if teacher exists first for a slightly better error message
                // if (!await _teacherService.TeacherExistsAsync(teacherId))
                // {
                //     return NotFound($"Teacher with ID {teacherId} not found.");
                // }
                var courses = await _teacherService.GetTeacherCoursesAsync(teacherId);
                return Ok(courses);
            }
            catch (KeyNotFoundException knfex) // Catch specific exception for not found teacher
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex) // Catch unexpected errors
            {
                // Log the exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred retrieving courses.");
            }
        }

        // GET: api/teachers/{teacherId}/courses/{courseId}/students
        [HttpGet("courses/{courseId}/students")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudentsInCourse(int teacherId, int courseId)
        {
            try
            {
                var students = await _teacherService.GetStudentsInCourseAsync(teacherId, courseId);
                return Ok(students);
            }
            catch (UnauthorizedAccessException uaex) // Teacher doesn't teach this course
            {
                // Return Forbidden (403) as the teacher exists but isn't allowed access here
                // Or NotFound (404) to hide the course's existence from unauthorized teachers
                return NotFound(uaex.Message); // Let's use NotFound for simplicity/security through obscurity
                // return Forbid(uaex.Message); // Alternative status code
            }
            catch (KeyNotFoundException knfex) // Underlying resource (e.g. course itself) might not exist
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                // Log exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred retrieving students.");
            }
        }

        // POST: api/teachers/{teacherId}/grades
        [HttpPost("grades")]
        // Note: teacherId comes from the route, AssignGradeDto from the body
        public async Task<ActionResult<GradeDto>> AssignOrUpdateGrade(int teacherId, [FromBody] AssignGradeDto assignGradeDto)
        {
            if (!ModelState.IsValid) // Basic validation from DTO attributes
            {
                return BadRequest(ModelState);
            }

            try
            {
                var resultingGrade = await _teacherService.AssignOrUpdateGradeAsync(teacherId, assignGradeDto);
                // For simplicity, returning OK for both create/update.
                // Could check if resultingGrade.Id was newly generated vs existing to return CreatedAtAction.
                return Ok(resultingGrade);
            }
            catch (UnauthorizedAccessException uaex) // Teacher doesn't teach this course
            {
                return NotFound(uaex.Message); // Or Forbid()
            }
            catch (ArgumentException argex) // Invalid input data (e.g., student not enrolled)
            {
                return BadRequest(argex.Message);
            }
            catch (KeyNotFoundException knfex) // Underlying resource might not exist
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                // Log exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred assigning the grade.");
            }
        }

        // GET: api/teachers/{teacherId}/courses/{courseId}/grades
        [HttpGet("courses/{courseId}/grades")]
        public async Task<ActionResult<IEnumerable<GradeDto>>> GetGradesForCourse(int teacherId, int courseId)
        {
            try
            {
                var grades = await _teacherService.GetGradesForCourseAsync(teacherId, courseId);
                return Ok(grades);
            }
            catch (UnauthorizedAccessException uaex) // Teacher doesn't teach this course
            {
                return NotFound(uaex.Message); // Or Forbid()
            }
            catch (KeyNotFoundException knfex) // Underlying resource might not exist
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                // Log exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred retrieving grades.");
            }
        }
    }
}

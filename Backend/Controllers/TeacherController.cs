using Backend.Data;
using Backend.DTOs;
using Backend.DTOs.Backend.DTOs;
using Backend.Models;
using Backend.Services.Implementations;
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
        // 1. Change Route - Remove teacherId parameter
        [Route("api/teacher")]
        [ApiController]
        // 2. Add Authorization
// Make sure "Teacher" role name matches your Identity setup
        public class TeachersController : ControllerBase
        {
            private readonly ITeacherService _teacherService;
        private readonly CurrentUserService _currentUserService;

            public TeachersController(ITeacherService teacherService,CurrentUserService currentUserService)
            {
                _teacherService = teacherService;
                _currentUserService = currentUserService;
            }

            // Helper method to get validated teacher ID from claims
            private bool TryGetAuthenticatedTeacherId(out int teacherId)
            {
                teacherId = 0;
                if(_currentUserService.GetUserId()==0)
                {
                return false;
                }
            teacherId = _currentUserService.GetUserId();
            return true;
            }

            // GET: api/teacher/courses
            [HttpGet("courses")]
            public async Task<ActionResult<IEnumerable<CourseDto>>> GetMyCourses()
            {
                // 3. Get teacherId from claims
                if (!TryGetAuthenticatedTeacherId(out var teacherId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                try
                {
                    var courses = await _teacherService.GetTeacherCoursesAsync(teacherId); // Pass validated ID
                    return Ok(courses);
                }
                // Removed KeyNotFoundException for teacherId as it's now validated from token
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting courses for teacher {teacherId}: {ex}"); // Basic Logging
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred retrieving your courses.");
                }
            }

            // GET: api/teacher/courses/{courseId}/students
            [HttpGet("courses/{courseId}/students")]
            public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudentsInCourse(int courseId) // Removed teacherId param
            {
                // 3. Get teacherId from claims
                if (!TryGetAuthenticatedTeacherId(out var teacherId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                try
                {
                    var students = await _teacherService.GetStudentsInCourseAsync(teacherId, courseId); // Pass validated ID
                    return Ok(students);
                }
                catch (UnauthorizedAccessException uaex)
                {
                    // Return 403 Forbidden as the teacher is authenticated but not authorized for this specific course
                    return Forbid(uaex.Message);
                }
                catch (KeyNotFoundException knfex) // Course or other resource not found
                {
                    return NotFound(knfex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting students for teacher {teacherId}, course {courseId}: {ex}");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred retrieving students.");
                }
            }

            // POST: api/teacher/grades
            [HttpPost("grades")]
            public async Task<ActionResult<GradeDto>> AssignOrUpdateGrade([FromBody] AssignGradeDto assignGradeDto) // Removed teacherId param
            {
                // 3. Get teacherId from claims
                if (!TryGetAuthenticatedTeacherId(out var teacherId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                if (!ModelState.IsValid) return BadRequest(ModelState);

                try
                {
                    var resultingGrade = await _teacherService.AssignOrUpdateGradeAsync(teacherId, assignGradeDto); // Pass validated ID
                    return Ok(resultingGrade);
                }
                catch (UnauthorizedAccessException uaex)
                {
                    return Forbid(uaex.Message); // Use 403 Forbidden
                }
                catch (ArgumentException argex)
                {
                    return BadRequest(new { message = argex.Message }); // Use 400 Bad Request
                }
                catch (KeyNotFoundException knfex)
                {
                    return NotFound(new { message = knfex.Message }); // Use 404 Not Found
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error assigning grade for teacher {teacherId}: {ex}");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred assigning the grade.");
                }
            }

            // GET: api/teacher/courses/{courseId}/grades
            [HttpGet("courses/{courseId}/grades")]
            public async Task<ActionResult<IEnumerable<GradeDto>>> GetGradesForCourse(int courseId) // Removed teacherId param
            {
                // 3. Get teacherId from claims
                if (!TryGetAuthenticatedTeacherId(out var teacherId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                try
                {
                    var grades = await _teacherService.GetGradesForCourseAsync(teacherId, courseId); // Pass validated ID
                    return Ok(grades);
                }
                catch (UnauthorizedAccessException uaex)
                {
                    return Forbid(uaex.Message); // Use 403 Forbidden
                }
                catch (KeyNotFoundException knfex)
                {
                    return NotFound(new { message = knfex.Message }); // Use 404 Not Found
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting grades for teacher {teacherId}, course {courseId}: {ex}");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred retrieving grades.");
                }
            }

        [HttpPost("courses/{courseId}/students/{studentId}")]
        public async Task<IActionResult> AddStudentToCourse(int courseId, int studentId)
        {
            if (!TryGetAuthenticatedTeacherId(out var teacherId))
            {
                return Unauthorized("User ID not found in token.");
            }

            try
            {
                await _teacherService.AddStudentToCourseAsync(teacherId, studentId, courseId);
                return Ok(new { message = "Student added to course successfully." }); // Or return NoContent()
            }
            catch (UnauthorizedAccessException uaex) { return Forbid(uaex.Message); }
            catch (KeyNotFoundException knfex) { return NotFound(new { message = knfex.Message }); } // Student or Course not found
            catch (ArgumentException argex) { return BadRequest(new { message = argex.Message }); } // Already enrolled
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding student {studentId} to course {courseId} for teacher {teacherId}: {ex}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // DELETE: api/teacher/courses/{courseId}/students/{studentId}
        [HttpDelete("courses/{courseId}/students/{studentId}")]
        public async Task<IActionResult> RemoveStudentFromCourse(int courseId, int studentId)
        {
            if (!TryGetAuthenticatedTeacherId(out var teacherId))
            {
                return Unauthorized("User ID not found in token.");
            }

            try
            {
                await _teacherService.RemoveStudentFromCourseAsync(teacherId, studentId, courseId);
                return NoContent(); // Success
            }
            catch (UnauthorizedAccessException uaex) { return Forbid(uaex.Message); }
            catch (KeyNotFoundException knfex) { return NotFound(new { message = knfex.Message }); } // Enrollment not found
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing student {studentId} from course {courseId} for teacher {teacherId}: {ex}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("courses/{courseId}/grades/bulk")]
        public async Task<ActionResult<BulkCreateGradesResponseDto>> AddGradesBulk(int courseId, [FromBody] List<StudentGradeEntryDto> gradeEntries)
        {
            if (!TryGetAuthenticatedTeacherId(out var teacherId))
            {
                return Unauthorized("User ID not found in token.");
            }

            // Basic validation: Check if list is provided
            if (gradeEntries == null || !gradeEntries.Any())
            {
                return BadRequest(new { message = "Grade entries list cannot be empty." });
            }

            // Construct the request DTO expected by the service
            var requestDto = new BulkCreateGradesRequestDto
            {
                CourseId = courseId,
                GradeEntries = gradeEntries
            };

            // Note: Detailed validation of individual entries happens in the service
            // You could add more ModelState validation here if desired

            try
            {
                var result = await _teacherService.AddGradesBulkAsync(teacherId, requestDto);

                if (result.Errors.Any())
                {
                    // Return partial success with errors if some grades were added
                    if (result.GradesSuccessfullyAdded > 0)
                    {
                        return Ok(result); // 200 OK, but check response body for errors
                    }
                    // Return bad request if only errors occurred
                    return BadRequest(result); // Contains list of errors
                }

                return Ok(result); // All successful
            }
            catch (Exception ex) // Catch unexpected service errors
            {
                Console.WriteLine($"Error adding bulk grades for course {courseId}: {ex}");
                return StatusCode(500, "An unexpected error occurred during bulk grade processing.");
            }
        }
    }
    }

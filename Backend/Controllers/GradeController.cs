using Backend.DTOs;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class GradeController:ControllerBase
    {
        private readonly IGradeService _gradeService;

        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GradeDto>> GetGrade(int id)
        {
            var grade = await _gradeService.GetGradeByIdAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            var gradeDto = new GradeDto
            {
                Id = grade.Id,
                StudentId = grade.StudentId,
                CourseId = grade.CourseId,
                Value = grade.Value,
            };

            return Ok(gradeDto);
        }

        [HttpPost]
        public async Task<ActionResult<GradeDto>> CreateGrade([FromBody] CreateGradeDto createGradeDto)
        {
            var grade = new Grade
            {
                StudentId = createGradeDto.StudentId,
                CourseId = createGradeDto.CourseId,
                Value = createGradeDto.Value,
            };

            var createdGrade = await _gradeService.CreateGradeAsync(grade);

            var gradeDto = new GradeDto
            {
                Id = createdGrade.Id,
                StudentId = createdGrade.StudentId,
                CourseId = createdGrade.CourseId,
                Value = createdGrade.Value,
            };

            return CreatedAtAction(nameof(GetGrade), new { id = gradeDto.Id }, gradeDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrade(int id, [FromBody] UpdateGradeDto updateGradeDto)
        {
            var existingGrade = await _gradeService.GetGradeByIdAsync(id);
            if (existingGrade == null)
            {
                return NotFound();
            }

            if (updateGradeDto.Value.HasValue)
            {
                existingGrade.Value = updateGradeDto.Value.Value;
            }
           
            await _gradeService.UpdateGradeAsync(existingGrade);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var result = await _gradeService.DeleteGradeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("students/{studentId}/coursegrades")]
        public async Task<ActionResult<IEnumerable<CourseGradeDto>>> GetStudentCourseGrades(int studentId)
        {
            var grades = await _gradeService.GetCourseGradesForStudentAsync(studentId);
            return Ok(grades);
        }
        [HttpGet("students/{studentId}/averagegrade")]
        public async Task<ActionResult<object>> GetStudentAverageGrade(int studentId)
        {
            var average = await _gradeService.CalculateAverageGradeForStudentAsync(studentId);

            return Ok(new { average = average });
        }

    }
}

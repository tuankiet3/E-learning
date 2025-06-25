using Microsoft.AspNetCore.Mvc;
using E_learning.Model.Courses;
using System.Threading.Tasks;
using E_learning.DAL.Course;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using E_learning.DTO.Course;
using E_learning.Repositories;
using E_learning.Services;
namespace E_learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly GenerateID _generateID;
        public LessonController(ILogger<CourseController> logger, ICourseRepository courseRepo, GenerateID generateID)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _generateID = generateID;
        }
        [HttpGet("GetLessonsByCourseID/{courseID}")]
        [ProducesResponseType(typeof(IEnumerable<LessonModel>), statusCode: 200)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLessonsByCourseID(string courseID)
        {
            try
            {
                List<LessonModel> lessons = await _courseRepo.GetLessonsByCourseID(courseID);
                if (lessons == null || lessons.Count == 0)
                {
                    return NotFound("No lessons found for the specified course ID");
                }
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons for course ID: {CourseID}", courseID);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("DeleteLesson/{lessonID}")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLesson(string lessonID)
        {
            try
            {
                bool isDeleted = await _courseRepo.DeleteLessons(lessonID);
                if (!isDeleted)
                {
                    return NotFound("Lesson not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson with ID: {LessonID}", lessonID);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("InsertLesson")]
        [ProducesResponseType(typeof(LessonModel), statusCode: 201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertLesson([FromBody] LessonDTO lesson)
        {
            if (lesson == null)
            {
                return BadRequest("Lesson data is null");
            }
            try
            {
                string lessonID = _generateID.generateLessonID();
                LessonModel lessonModel = new LessonModel(
                    lessonID,
                    lesson.lessonTitle,
                    lesson.lessonURL,
                    lesson.courseID
                );
                bool isInserted = await _courseRepo.InsertLesson(lessonModel);
                if (!isInserted)
                {
                    return StatusCode(500, "Error inserting lesson");
                }
                return CreatedAtAction(nameof(GetLessonsByCourseID), new { courseID = lesson.courseID }, lessonModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting lesson");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

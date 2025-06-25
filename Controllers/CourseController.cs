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
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly GenerateID _generateID;

        public CourseController(ILogger<CourseController> logger, ICourseRepository courseRepo, GenerateID generateID)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _generateID = generateID;
        }

        [HttpGet("GetAllCourses")]
        [ProducesResponseType(typeof(IEnumerable<CoursesModel>), statusCode: 200)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                List<CoursesModel> courses = await _courseRepo.GetAllCourses();
                if (courses == null || courses.Count == 0)
                {
                    return NotFound("No courses found");
                }
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all courses");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("InsertCourse")]
        [ProducesResponseType(typeof(CoursesModel), statusCode: 201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertCourse([FromBody] CourseDTO course)
        {
            if (course == null)
            {
                return BadRequest("Course data is null");
            }
            try
            {
                string CourseID = _generateID.generateCourseID();
                CoursesModel courseModel = new CoursesModel(
                    CourseID,
                    course.CourseName,
                    course.CoursePrice,
                    course.Author
                );
                bool isInserted = await _courseRepo.InsertCourse(courseModel);
                if (isInserted)
                {
                    return Ok(new { Message = "Course inserted successfully." });
                }
                else
                {
                    return BadRequest("Failed to insert course");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting course");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("DeleteCourse/{courseID}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCourse(string courseID)
        {
            if (string.IsNullOrEmpty(courseID))
            {
                return BadRequest("Course ID is null or empty");
            }
            try
            {
                bool isDeleted = await _courseRepo.DeleteCourse(courseID);
                if (isDeleted)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound("Course not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course with ID: {CourseID}", courseID);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetCourseByID/{courseID}")]
        [ProducesResponseType(typeof(CoursesModel), statusCode: 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCourseByID(string courseID)
        {
            if (string.IsNullOrEmpty(courseID))
            {
                return BadRequest("Course ID is null or empty");
            }
            try
            {
                CoursesModel course = await _courseRepo.GetCourseByID(courseID);
                if (course == null)
                {
                    return NotFound("Course not found");
                }
                return Ok(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course with ID: {CourseID}", courseID);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

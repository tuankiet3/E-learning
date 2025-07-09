using Microsoft.AspNetCore.Mvc;
using E_learning.Model.Courses;
using E_learning.DTO.Course;
using E_learning.Services;
using E_learning.Repositories.Course;
using Microsoft.AspNetCore.Authorization;
using E_learning.Enums;

namespace E_learning.Controllers.Course
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly GenerateID _generateID;
        private readonly CheckExsistingID _checkExsistingID;
        public CourseController(ILogger<CourseController> logger, ICourseRepository courseRepo, GenerateID generateID, CheckExsistingID exsistingID)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _generateID = generateID;
            _checkExsistingID = exsistingID;
        }

        [HttpGet("GetAllCourses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                List<CoursesModel> courses = await _courseRepo.GetAllCourses();
                Console.WriteLine(courses);
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
        [Authorize(Roles = $"{nameof(UserRole.Lecturer)},{nameof(UserRole.Admin)}")]
        public async Task<IActionResult> InsertCourse([FromBody] CourseDTO course)
        {
            if (course == null)
            {
                return BadRequest("Course data is null");
            }
            try
            {
                string newID = await _checkExsistingID.GenerateUniqueID(
                    _courseRepo.GetAllCourses,
                    c => c.GetCourseID(),
                    _generateID.generateCourseID
                );
                CoursesModel courseModel = new CoursesModel(
                    newID,
                    course.CourseName,
                    course.CoursePrice,
                    course.CourseDescription,
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
        [Authorize(Roles = nameof(UserRole.Admin))]
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
        [AllowAnonymous]
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

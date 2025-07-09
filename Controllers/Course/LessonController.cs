using Microsoft.AspNetCore.Mvc;
using E_learning.Model.Courses;
using E_learning.DTO.Course;
using E_learning.Services;
using E_learning.Repositories.Course;
using E_learning.Services.Lesson;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
namespace E_learning.Controllers.Course
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly GenerateID _generateID;
        private readonly CheckExsistingID _checkExsistingID;
        private readonly ConvertURL _convertURL;
        public LessonController(ILogger<CourseController> logger, ICourseRepository courseRepo, GenerateID generateID, CheckExsistingID exsistingID, ConvertURL convertURL)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _generateID = generateID;
            _checkExsistingID = exsistingID;
            _convertURL = convertURL;

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
        [ProducesResponseType(typeof(LessonModel), statusCode: 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertLesson([FromForm] LessonDTO lesson)
        {
            try
            {
                string lessonID = await _checkExsistingID.GenerateUniqueIDForStringList(
                    _courseRepo.GetAllLessonsID,
                    _generateID.generateLessonID
                );

                _logger.LogInformation("🆕 Generating lesson ID: {lessonId}", lessonID);

                await _convertURL.UploadVideo(lesson.videoFile, lessonID);

                string conversionResult = await _convertURL.TryConvertWithRetry(lessonID);

               
                var fullUrl = $"{Request.Scheme}://{Request.Host}{conversionResult}";
                var model = new LessonModel(
                    lessonID,
                    lesson.lessonTitle,
                    fullUrl,
                    lesson.courseID
                );

                bool inserted = await _courseRepo.InsertLesson(model);

                if (!inserted)
                {
                    await _convertURL.DeleteVideo(lessonID); // Clean up if insert fails
                    return StatusCode(500, "Failed to insert lesson");
                }

                if (conversionResult.Contains("failed") || conversionResult.Contains("timed out"))
                {
                    return Ok(new
                    {
                        message = "Lesson inserted, but video conversion failed.",
                        lesson = model
                    });
                }

                return Ok(new
                {
                    message = "Lesson inserted successfully",
                    lesson = model
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ InsertLesson failed");
                return StatusCode(500, "Unexpected error occurred: " + ex.Message);
            }
        }

        [Authorize]
        [AcceptVerbs("GET", "HEAD")]
        [Route("authorize")]
        public async Task<IActionResult> AuthorizeVideoAccess()
        {
            var uri = Request.Headers["X-Original-URI"].ToString();
            Console.WriteLine("🔍 Header X-Original-URI = " + uri);

            if (string.IsNullOrEmpty(uri))
            {
                Console.WriteLine("❌ X-Original-URI header is missing or empty.");
                return BadRequest();
            }

            var match = Regex.Match(uri, @"^/secure/videos/([^/]+)/");
            if (!match.Success)
            {
                Console.WriteLine("❌ Invalid video URL format.");
                return BadRequest();
            }

            var lessonId = match.Groups[1].Value;
            var userId = User.FindFirst("UserID")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("❌ User ID is missing in the token.");
                return Unauthorized();
            }

            bool hasAccess = await _courseRepo.checkBuyCourse(userId, lessonId);
            if (!hasAccess)
            {
                Console.WriteLine($"❌ User {userId} does not have access to lesson {lessonId}.");
                return Forbid();
            }

            Console.WriteLine($"✅ User {userId} is authorized to access lesson {lessonId}.");

            // 🔥 Trả về rỗng nếu là HEAD (nghĩa là NGINX gọi), tránh lỗi upstream
            if (HttpContext.Request.Method == HttpMethods.Head)
            {
                Console.WriteLine("🔍 HEAD request received, returning 200 OK without body.");
                return Ok(); // trả về 200 OK, không body
            }

            // Nếu là GET, trả về thông báo thành công
            Console.WriteLine("✅ Access granted for GET request.");
            return Ok(new { message = "Access granted" }); // nếu bạn test thủ công qua GET
        }
    }
}

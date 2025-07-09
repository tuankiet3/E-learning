using Microsoft.AspNetCore.Mvc;
using E_learning.Model.Courses;
using E_learning.DTO.Course;
using E_learning.Services;
using E_learning.Repositories.Course;
using E_learning.Services.Lesson; // [MERGE] Thêm dependency mới
using Microsoft.AspNetCore.Authorization;
using E_learning.Enums; // [MERGE] Giữ lại dependency này để dùng Roles
using System.Text.RegularExpressions; // [MERGE] Thêm dependency mới

namespace E_learning.Controllers.Course
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // [MERGE] Áp dụng Authorize ở cấp độ controller từ phiên bản HEAD
    public class LessonController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly GenerateID _generateID;
        private readonly CheckExsistingID _checkExsistingID;
        private readonly ConvertURL _convertURL; // [MERGE] Thêm service mới

        // [MERGE] Constructor hợp nhất, bao gồm tất cả các dependency
        public LessonController(
            ILogger<CourseController> logger,
            ICourseRepository courseRepo,
            GenerateID generateID,
            CheckExsistingID exsistingID,
            ConvertURL convertURL)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _generateID = generateID;
            _checkExsistingID = exsistingID;
            _convertURL = convertURL; // [MERGE] Khởi tạo service mới
        }

        [HttpGet("GetLessonsByCourseID/{courseID}")]
        [ProducesResponseType(typeof(IEnumerable<LessonModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [Authorize(Roles = $"{nameof(UserRole.Lecturer)},{nameof(UserRole.Admin)}")] // [MERGE] Thêm lại Authorize Roles từ HEAD
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
        [Authorize(Roles = $"{nameof(UserRole.Lecturer)},{nameof(UserRole.Admin)}")] // [MERGE] Thêm lại Authorize Roles từ HEAD
        [ProducesResponseType(typeof(LessonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertLesson([FromForm] LessonDTO lesson)
        {
            // [MERGE] Sử dụng logic xử lý video từ phiên bản mới (80fe94d...) vì nó chuyên nghiệp và tách biệt hơn.
            try
            {
                string lessonID = await _checkExsistingID.GenerateUniqueID(
                    _courseRepo.GetAllLessons,
                    l => l.GetLessonID(), // Sử dụng lambda expression nhất quán
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
                    await _convertURL.DeleteVideo(lessonID); // Clean up nếu insert thất bại
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

        // [MERGE] Giữ lại toàn bộ endpoint mới để xác thực video
        [AllowAnonymous] // NGINX sẽ gửi yêu cầu không có token, app sẽ check token riêng
        [AcceptVerbs("GET", "HEAD")]
        [Route("authorize")]
        public async Task<IActionResult> AuthorizeVideoAccess()
        {
            var uri = Request.Headers["X-Original-URI"].ToString();
            Console.WriteLine("🔍 Header X-Original-URI = " + uri);

            if (string.IsNullOrEmpty(uri))
            {
                Console.WriteLine("❌ X-Original-URI header is missing or empty.");
                return Forbid(); // Trả về 403 thay vì 400
            }

            // Lấy token từ header Authorization
            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                Console.WriteLine("❌ Authorization token is missing.");
                return Forbid();
            }
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // Xác thực token và lấy userId (bạn cần có logic này, đây là ví dụ)
            var userId = GetUserIdFromToken(token); // Bạn cần tự implement hàm này
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("❌ Invalid token or User ID is missing in the token.");
                return Forbid();
            }

            var match = Regex.Match(uri, @"^/secure/videos/([^/]+)/");
            if (!match.Success)
            {
                Console.WriteLine("❌ Invalid video URL format.");
                return Forbid();
            }

            var lessonId = match.Groups[1].Value;

            bool hasAccess = await _courseRepo.checkBuyCourse(userId, lessonId);
            if (!hasAccess)
            {
                Console.WriteLine($"❌ User {userId} does not have access to lesson {lessonId}.");
                return Forbid();
            }

            Console.WriteLine($"✅ User {userId} is authorized to access lesson {lessonId}.");
            return Ok(); // Luôn trả về 200 OK để NGINX cho phép truy cập
        }

        // Hàm ví dụ, bạn cần thay thế bằng logic giải mã JWT token của mình
        private string GetUserIdFromToken(string token)
        {
            // Ví dụ: sử dụng System.IdentityModel.Tokens.Jwt
            // var handler = new JwtSecurityTokenHandler();
            // var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            // var userId = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "UserID")?.Value;
            // return userId;
            return "user_id_from_token"; // Placeholder
        }
    }
}
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
    public class LessonController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly GenerateID _generateID;
        private readonly CheckExsistingID _checkExsistingID;
        public LessonController(ILogger<CourseController> logger, ICourseRepository courseRepo, GenerateID generateID, CheckExsistingID exsistingID)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _generateID = generateID;
            _checkExsistingID = exsistingID;
        }

        [HttpGet("GetLessonsByCourseID/{courseID}")]
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
        [Authorize(Roles = $"{nameof(UserRole.Lecturer)},{nameof(UserRole.Admin)}")]
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
        [Authorize(Roles = $"{nameof(UserRole.Lecturer)},{nameof(UserRole.Admin)}")]
        public async Task<IActionResult> InsertLesson([FromForm] LessonDTO lesson)
        {
            if (lesson == null)
            {
                return BadRequest("Lesson data is null");
            }
            if (lesson.videoFile == null || lesson.videoFile.Length == 0)
            {
                return BadRequest("Video file is null or empty");
            }
            string newID = await _checkExsistingID.GenerateUniqueID(
                   _courseRepo.GetAllLessons,
                   l => l.GetLessonID(),
                   _generateID.generateLessonID
              );
            var orgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var videoPath = Path.Combine(orgPath, "videos");
            if (!Directory.Exists(videoPath))
            {
                Directory.CreateDirectory(videoPath);
            }
            var fileExt = Path.GetExtension(lesson.videoFile.FileName);
            var fileName = $"{newID}{fileExt}";
            var savePath = Path.Combine(videoPath, fileName);
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await lesson.videoFile.CopyToAsync(stream);
            }

            var videoUrl = $"{Request.Scheme}://{Request.Host}/videos/{fileName}";
            try
            {
                LessonModel lessonModel = new LessonModel(
                    newID,
                    lesson.lessonTitle,
                    videoUrl,
                    lesson.courseID
                );
                bool isInserted = await _courseRepo.InsertLesson(lessonModel);
                if (!isInserted)
                {
                    return BadRequest("Failed to insert lesson");
                }
                return Ok(new
                {
                    Message = "Lesson inserted successfully",
                    Lesson = lessonModel
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting lesson");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}


//[HttpPost("upload_video")]
//[ProducesResponseType(typeof(string), statusCode: 201)]
//[ProducesResponseType(StatusCodes.Status400BadRequest)]
//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
//public async Task<IActionResult> UploadVideo(IFormFile videoFile)
//{
//    if(videoFile == null || videoFile.Length == 0)
//    {
//        return BadRequest("Video file is null or empty");
//    }
//    var orgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
//    var videoPath = Path.Combine(orgPath, "videos");
//    if(!Directory.Exists(videoPath))
//    {
//        Directory.CreateDirectory(videoPath);
//    }
//    var fileName = Path.GetFileName(videoFile.FileName);

//    // Đường dẫn lưu file
//    var savePath = Path.Combine(videoPath, fileName);

//    // Lưu file vào ổ đĩa
//    using (var stream = new FileStream(savePath, FileMode.Create))
//    {
//        await videoFile.CopyToAsync(stream);
//    }

//    // Trả về URL truy cập video
//    var videoUrl = $"{Request.Scheme}://{Request.Host}/videos/{fileName}";
//    return Ok(new { url = videoUrl });
//}
using E_learning.Services;
using Microsoft.AspNetCore.Mvc;
using E_learning.Model.Courses;
using E_learning.DTO.Course;
using E_learning.Repositories.Course;
using Microsoft.AspNetCore.Authorization;
using E_learning.Enums;

namespace E_learning.Controllers.Course
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly GenerateID _generateID;
        private readonly CheckExsistingID _checkExsistingID;
        public QuizController(ILogger<CourseController> logger, ICourseRepository courseRepo, GenerateID generateID, CheckExsistingID checkExsistingID)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _generateID = generateID;
            _checkExsistingID = checkExsistingID;

        }

        [HttpGet("GetQuizzesByCourseID/{courseID}")]
        public async Task<IActionResult> GetQuizzesByCourseID(string courseID)
        {
            try
            {
                List<QuizModel> quizzes = await _courseRepo.GetQuizzesByCourseID(courseID);
                if (quizzes == null || quizzes.Count == 0)
                {
                    return NotFound("No quizzes found for the specified course ID");
                }
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes for course ID: {CourseID}", courseID);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("DeleteQuiz/{quizID}")]
        [Authorize(Roles = $"{nameof(UserRole.Lecturer)},{nameof(UserRole.Admin)}")]
        public async Task<IActionResult> DeleteQuiz(string quizID)
        {
            try
            {
                bool isDeleted = await _courseRepo.DeleteQuiz(quizID);
                if (!isDeleted)
                {
                    return NotFound("Quiz not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quiz with ID: {QuizID}", quizID);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("InsertQuiz")]
        [Authorize(Roles = $"{nameof(UserRole.Lecturer)},{nameof(UserRole.Admin)}")]
        public async Task<IActionResult> InsertQuiz([FromBody] QuizDTO quiz)
        {
            if (quiz == null)
            {
                return BadRequest("Quiz data is null");
            }
            try
            {
                string newID = await _checkExsistingID.GenerateUniqueID(_courseRepo.GetAllQuiz, q => q.getQuizID(), _generateID.generateQuizID
                );
                QuizModel quizModel = new QuizModel(
                    newID,
                    quiz.QuizTitle,
                    quiz.courseID
                );
                bool isInserted = await _courseRepo.InsertQuiz(quizModel);
                if (!isInserted)
                {
                    return BadRequest("Failed to insert quiz");
                }
                return CreatedAtAction(nameof(GetQuizzesByCourseID), new { courseID = quiz.courseID }, quizModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting quiz");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

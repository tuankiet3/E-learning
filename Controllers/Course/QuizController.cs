using E_learning.Services;
using Microsoft.AspNetCore.Mvc;
using E_learning.Model.Courses;
using E_learning.DTO.Course;
using E_learning.Repositories.Course;
namespace E_learning.Controllers.Course
{
    [ApiController]
    [Route("api/[controller]")]
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
        [ProducesResponseType(typeof(IEnumerable<QuizModel>), statusCode: 200)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(QuizModel), statusCode: 201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertQuiz([FromBody] QuizDTO quiz)
        {
            if (quiz == null)
            {
                return BadRequest("Quiz data is null");
            }
            try
            {
                string newID = await _checkExsistingID.GenerateUniqueIDForStringList(_courseRepo.GetAllQuizID, _generateID.generateQuizID
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
                return CreatedAtAction(nameof(GetQuizzesByCourseID), new { quiz.courseID }, quizModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting quiz");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using E_learning.Model.Courses;
using E_learning.DTO.Course;
using E_learning.Services;
using E_learning.Repositories.Course;


namespace E_learning.Controllers.Course
{

    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly GenerateID _generateID;
        private readonly CheckExsistingID _checkExsistingID;
        public QuestionController(ILogger<CourseController> logger, ICourseRepository courseRepo, GenerateID generateID, CheckExsistingID checkExsistingID)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _generateID = generateID;
            _checkExsistingID = checkExsistingID;
        }


        [HttpGet("GetQuestionsByQuizID/{quizID}")]
        [ProducesResponseType(typeof(IEnumerable<QuestionModel>), statusCode: 200)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQuestionsByQuizID(string quizID)
        {
            try
            {
                List<QuestionModel> questions = await _courseRepo.GetQuestionsByQuizID(quizID);
                if (questions == null || questions.Count == 0)
                {
                    return NotFound("No questions found for the specified quiz ID");
                }
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for quiz ID: {QuizID}", quizID);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("DeleteQuestion/{questionID}")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteQuestion(string questionID)
        {
            try
            {
                bool isDeleted = await _courseRepo.DeleteQuestion(questionID);
                if (!isDeleted)
                {
                    return NotFound("Question not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question with ID: {QuestionID}", questionID);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("InsertQuestion")]
        [ProducesResponseType(typeof(QuestionModel), statusCode: 201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertQuestion([FromBody] QuestionDTO question)
        {
            if (question == null)
            {
                return BadRequest("Question data is null");
            }
            try
            {
                string newQuestionID = await _checkExsistingID.GenerateUniqueIDForStringList(
                    _courseRepo.getALLQuestionID,
                    _generateID.GenerateQuestionID);
                QuestionModel newQuestion = new QuestionModel
                (
                    newQuestionID,
                    question.questionContent,
                    question.quizID
                );
                bool isInserted = await _courseRepo.InsertQuestion(newQuestion);
                if (!isInserted)
                {
                    return StatusCode(500, "Failed to insert question");
                }
                return CreatedAtAction(nameof(GetQuestionsByQuizID), new { quizID = question.quizID }, newQuestion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting question");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 

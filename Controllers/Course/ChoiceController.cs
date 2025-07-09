using Microsoft.AspNetCore.Mvc;
using E_learning.Model.Courses;
using E_learning.DTO.Course;
using E_learning.Services;
using E_learning.Repositories.Course;
using Microsoft.AspNetCore.Authorization;
using E_learning.Enums;

namespace E_learning.Controllers.Course
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChoiceController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseRepository _courseRepo;
        private readonly GenerateID _generateID;
        private readonly CheckExsistingID _checkExsistingID;
        public ChoiceController(ILogger<CourseController> logger, ICourseRepository courseRepo, GenerateID generateID, CheckExsistingID checkExsistingID)
        {
            _logger = logger;
            _courseRepo = courseRepo;
            _generateID = generateID;
            _checkExsistingID = checkExsistingID;
        }

        [HttpGet("GetChoicesByQuizID/{quizID}")]
        public async Task<IActionResult> GetChoicesByQuizID(string quizID)
        {
            try
            {
                List<ChoiceModel> choices = await _courseRepo.GetChoicesByQuizID(quizID);
                if (choices == null || choices.Count == 0)
                {
                    return NotFound("No choices found for the specified quiz ID");
                }
                return Ok(choices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving choices for quiz ID: {QuizID}", quizID);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("DeleteChoice/{choiceID}")]
        [Authorize(Roles = $"{nameof(UserRole.Lecturer)},{nameof(UserRole.Admin)}")]
        public async Task<IActionResult> DeleteChoice(string choiceID)
        {
            try
            {
                bool isDeleted = await _courseRepo.DeleteChoice(choiceID);
                if (!isDeleted)
                {
                    return NotFound("Choice not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting choice with ID: {ChoiceID}", choiceID);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("InsertChoice")]
        [Authorize(Roles = $"{nameof(UserRole.Lecturer)},{nameof(UserRole.Admin)}")]
        public async Task<IActionResult> InsertChoice([FromBody] ChoiceDTO choice)
        {
            if (choice == null)
            {
                return BadRequest("Choice data is null");
            }
            try
            {
                string newID = await _checkExsistingID.GenerateUniqueID(_courseRepo.getAllChoice, c => c.GetChoiceID(), _generateID.generateChoiceID);
                ChoiceModel choiceModel = new ChoiceModel(
                    newID,
                    choice.ChoiceText,
                    choice.IsCorrect,
                    choice.QuestionID
                );
                bool isInserted = await _courseRepo.InsertChoice(choiceModel);
                if (!isInserted)
                {
                    return BadRequest("Failed to insert choice");
                }
                return Ok(new { Message = "Choice inserted successfully.", Choice = choiceModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting choice");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

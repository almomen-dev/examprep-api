using Examprep.Application.DTOs;
using Examprep.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Examprep.API.Controllers
{
    /// <summary>
    /// Manage questions in the Examprep system
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/questions")]
    public class QuestionsController : ControllerBase
    {
        private readonly QuestionService _questionService;

        public QuestionsController(QuestionService questionService)
        {
            _questionService = questionService;
        }

        /// <summary>Get all questions</summary>
        /// <returns>List of all questions</returns>
        [HttpGet]
        public async Task<IActionResult> GetQuestion()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        /// <summary>Get a question by its ID</summary>
        /// <param name="id">The question ID</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById([FromRoute] int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);

            if (question == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Question Not Found",
                    Detail = $"Question with ID {id} was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(question);
        }

        /// <summary>Search questions by text</summary>
        /// <param name="search">Text to search for</param>
        [HttpGet("search")]
        public async Task<IActionResult> SearchQuestion([FromQuery] string search)
        {
            var result = await _questionService.SearchQuestionsAsync(search);
            return Ok(result);
        }

        /// <summary>Get paged questions</summary>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Items per page (default 10, max 100)</param>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _questionService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>Advanced query with search, sorting, and paging</summary>
        /// <param name="dto">Query parameters</param>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QuestionQueryDto dto)
        {
            var result = await _questionService.QueryAsync(dto);
            return Ok(result);
        }

        /// <summary>Create a new question (requires login)</summary>
        /// <param name="dto">Question data</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionCreateDto dto)
        {
            var userId = int.Parse(User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var question = await _questionService.CreateQuestionAsync(dto,userId);
            return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
        }

        /// <summary>Update an existing question (requires login)</summary>
        /// <param name="id">Question ID</param>
        /// <param name="dto">New question data</param>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionUpdateDto dto)
        {
            var updated = await _questionService.UpdateQuestionAsync(id, dto);

            if (!updated)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Question Not Found",
                    Detail = $"Question with ID {id} was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok();
        }

        /// <summary>Delete a question (Admin only)</summary>
        /// <param name="id">Question ID</param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var deleted = await _questionService.DeleteQuestionAsync(id);

            if (!deleted)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Question Not Found",
                    Detail = $"Question with ID {id} was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return NoContent();
        }
    }
}
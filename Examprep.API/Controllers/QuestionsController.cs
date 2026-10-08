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
        [HttpGet]
        public async Task<IActionResult> GetQuestion()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        /// <summary>Get a question by its ID</summary>
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
        [HttpGet("search")]
        public async Task<IActionResult> SearchQuestion([FromQuery] string search)
        {
            var result = await _questionService.SearchQuestionsAsync(search);
            return Ok(result);
        }

        /// <summary>Get paged questions</summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _questionService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>Advanced query with search, sorting, and paging</summary>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QuestionQueryDto dto)
        {
            var result = await _questionService.QueryAsync(dto);
            return Ok(result);
        }

        /// <summary>Create a new question (requires login)</summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionCreateDto dto)
        {
            var userId = int.Parse(User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var question = await _questionService.CreateQuestionAsync(dto, userId);
            return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
        }

        /// <summary>Update an existing question (requires login)</summary>
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

        [Authorize(Roles = "Admin")]
        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Only .csv files allowed" });

            var dtos = new List<QuestionCreateDto>();
            using var reader = new StreamReader(file.OpenReadStream());

            var header = await reader.ReadLineAsync();   // skip header
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = line.Split(',');
                if (cols.Length < 2) continue;

                dtos.Add(new QuestionCreateDto
                {
                    Text = cols[0].Trim(),
                    OptionA = cols.Length > 1 ? cols[1].Trim() : null,
                    OptionB = cols.Length > 2 ? cols[2].Trim() : null,
                    OptionC = cols.Length > 3 ? cols[3].Trim() : null,
                    OptionD = cols.Length > 4 ? cols[4].Trim() : null,
                    CorrectOption = cols.Length > 5 ? cols[5].Trim() : null,
                    Category = cols.Length > 6 ? cols[6].Trim() : null
                });
            }

            var userId = int.Parse(User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var result = await _questionService.BulkUploadAsync(dtos, userId);
            return Ok(result);
        }
    }
}
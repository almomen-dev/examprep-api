using Examprep.Application.DTOs;
using Examprep.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examprep.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/exam")]
    public class ExamController : ControllerBase
    {
        private readonly ExamService _service;

        public ExamController(ExamService service) { _service = service; }

        private int CurrentUserId => int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromQuery] int count = 10, [FromQuery] string? category = null)
        {
            var result = await _service.StartAsync(CurrentUserId, count, category);
            return Ok(result);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitExamDto dto)
        {
            var result = await _service.SubmitAsync(dto.AttemptId, dto.Answers);
            return Ok(result);
        }


        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            var result = await _service.GetHistoryAsync(CurrentUserId);
            return Ok(result);
        }


        [HttpGet("admin/results")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllResults()
        {
            var result = await _service.GetAllAttemptsAsync();
            return Ok(result);
        }
    }
}
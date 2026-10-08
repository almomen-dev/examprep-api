using Examprep.Application.DTOs;
using Examprep.Infrastructure.Data;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Examprep.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/stats")]
    [Authorize]
    public class StatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }

        // GET: api/v1/stats/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyStats()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var attempts = await _context.ExamAttempts
                .Where(a => a.UserId == userId && a.CompletedAt != null)
                .ToListAsync();

            var stats = new UserStatsDto
            {
                ExamsTaken = attempts.Count,
                QuestionsSolved = attempts.Sum(a => a.TotalQuestions),
                CorrectAnswers = attempts.Sum(a => a.CorrectCount),
                AverageScore = attempts.Count > 0
                    ? (int)attempts.Average(a => a.TotalQuestions > 0
                        ? (a.CorrectCount * 100.0 / a.TotalQuestions)
                        : 0)
                    : 0,
                BestScore = attempts.Count > 0
                    ? (int)attempts.Max(a => a.TotalQuestions > 0
                        ? (a.CorrectCount * 100.0 / a.TotalQuestions)
                        : 0)
                    : 0
            };

            return Ok(stats);
        }

        // GET: api/v1/stats/recent?count=3
        [HttpGet("recent")]
        public async Task<IActionResult> GetMyRecentAttempts(int count = 3)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var attempts = await _context.ExamAttempts
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.StartedAt)
                .Take(count)
                .Select(a => new RecentAttemptDto
                {
                    AttemptId = a.Id,
                    TotalQuestions = a.TotalQuestions,
                    CorrectCount = a.CorrectCount,
                    ScorePercent = a.TotalQuestions > 0
                        ? (int)(a.CorrectCount * 100.0 / a.TotalQuestions)
                        : 0,
                    CompletedAt = a.CompletedAt,
                    IsCompleted = a.CompletedAt != null
                })
                .ToListAsync();

            return Ok(attempts);
        }
    }
}
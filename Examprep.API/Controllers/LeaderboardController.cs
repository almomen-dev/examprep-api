using Examprep.Application.DTOs;
using Examprep.Infrastructure.Data;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Examprep.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/leaderboard")]
    [Authorize]
    public class LeaderboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaderboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaderboard()
        {
            var leaderboard = await _context.ExamAttempts
                .Include(a => a.User)
                .Where(a => a.CompletedAt != null && a.User != null)
                .GroupBy(a => a.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Email = g.First().User!.Email,
                    TotalAttempts = g.Count(),
                    AvgScore = (int)g.Average(a =>
                        a.TotalQuestions > 0
                            ? (a.CorrectCount * 100.0 / a.TotalQuestions)
                            : 0),
                    BestScore = (int)g.Max(a =>
                        a.TotalQuestions > 0
                            ? (a.CorrectCount * 100.0 / a.TotalQuestions)
                            : 0),
                    TotalCorrect = g.Sum(a => a.CorrectCount)
                })
                .OrderByDescending(x => x.AvgScore)
                .ThenByDescending(x => x.TotalAttempts)
                .ToListAsync();

            var ranked = leaderboard
                .AsEnumerable()   // ✅ safe in-memory ranking
                .Select((x, i) => new LeaderboardDto
                {
                    Rank = i + 1,
                    UserId = x.UserId,
                    Email = x.Email,
                    TotalAttempts = x.TotalAttempts,
                    AvgScore = x.AvgScore,
                    BestScore = x.BestScore,
                    TotalCorrect = x.TotalCorrect
                })
                .ToList();

            return Ok(ranked);
        }
    }
}
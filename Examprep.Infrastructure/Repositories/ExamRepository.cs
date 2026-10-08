using Examprep.Application.Repositories;
using Examprep.Domain.Model;
using Examprep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Examprep.Infrastructure.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;

        public ExamRepository(AppDbContext context) { _context = context; }

        public async Task<ExamAttempt> CreateAttemptAsync(ExamAttempt attempt)
        {
            _context.ExamAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<ExamAttempt?> GetAttemptAsync(int id)
        {
            return await _context.ExamAttempts
                .Include(e => e.Answers)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<ExamAnswer>> GetAnswersAsync(int attemptId)
        {
            return await _context.ExamAnswers
                .Where(a => a.ExamAttemptId == attemptId)
                .ToListAsync();
        }

        public async Task UpdateAttemptAsync(ExamAttempt attempt)
        {
            _context.ExamAttempts.Update(attempt);
            await _context.SaveChangesAsync();
        }

        public async Task AddAnswersAsync(List<ExamAnswer> answers)
        {
            _context.ExamAnswers.AddRange(answers);
            await _context.SaveChangesAsync();
        }


        public async Task<List<ExamAttempt>> GetAttemptsByUserAsync(int userId)
        {
            return await _context.ExamAttempts
                .AsNoTracking()
                .Where(e => e.UserId == userId && e.CompletedAt != null)
                .OrderByDescending(e => e.CompletedAt)
                .ToListAsync();
        }

        public async Task<List<ExamAttempt>> GetAllAttemptsAsync()
        {
            return await _context.ExamAttempts
                .AsNoTracking()
                .Include(e => e.User)
                .Where(e => e.CompletedAt != null)
                .OrderByDescending(e => e.CompletedAt)
                .ToListAsync();
        }
    }
}
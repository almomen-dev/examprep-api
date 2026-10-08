using Examprep.Application.DTOs;
using Examprep.Application.Repositories;
using Examprep.Domain.Model;
using Examprep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Examprep.Infrastructure.Repositories
{
    public class QuestionRepository: IQuestionRepository
    {
        private readonly AppDbContext _context;
        public QuestionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<QuestionResponseDto>> GetAllQuestionsAsync()
        {
            return await _context.Questions
                .AsNoTracking()
                .OrderBy(q => q.id)
                .Select(q => new QuestionResponseDto
                {
                    Id = q.id,
                    Text = q.text,
                    UserId = q.UserId,
                    UserEmail = q.User != null ? q.User.Email : null,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    CorrectOption = q.CorrectOption,
                    Category = q.Category
                })
                .ToListAsync();
        }

        public async Task<Question?> GetQuestionByIdAsync(int id)
        {
            return await _context.Questions
    .AsNoTracking()
    .Include(q => q.User)
    .FirstOrDefaultAsync(q => q.id == id);
        }

        public async Task AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(Question question)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<Question> items, int totalCount)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Questions.AsNoTracking().AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }
        public async Task<(List<QuestionResponseDto> items, int totalCount)> QueryAsync(
    string? search, string? category, string? sortBy, string? sortDir, int page, int pageSize)
        {
            var query = _context.Questions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(q => q.text.Contains(search));
            if (!string.IsNullOrWhiteSpace(category))                      
                query = query.Where(q => q.Category == category);

            query = sortBy?.ToLower() switch
            {
                "text" => sortDir == "desc"
                    ? query.OrderByDescending(q => q.text)
                    : query.OrderBy(q => q.text),
                _ => sortDir == "desc"
                    ? query.OrderByDescending(q => q.id)
                    : query.OrderBy(q => q.id),
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new QuestionResponseDto
                {
                    Id = q.id,
                    Text = q.text,
                    UserId = q.UserId,
                    UserEmail = q.User != null ? q.User.Email : null,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    CorrectOption = q.CorrectOption,
                    Category = q.Category
                })
                .ToListAsync();

            return (items, totalCount);
        }


        public async Task AddQuestionWithCounterAsync(Question question, int userId)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Questions.Add(question);
                    await _context.SaveChangesAsync();

                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        user.QuestionCount += 1;
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }


        public async Task AddRangeAsync(List<Question> questions)
        {
            _context.Questions.AddRange(questions);
            await _context.SaveChangesAsync();
        }


        public async Task<List<Question>> GetRandomAsync(int count, string? category = null)
        {
            var query = _context.Questions
                .AsNoTracking()
                .Where(q => q.CorrectOption != null && q.OptionA != null);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(q => q.Category == category);

            return await query
                .OrderBy(q => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Question>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Questions
                .AsNoTracking()
                .Where(q => ids.Contains(q.id))
                .ToListAsync();
        }
    }
}

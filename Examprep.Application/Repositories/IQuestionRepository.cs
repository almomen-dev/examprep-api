using Examprep.Application.DTOs;
using Examprep.Domain.Model;

namespace Examprep.Application.Repositories
{
    public interface IQuestionRepository
    {
        Task<List<QuestionResponseDto>> GetAllQuestionsAsync();

        Task<Question?> GetQuestionByIdAsync(int id);

        Task AddQuestionAsync(Question question);

        Task UpdateQuestionAsync(Question question);

        Task DeleteQuestionAsync(Question question);
        Task<(List<Question> items, int totalCount)> GetPagedAsync(int page, int pageSize);

        Task<(List<QuestionResponseDto> items, int totalCount)> QueryAsync(
     string? search, string? sortBy, string? sortDir, int page, int pageSize);


        Task AddQuestionWithCounterAsync(Question question, int userId);
    }
}
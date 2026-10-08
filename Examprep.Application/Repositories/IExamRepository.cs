using Examprep.Domain.Model;

namespace Examprep.Application.Repositories
{
    public interface IExamRepository
    {
        Task<ExamAttempt> CreateAttemptAsync(ExamAttempt attempt);
        Task<ExamAttempt?> GetAttemptAsync(int id);
        Task<List<ExamAnswer>> GetAnswersAsync(int attemptId);
        Task UpdateAttemptAsync(ExamAttempt attempt);
        Task AddAnswersAsync(List<ExamAnswer> answers);
        Task<List<ExamAttempt>> GetAttemptsByUserAsync(int userId);
        Task<List<ExamAttempt>> GetAllAttemptsAsync();
    }
}
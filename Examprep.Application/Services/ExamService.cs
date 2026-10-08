using Examprep.Application.DTOs;
using Examprep.Application.Repositories;
using Examprep.Domain.Model;

namespace Examprep.Application.Services
{
    public class ExamService
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IExamRepository _examRepo;

        public ExamService(IQuestionRepository questionRepo, IExamRepository examRepo)
        {
            _questionRepo = questionRepo;
            _examRepo = examRepo;
        }

        public async Task<StartExamDto> StartAsync(int userId, int count = 10, string? category = null)
        {
            var questions = await _questionRepo.GetRandomAsync(count, category);
            if (questions.Count == 0)
                throw new InvalidOperationException("No questions available for exam.");

            var attempt = new ExamAttempt
            {
                UserId = userId,
                TotalQuestions = questions.Count
            };
            await _examRepo.CreateAttemptAsync(attempt);

            return new StartExamDto
            {
                AttemptId = attempt.Id,
                Questions = questions.Select(q => new ExamQuestionDto
                {
                    Id = q.id,
                    Text = q.text,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD
                }).ToList()
            };
        }

        public async Task<ExamResultDto> SubmitAsync(int attemptId, List<ExamAnswerDto> submitted)
        {
            var attempt = await _examRepo.GetAttemptAsync(attemptId)
                ?? throw new InvalidOperationException("Attempt not found.");

            var questionIds = submitted.Select(a => a.QuestionId).ToList();
            var questions = await _questionRepo.GetByIdsAsync(questionIds);
            var questionMap = questions.ToDictionary(q => q.id);

            var result = new ExamResultDto
            {
                AttemptId = attempt.Id,
                TotalQuestions = attempt.TotalQuestions
            };

            var answers = new List<ExamAnswer>();

            foreach (var sub in submitted)
            {
                if (!questionMap.TryGetValue(sub.QuestionId, out var q)) continue;

                bool correct = string.Equals(
                    sub.SelectedOption, q.CorrectOption,
                    StringComparison.OrdinalIgnoreCase);

                answers.Add(new ExamAnswer
                {
                    ExamAttemptId = attempt.Id,
                    QuestionId = q.id,
                    SelectedOption = sub.SelectedOption?.ToUpper() ?? "",
                    IsCorrect = correct
                });

                result.Items.Add(new ExamResultItemDto
                {
                    QuestionId = q.id,
                    QuestionText = q.text,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    SelectedOption = sub.SelectedOption?.ToUpper(),
                    CorrectOption = q.CorrectOption,
                    IsCorrect = correct
                });

                if (correct) result.CorrectCount++;
            }

            await _examRepo.AddAnswersAsync(answers);

            attempt.CorrectCount = result.CorrectCount;
            attempt.CompletedAt = DateTime.UtcNow;
            await _examRepo.UpdateAttemptAsync(attempt);

            result.ScorePercent = (int)Math.Round(
                (double)result.CorrectCount / result.TotalQuestions * 100);

            return result;
        }

        public async Task<List<ExamHistoryDto>> GetHistoryAsync(int userId)
        {
            var attempts = await _examRepo.GetAttemptsByUserAsync(userId);
            return attempts.Select(a => new ExamHistoryDto
            {
                AttemptId = a.Id,
                TotalQuestions = a.TotalQuestions,
                CorrectCount = a.CorrectCount,
                ScorePercent = a.TotalQuestions > 0
                    ? (int)Math.Round((double)a.CorrectCount / a.TotalQuestions * 100)
                    : 0,
                CompletedAt = a.CompletedAt ?? a.StartedAt
            }).ToList();
        }




        public async Task<List<AdminExamResultDto>> GetAllAttemptsAsync()
        {
            var attempts = await _examRepo.GetAllAttemptsAsync();
            return attempts.Select(a => new AdminExamResultDto
            {
                AttemptId = a.Id,
                UserId = a.UserId,
                UserEmail = a.User?.Email ?? "unknown",
                TotalQuestions = a.TotalQuestions,
                CorrectCount = a.CorrectCount,
                ScorePercent = a.TotalQuestions > 0
                    ? (int)Math.Round((double)a.CorrectCount / a.TotalQuestions * 100)
                    : 0,
                CompletedAt = a.CompletedAt ?? a.StartedAt
            }).ToList();
        }
    }
}
namespace Examprep.Application.DTOs
{
    public class SubmitExamDto
    {
        public int AttemptId { get; set; }
        public List<ExamAnswerDto> Answers { get; set; } = new();
    }

    public class ExamAnswerDto
    {
        public int QuestionId { get; set; }
        public string SelectedOption { get; set; } = string.Empty;
    }

    public class ExamResultDto
    {
        public int AttemptId { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int ScorePercent { get; set; }
        public List<ExamResultItemDto> Items { get; set; } = new();
    }

    public class ExamResultItemDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? SelectedOption { get; set; }
        public string? CorrectOption { get; set; }
        public bool IsCorrect { get; set; }
    }
}
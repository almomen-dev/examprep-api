namespace Examprep.Application.DTOs
{
    public class StartExamDto
    {
        public int AttemptId { get; set; }
        public List<ExamQuestionDto> Questions { get; set; } = new();
    }

    public class ExamQuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
    }
}
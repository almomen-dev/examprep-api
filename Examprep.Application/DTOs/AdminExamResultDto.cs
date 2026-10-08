namespace Examprep.Application.DTOs
{
    public class AdminExamResultDto
    {
        public int AttemptId { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int ScorePercent { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
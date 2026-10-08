namespace Examprep.Application.DTOs
{
    public class RecentAttemptDto
    {
        public int AttemptId { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int ScorePercent { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}
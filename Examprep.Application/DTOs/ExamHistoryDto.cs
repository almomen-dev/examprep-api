namespace Examprep.Application.DTOs
{
    public class ExamHistoryDto
    {
        public int AttemptId { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int ScorePercent { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
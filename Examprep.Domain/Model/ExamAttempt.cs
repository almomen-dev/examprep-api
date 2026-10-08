namespace Examprep.Domain.Model
{
    public class ExamAttempt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public List<ExamAnswer> Answers { get; set; } = new();
    }
}
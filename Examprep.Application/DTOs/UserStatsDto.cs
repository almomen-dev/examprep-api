namespace Examprep.Application.DTOs
{
    public class UserStatsDto
    {
        public int ExamsTaken { get; set; }
        public int QuestionsSolved { get; set; }
        public int CorrectAnswers { get; set; }
        public int AverageScore { get; set; }
        public int BestScore { get; set; }
    }
}
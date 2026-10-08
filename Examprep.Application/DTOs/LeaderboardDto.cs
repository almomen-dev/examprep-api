namespace Examprep.Application.DTOs
{
    public class LeaderboardDto
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int TotalAttempts { get; set; }
        public int AvgScore { get; set; }
        public int BestScore { get; set; }
        public int TotalCorrect { get; set; }
    }
}
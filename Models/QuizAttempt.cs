using Quizard.Models.Shared;

namespace Quizard.Models
{
    public class QuizAttempt : BaseModel
    {
        public string UserName { get; set; } = string.Empty;
        public double Score { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public ICollection<QuizAnswer> QuizAnswers { get; set; } = [];

        public QuizAttemptStatus Status =>
            CompletedOn.HasValue ? QuizAttemptStatus.Completed : QuizAttemptStatus.InProgress;

        // Navigation properties
        public Guid QuizId { get; set; }
        public Quiz? Quiz { get; set; } = null!;

        // public Guid UserId { get; set; }
        // public User? User { get; set; } = null!;
    }
}

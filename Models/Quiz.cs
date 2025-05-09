using Quizard.Models.Shared;

namespace Quizard.Models
{
    public class Quiz : BaseModel
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan? TimeLimit { get; set; }
        public bool IsRandomOrder { get; set; } = false;

        // public Guid OwnerId { get; set; }
        // public User? Owner { get; set; } = null!;

        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = [];
    }
}

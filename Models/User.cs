using Quizard.Models.Shared;

namespace Quizard.Models
{
    public class User : BaseModel
    {
        public required string Username { get; set; }

        public ICollection<Quiz> MyQuizzes { get; set; } = [];
        public ICollection<QuizAttempt> MyQuizAttempts { get; set; } = [];
    }
}

using Quizard.Models.Shared;

namespace Quizard.Models
{
    public class QuizAnswer : BaseModel
    {
        public Guid QuizAttemptId { get; set; }
        public QuizAttempt? QuizAttempt { get; set; } = null!;

        public Guid QuestionId { get; set; }
        public Question? Question { get; set; } = null!;

        public int Order { get; set; }

        public ICollection<QuizAnswerChoice> AnswerChoices { get; set; } = [];
    }
}
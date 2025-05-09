namespace Quizard.Models
{
    public class QuizAnswerChoice
    {
        public Guid QuizAnswerId { get; set; }
        public QuizAnswer? QuizAnswer { get; set; } = null!;
        public Guid ChoiceId { get; set; }
        public Choice? Choice { get; set; } = null!;
    }
}

using Quizard.Models;

namespace Quizard.ViewModels
{
    public class AnswerViewModel
    {
        public QuizAnswer QuizAnswer { get; set; } = null!;
        public bool IsCorrect { get; set; }
    }
}

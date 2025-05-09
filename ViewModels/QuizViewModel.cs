using System.ComponentModel.DataAnnotations;

namespace Quizard.ViewModels
{
    // View model for creating/editing a quiz
    public class QuizViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Quiz Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Quiz Description")]
        public string? Description { get; set; }

        [Display(Name = "No. of Questions")]
        public int QuestionCount => Questions?.Count ?? 0;

        [Display(Name = "Time Limit (minutes)")]
        [Range(1, 120, ErrorMessage = "Time limit must be between 1 and 120 minutes.")]
        public double? TimeLimit { get; set; }

        [Display(Name = "Randomise question order?")]
        public bool IsRandomOrder { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = [];
    }

    public class QuestionViewModel
    {
        public Guid QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsMultiSelect { get; set; }

        public List<Guid> SelectedChoiceIds { get; set; } = [];

        public List<ChoiceViewModel> Choices { get; set; } = [];
    }

    public class ChoiceViewModel
    {
        public Guid ChoiceId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}

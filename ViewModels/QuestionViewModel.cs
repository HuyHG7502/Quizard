using System.ComponentModel.DataAnnotations;

namespace Quizard.ViewModels
{
    // View model for creating/editing a quiz question
    public class QuestionFormViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Question Text")]
        public string Text { get; set; } = string.Empty;

        [Display(Name = "Multiple correct answers?")]
        public bool IsMultiSelect { get; set; } = false;

        public List<ChoiceFormViewModel> Choices { get; set; } = new()
        {
            new() { IsCorrect = true },
            new(),
            new(),
            new()
        };
    }

    public class ChoiceFormViewModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}

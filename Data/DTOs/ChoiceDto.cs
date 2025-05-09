namespace Quizard.Data.DTOs
{
    public class ChoiceDto
    {
        public string Text { get; set; } = default!;
        public bool IsCorrect { get; set; }
    }
}

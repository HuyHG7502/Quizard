namespace Quizard.Data.DTOs
{
    public class QuestionDto
    {
        public string Text { get; set; } = default!;
        public bool IsMultiSelect { get; set; } = false;
        public List<ChoiceDto> Choices { get; set; } = [];
    }
}

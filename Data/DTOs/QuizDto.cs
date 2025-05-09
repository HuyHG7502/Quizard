namespace Quizard.Data.DTOs
{
    public class QuizDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string? TimeLimit { get; set; }
        public List<QuestionDto> Questions { get; set; } = [];
    }
}

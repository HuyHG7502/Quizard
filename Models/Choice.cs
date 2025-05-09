using Quizard.Models.Shared;

namespace Quizard.Models
{
    public class Choice : BaseModel
    {
        public required string Text { get; set; }
        public bool IsCorrect { get; set; }

        // Navigation properties
        public Guid QuestionId { get; set; }
        public Question? Question { get; set; } = null!;
    }
}

using Quizard.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizard.Models
{
    public class Question : BaseModel
    {
        public required string Text { get; set; }
        public bool IsMultiSelect { get; set; } = false;
        public ICollection<Choice> Choices { get; set; } = [];

        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
    }
}

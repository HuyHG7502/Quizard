namespace Quizard.ViewModels
{
    public class AttemptViewModel
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QuestionCount => Questions?.Count ?? 0;
        public double? TimeLimit { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = [];
        public Dictionary<Guid, List<Guid>> Answers { get; set; } = new();
    }
}

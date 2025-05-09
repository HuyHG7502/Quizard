using Quizard.Data;
using Quizard.Data.DTOs;
using Quizard.Models;
using System.Text.Json;

namespace Quizard.Helpers
{
    public static class SeederService
    {
        public static async Task SeedAsync(AppDbContext context, string jsonPath)
        {
            // Db already seeded
            if (context.Quizzes.Any()) return;
        
            var json = await File.ReadAllTextAsync(jsonPath);
            var quizzes = JsonSerializer.Deserialize<List<QuizDto>>(json);

            if (quizzes == null)
            {
                Console.WriteLine("Deserialization failed. No quizzes were added.");
                return;
            }

            foreach (var quizDto in quizzes!)
            {
                var quiz = new Quiz
                {
                    Id = Guid.NewGuid(),
                    Title = quizDto.Title,
                    Description = quizDto.Description,
                    TimeLimit = TimeSpan.TryParse(quizDto.TimeLimit, out var timeLimit) ? timeLimit : TimeSpan.Zero,
                    QuizQuestions = []
                };

                var questions = new List<Question>();
                foreach (var qDto in quizDto.Questions)
                {
                    var question = new Question
                    {
                        Id = Guid.NewGuid(),
                        Text = qDto.Text,
                        IsMultiSelect = qDto.IsMultiSelect,
                        Choices = qDto.Choices.Select(c => new Choice
                        {
                            Id = Guid.NewGuid(),
                            Text = c.Text,
                            IsCorrect = c.IsCorrect
                        }).ToList(),
                    };
                    questions.Add(question);
                }

                context.Questions.AddRange(questions);
                await context.SaveChangesAsync();

                int order = 0;
                foreach (var question in questions)
                {
                    quiz.QuizQuestions.Add(new QuizQuestion
                    {
                        QuizId = quiz.Id,
                        QuestionId = question.Id,
                        Order = order++,
                    });
                }


                context.Quizzes.Add(quiz);
            }

            await context.SaveChangesAsync();
        }
    }
}

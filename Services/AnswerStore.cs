using Microsoft.Extensions.Caching.Distributed;
using Quizard.Interfaces;
using Quizard.Pages.Quizzes;
using System.Text.Json;

namespace Quizard.Services
{
    public class AnswerStore : IAnswerStore
    {
        private readonly IDistributedCache _cache;

        public AnswerStore(IDistributedCache cache)
            => _cache = cache;

        private string GetKey(Guid attemptId)
            => $"quiz-answers:{attemptId}";

        public async Task SaveAsync(QuizAttemptSession session)
        {
            var json = JsonSerializer.Serialize(session.Answers);
            await _cache.SetStringAsync(GetKey(session.AttemptId), json);
        }

        public async Task<Dictionary<Guid, List<Guid>>?> LoadAsync(Guid attemptId)
        {
            var json = await _cache.GetStringAsync(GetKey(attemptId));
            return json == null
                ? null
                : JsonSerializer.Deserialize<Dictionary<Guid, List<Guid>>>(json);
        }

        public Task RemoveAsync(Guid attemptId)
        {
            return _cache.RemoveAsync(GetKey(attemptId));
        }
    }
}

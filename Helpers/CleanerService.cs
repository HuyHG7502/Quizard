using Quizard.Interfaces;

namespace Quizard.Helpers
{
    // Cleaner Service to update expired attempts
    public class CleanerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CleanerService> _logger;

        public CleanerService(IServiceScopeFactory scopeFactory, ILogger<CleanerService> logger)
            => (_scopeFactory, _logger) = (scopeFactory, logger);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Cleaner Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var takeQuizService = scope.ServiceProvider.GetRequiredService<ITakeQuizService>();

                // Clean up incomplete quiz attempts that have exceeded their time limit
                var attempts = await takeQuizService.GetStaleAttemptsAsync();

                foreach (var attempt in attempts)
                {
                    await takeQuizService.SubmitQuizAsync(attempt.Id);
                    _logger.LogInformation("Auto-submitted Attempt {AttemptId} for Quiz {QuizId}, User '{User}', at {ExpiresAt}",
                        attempt.Id, attempt.QuizId, attempt.UserName, attempt.ExpiresAt?.ToLocalTime().ToString("g"));
                }

                var nextExpiresAt = await takeQuizService.GetNextExpiryAsync();
                var now = DateTime.UtcNow;

                TimeSpan delay = nextExpiresAt.HasValue && nextExpiresAt > now
                    ? nextExpiresAt.Value - now
                    : TimeSpan.FromMinutes(1);

                // Prevent sleeping too short or long
                if (delay < TimeSpan.FromSeconds(30)) delay = TimeSpan.FromSeconds(30);

                _logger.LogInformation("Next cleanup check scheduled after {Delay} at {ResumeTime}.", 
                    delay.TotalSeconds, now.Add(delay).ToLocalTime());

                await Task.Delay(delay, stoppingToken);
            }

            _logger.LogInformation("Cleaner Service is stopping.");
        }
    }
}

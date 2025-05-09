using Microsoft.EntityFrameworkCore;
using Quizard.Models;
using Quizard.Models.Shared;

namespace Quizard.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<QuizAnswerChoice> QuizAnswerChoices { get; set; }
        
        // public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Automatically apply all IEntityTypeConfiguration configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<BaseModel>();
            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = utcNow;
                    entry.Entity.ModifiedOn = utcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedOn = utcNow;
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseModel>();
            var utcNow = DateTime.UtcNow;
            
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = utcNow;
                    entry.Entity.ModifiedOn = utcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedOn = utcNow;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

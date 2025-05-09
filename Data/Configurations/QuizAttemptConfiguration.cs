using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Quizard.Models;

namespace Quizard.Data.Configurations
{
    public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
    {
        public void Configure(EntityTypeBuilder<QuizAttempt> builder)
        {
            builder.ToTable("QuizAttempts");
            builder.HasKey(qa => qa.Id);
            builder.Property(qa => qa.UserName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(qa => qa.Score)
                .IsRequired();
            builder.Property(qa => qa.StartedOn)
                .IsRequired();
            builder.Property(qa => qa.CompletedOn)
                .IsRequired(false);

            builder.HasOne(qa => qa.Quiz)
                .WithMany(q => q.QuizAttempts)
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(qa => qa.QuizAnswers)
                .WithOne(qa => qa.QuizAttempt)
                .HasForeignKey(qa => qa.QuizAttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            /*
            builder.HasOne(qa => qa.User)
                .WithMany(u => u.MyQuizAttempts)
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            */
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Models;

namespace Quizard.Data.Configurations
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.ToTable("Quizzes");
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(q => q.Description)
                .HasMaxLength(1000);
            builder.Property(q => q.TimeLimit)
                .HasColumnType("time")
                .IsRequired(false);
            builder.Property(q => q.IsRandomOrder)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasMany(q => q.QuizQuestions)
                .WithOne(qq => qq.Quiz)
                .HasForeignKey(qq => qq.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.QuizAttempts)
                .WithOne(qa => qa.Quiz)
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
            builder.HasOne(q => q.Owner)
                .WithMany(u => u.MyQuizzes)
                .HasForeignKey(q => q.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            */
        }
    }
}
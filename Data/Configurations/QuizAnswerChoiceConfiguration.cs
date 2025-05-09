using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Models;

namespace Quizard.Data.Configurations
{
    public class QuizAnswerChoiceConfiguration : IEntityTypeConfiguration<QuizAnswerChoice>
    {
        public void Configure(EntityTypeBuilder<QuizAnswerChoice> builder)
        {
            builder.ToTable("QuizAnswerChoices");
            builder.HasKey(qac => new { qac.QuizAnswerId, qac.ChoiceId });
            
            builder.HasOne(qac => qac.QuizAnswer)
                .WithMany(qa => qa.AnswerChoices)
                .HasForeignKey(qac => qac.QuizAnswerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(qac => qac.Choice)
                .WithMany()
                .HasForeignKey(qac => qac.ChoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

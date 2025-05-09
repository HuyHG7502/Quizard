using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Quizard.Models;

namespace Quizard.Data.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Text)
                .IsRequired();
            builder.Property(q => q.IsMultiSelect)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasMany(q => q.Choices)
                .WithOne(c => c.Question)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.QuizQuestions)
                .WithOne(qq => qq.Question)
                .HasForeignKey(qq => qq.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

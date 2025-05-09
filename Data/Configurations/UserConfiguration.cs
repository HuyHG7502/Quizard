using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizard.Models;

namespace Quizard.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);
            
            /*
            builder.HasMany(u => u.MyQuizzes)
                .WithOne(q => q.Owner)
                .HasForeignKey(q => q.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.MyQuizAttempts)
                .WithOne(qa => qa.User)
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            */
        }
    }
}

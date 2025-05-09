using System.ComponentModel.DataAnnotations;

namespace Quizard.Models.Shared
{
    public abstract class BaseModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}

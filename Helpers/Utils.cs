using Quizard.Models.Shared;

namespace Quizard.Helpers
{
    public static class Utils
    {
        public static void Initialize<T>(T entity) where T : BaseModel
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedOn = DateTime.UtcNow;
            entity.ModifiedOn = DateTime.UtcNow;
        }

        public static void Update<T>(T entity) where T : BaseModel
        {
            entity.ModifiedOn = DateTime.UtcNow;
        }

        public static void InitializeCollection<T>(IEnumerable<T> entities) where T : BaseModel
        {
            foreach (var entity in entities)
            {
                Initialize(entity);
            }
        }

        public static void UpdateCollection<T>(IEnumerable<T> entities) where T : BaseModel
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }
    }
}

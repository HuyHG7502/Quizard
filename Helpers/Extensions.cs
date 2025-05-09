using System.Text.Json;

namespace Quizard.Helpers
{
    public static class Extensions
    {   
        public static List<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return [.. source.OrderBy(_ => Guid.NewGuid())];
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        }
    }
}

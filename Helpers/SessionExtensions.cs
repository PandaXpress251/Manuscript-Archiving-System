using Newtonsoft.Json;

namespace Thesis_Capstone_Archive.Helpers
{
    public static class SessionExtensions
    {
        // Store complex objects in session by serializing to JSON
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Retrieve complex objects from session by deserializing from JSON
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}

using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text;

namespace Research_Repository_Utility
{
    public static class SessionExtensions
    {
        //Allows session to store complex objects
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        //Allows session to retrieve complex objects
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}

using System.Text.Json;

namespace CritiQuest2.Server.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Extension method for JsonSerializer.Deserialize without optional parameters
        /// Safe to use in LINQ expressions/expression trees
        /// </summary>
        public static T? DeserializeJson<T>(this string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return default(T);

            return JsonSerializer.Deserialize<T>(jsonString, (JsonSerializerOptions?)null);
        }

        /// <summary>
        /// Safe string array deserializer - returns empty array if null/empty
        /// </summary>
        public static string[] DeserializeStringArray(this string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return Array.Empty<string>();

            try
            {
                return JsonSerializer.Deserialize<string[]>(jsonString, (JsonSerializerOptions?)null)
                       ?? Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Safe object deserializer - returns null if fails
        /// </summary>
        public static object? DeserializeObject(this string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return null;

            try
            {
                return JsonSerializer.Deserialize<object>(jsonString, (JsonSerializerOptions?)null);
            }
            catch
            {
                return null;
            }
        }
    }
}

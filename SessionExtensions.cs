using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class SessionExtensions
{
    public static void SetObject<T>(this ISession session, string key, T value)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };
        session.SetString(key, JsonSerializer.Serialize(value, options));
    }

    public static T? GetObject<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        if (value == null) return default;

        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        return JsonSerializer.Deserialize<T>(value, options);
    }
}

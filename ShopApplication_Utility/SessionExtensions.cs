using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ShopApplication_Utility;

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer .Serialize(value, typeof(T)));
    }
    public static T? Get<T>(this ISession session, string key)
    {
        var valueJson = session.GetString(key);
        return valueJson == null ? default : JsonSerializer.Deserialize<T>(valueJson);
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BotToolBox.Util;

public class JsonSerializeUtil
{
    //ignore condition json options
    public static JsonSerializerOptions IgnoreCaseJsonOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    public static string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, IgnoreCaseJsonOptions);
    }

    public static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, IgnoreCaseJsonOptions);
    }
}
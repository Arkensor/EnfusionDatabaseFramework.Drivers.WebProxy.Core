using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core;

public class Vector : List<double>
{
}

[JsonConverter(typeof(TypenameConverter))]
public record Typename(string ClassName)
{
    public override string? ToString()
    {
        return ClassName;
    }

    public static implicit operator string(Typename typename)
    {
        return typename.ClassName;
    }

    public static implicit operator Typename(string type)
    {
        return new Typename(type);
    }
}

public class TypenameConverter : JsonConverter<Typename>
{
    public override Typename Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()!;
    }

    public override void Write(Utf8JsonWriter writer, Typename value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

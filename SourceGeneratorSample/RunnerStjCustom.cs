using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using SourceGeneratorSample.Generators;

namespace SourceGeneratorSample;

public class RunnerStjCustom : IRunner
{
    public const int PropertyCount = 100; // OK
    //public const int PropertyCount = 2480; // OK
    //public const int PropertyCount = 2481; // CS8078 (Too long expression)

    public void Run()
    {
        var json = $"{{ {string.Join(", ", Enumerable.Range(0, PropertyCount).Select(i => $"\"V{i}\": {i}"))} }}";
        var obj = JsonSerializer.Deserialize<FooStjCustom>(json, new JsonSerializerOptions
        {
            Converters = { new FooConverterFactory() }
        });
        Console.WriteLine(obj);
    }
}

[PropertyCount(RunnerStjCustom.PropertyCount)]
public partial record FooStjCustom;


public class FooConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(FooStjCustom);
    }

    public override JsonConverter CreateConverter(
        Type type,
        JsonSerializerOptions options)
    {
        return new FooConverter(options);
    }
}

public class FooConverter : JsonConverter<FooStjCustom>
{
    public FooConverter(JsonSerializerOptions options)
    {
    }

    public override FooStjCustom Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var dictionary = new Dictionary<string, int>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                var ctor = typeof(FooStjCustom).GetConstructors().First();
                var paramPositions = ctor.GetParameters().ToDictionary(p => p.Name!, p => p.Position);

                return (FooStjCustom?)Activator.CreateInstance(
                    type: typeof(FooStjCustom),
                    bindingAttr: BindingFlags.CreateInstance,
                    binder: null,
                    args: dictionary.OrderBy(kv => paramPositions[kv.Key]).Select(kv => (object)kv.Value).ToArray(),
                    culture: null)
                    ?? throw new JsonException();
            }

            // Get the field name
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            var propertyName = reader.GetString() ?? throw new JsonException();

            // Get the value.
            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException();
            var value = reader.GetInt32();

            dictionary.Add(propertyName, value);
        }

        throw new JsonException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        FooStjCustom foo,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
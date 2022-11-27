using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SourceGeneratorSample
{
    public class FooConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Foo);
        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {
            return new FooConverter(options);
        }
    }
    
    public class FooConverter : JsonConverter<Foo>
    {
        public FooConverter(JsonSerializerOptions options)
        {
        }

        public override Foo Read(
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
                    var ctor = typeof(Foo).GetConstructors().First();
                    var paramPositions = ctor.GetParameters().ToDictionary(p => p.Name!, p => p.Position);

                    return (Foo?)Activator.CreateInstance(
                        type: typeof(Foo),
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
            Foo foo,
            JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
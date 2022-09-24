using System.Text.Json;
using System.Text.Json.Serialization;

namespace AbiParser
{
    public class StatCounterJsonConverter : JsonConverter<StatCounter>
    {
        public override StatCounter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, StatCounter value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case VariableNameStatCounter _:
                    writer.WriteStartObject();
                    writer.WriteString("varName", value.Name);
                    writer.WriteNumber("occurance", value.Occurence);
                    writer.WriteNumber("likelyhood", value.CalculateLikelyhood());
                    writer.WriteEndObject();
                    break;
                case InputVariableStatCounter _:
                    writer.WriteStartObject();
                    writer.WriteString("inputVarType", value.Name);
                    writer.WriteStartArray("varNameColl");
                    foreach (var item in value.Child.Values)
                    {
                        JsonSerializer.Serialize(writer, item, options);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                case FunctionStatCounter _:
                    writer.WriteStartObject();
                    writer.WriteString("sigType", "Function");
                    writer.WriteString("name", value.Name);
                    writer.WriteNumber("occurance", value.Occurence);
                    writer.WriteNumber("likelyhood", value.CalculateLikelyhood());
                    writer.WriteStartArray("inputVarTypeColl");
                    foreach (var item in value.Child.Values)
                    {
                        JsonSerializer.Serialize(writer, item, options);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                case EventStatCounter _:
                    writer.WriteStartObject();
                    writer.WriteString("sigType", "Event");
                    writer.WriteString("name", value.Name);
                    writer.WriteNumber("occurance", value.Occurence);
                    writer.WriteNumber("likelyhood", value.CalculateLikelyhood());
                    writer.WriteStartArray("inputVarTypeColl");
                    foreach (var item in value.Child.Values)
                    {
                        JsonSerializer.Serialize(writer, item, options);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                case FourByteStatCounter _:
                    writer.WriteStartObject();
                    writer.WriteString("selector", "0x" + value.Name);
                    writer.WriteStartArray("sigColl");
                    foreach (var item in value.Child.Values)
                    {
                        JsonSerializer.Serialize(writer, item, options);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                default:    //the root statcounter of base type StatCounter
                    writer.WriteStartArray();
                    foreach (var item in value.Child.Values)
                    {
                        JsonSerializer.Serialize(writer, item, options);
                    }
                    writer.WriteEndArray();
                    break;
            }
        }
    }
}

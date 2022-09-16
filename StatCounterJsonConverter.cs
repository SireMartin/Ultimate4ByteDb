using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

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
                    writer.WriteNumber("occurance", value.Occurance);
                    writer.WriteNumber("likelyhood", .789);
                    writer.WriteEndObject();
                    break;
                case InputVariableStatCounter _:
                    writer.WriteStartObject();
                    writer.WriteString("varType", value.Name);
                    writer.WriteStartArray("varNames");
                    foreach (var item in value.Child.Values)
                    {
                        JsonSerializer.Serialize(writer, item, options);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                case FunctionStatCounter _:
                    writer.WriteStartObject();
                    writer.WriteString("fctSig", value.Name);
                    writer.WriteNumber("occurance", value.Occurance);
                    writer.WriteNumber("likelyhood", .123);
                    writer.WriteStartArray("inputVars");
                    foreach (var item in value.Child.Values)
                    {
                        JsonSerializer.Serialize(writer, item, options);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                case FourByteStatCounter _:
                    writer.WriteString("fctSelector", value.Name);
                    writer.WriteNumber("occurance", value.Occurance);
                    writer.WriteStartArray("fctSigs");
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

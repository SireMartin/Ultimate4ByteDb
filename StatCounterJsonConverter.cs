using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (value.Type == StatCounter.StatCounterType.VariableName)
            {
                writer.WriteStartObject();
                writer.WriteString("varName", value.Name);
                writer.WriteNumber("occurance", value.Occurance);
                writer.WriteNumber("likelyhood", .789);
                writer.WriteEndObject();
            }
            else if (value.Type == StatCounter.StatCounterType.InputVariable)
            {
                writer.WriteStartObject();
                writer.WriteString("varType", value.Name);
                writer.WriteStartArray("varNames");
                foreach (var item in value.Child.Values)
                {
                    JsonSerializer.Serialize(writer, item, options);
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            else if (value.Type == StatCounter.StatCounterType.FunctionSignature)
            {
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
            }
            else if (value.Type == StatCounter.StatCounterType.FourByteCode)
            {
                writer.WriteString("fctSelector", value.Name);
                writer.WriteNumber("occurance", value.Occurance);
                writer.WriteStartArray("fctSigs");
                foreach (var item in value.Child.Values)
                {
                    JsonSerializer.Serialize(writer, item, options);
                }
                writer.WriteEndArray();
            }
            else
            {
                throw new NotImplementedException($"Type {value.Type} not implemented for json serialization");
            }
        }
    }
}

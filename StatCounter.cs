using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AbiParser
{
    public class StatCounter
    {
        public enum StatCounterType
        {
            FourByteCode,
            FunctionSignature,
            InputVariable,
            VariableName
        };

        public StatCounterType Type { get; set; }
        public string? Name { get; set; }
        public Dictionary<string, StatCounter> Child { get; set; } = new();

        //we do not add non-existing fourbytes (preferred for memory), we generate them afterwards when filling the redis
        public int Occurance { get; set; } = 1;
    }
}

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
        public string? Name;
        public Dictionary<string, StatCounter> Child { get; set; }

        //we do not add non-existing fourbytes (preferred for memory), we generate them afterwards when filling the redis
        public int Occurance;

        public StatCounter(string argName)
        {
            Name = argName;
            Occurance = 1;
            Child = new Dictionary<string, StatCounter>();
        }

        public virtual void IncrementOccurance() => ++Occurance;
    }

    internal class FourByteStatCounter : StatCounter
    {
        public FourByteStatCounter(string argName) : base(argName) { }
    }

    internal class FunctionStatCounter : StatCounter
    {
        public FunctionStatCounter(string argName) : base(argName) { }
    }

    internal class InputVariableStatCounter : StatCounter
    {
        public InputVariableStatCounter(string argName) : base(argName) { }
    }

    internal class VariableNameStatCounter : StatCounter
    {
        public VariableNameStatCounter(string argName) : base(argName) { }
    }
}

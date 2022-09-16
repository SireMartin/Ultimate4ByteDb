using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbiParser
{
    internal static class StatCounterFactory
    {
        internal static StatCounter CreateFourByteStatCounter(string argName)
        {
            return new FourByteStatCounter(argName);
        }
        internal static StatCounter CreateFunctionStatCounter(string argName)
        {
            return new FunctionStatCounter(argName);
        }
        internal static StatCounter CreateInputVariableStatCounter(string argName)
        {
            return new InputVariableStatCounter(argName);
        }
        internal static StatCounter CreateVariableNameStatCounter(string argName)
        {
            return new VariableNameStatCounter(argName);
        }
    }
}

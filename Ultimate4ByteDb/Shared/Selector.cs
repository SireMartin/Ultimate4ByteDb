using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultimate4ByteDb.Shared
{
    public class FourByteSelector
    {
        public string Selector { get; set; }

        public Signature[] SigColl { get; set; }

        public class Signature
        {
            public SignatureType sigType { get; set; }
            public string Name { get; set; }
            public int Occurance { get; set; }
            public float Likelyhood { get; set; }
            public InputVariableType[] InputVarTypeColl { get; set; }

            public enum SignatureType
            {
                Function,
                Event
            }

            public class InputVariableType
            {
                public string InputVarType { get; set; }
                public VariableName[] VarNameColl { get; set; }

                public class VariableName
                {
                    public string VarName { get; set; }
                    public int Occurance { get; set; }
                    public float Likelyhood { get; set; }
                }
            }
        }
    }
}

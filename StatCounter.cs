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
        public string? Name;
        public StatCounter? Parent;
        public Dictionary<string, StatCounter> Child { get; set; }

        //we do not add non-existing fourbytes (preferred for memory), we generate them afterwards when filling the redis
        public int Occurance;

        public StatCounter(string argName, StatCounter? argParent)
        {
            Name = argName;
            Parent = argParent;
            Occurance = 1;
            Child = new Dictionary<string, StatCounter>();
        }

        public virtual void IncrementOccurance() => ++Occurance;

        public StatCounter AddChild(string argDictRef, string argName, StatCounter? argParent)
        {
            StatCounter? childStatCounter = null;
            if (!Child.ContainsKey(argDictRef))
            {
                childStatCounter = CreateChildStatCounter(argName, argParent);
                //we are in the iteration of a function so there will always be a child of the 4byte sig => a function signature
                Child.Add(argDictRef, childStatCounter);
            }
            else
            {
                childStatCounter = Child[argDictRef];
                childStatCounter.IncrementOccurance();
            }
            return childStatCounter;
        }

        public virtual float CalculateLikelyhood()
        {
            if (Parent is null)
            {
                return 0F;
            }
            return Occurance / Parent.GetSumOfChildOccurences();
        }

        public virtual int GetSumOfChildOccurences() => Child.Sum(x => x.Value.Occurance);

        public virtual StatCounter CreateChildStatCounter(string argName, StatCounter? argParent)
        {
            return new FourByteStatCounter(argName, argParent);
        }
    }

    internal class FourByteStatCounter : StatCounter
    {
        public FourByteStatCounter(string argName, StatCounter? argParent) : base(argName, argParent) { }

        public override StatCounter CreateChildStatCounter(string argName, StatCounter? argParent)
        {
            return new FunctionStatCounter(argName, argParent);
        }
    }

    internal class FunctionStatCounter : StatCounter
    {
        public FunctionStatCounter(string argName, StatCounter? argParent) : base(argName, argParent) { }

        public override StatCounter CreateChildStatCounter(string argName, StatCounter? argParent)
        {
            return new InputVariableStatCounter(argName, argParent);
        }
    }

    internal class InputVariableStatCounter : StatCounter
    {
        public InputVariableStatCounter(string argName, StatCounter? argParent) : base(argName, argParent) { }

        public override StatCounter CreateChildStatCounter(string argName, StatCounter? argParent)
        {
            return new VariableNameStatCounter(argName, argParent);
        }
    }

    internal class VariableNameStatCounter : StatCounter
    {
        public VariableNameStatCounter(string argName, StatCounter? argParent) : base(argName, argParent) { }

        public override StatCounter CreateChildStatCounter(string argName, StatCounter? argParent)
        {
            throw new Exception("No child statcounter type has been defined for VariableNameStatCounter");
        }
    }
}

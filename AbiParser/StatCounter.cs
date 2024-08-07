﻿using System;
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
        public float Occurence;

        public StatCounter(string argName, StatCounter? argParent)
        {
            Name = argName;
            Parent = argParent;
            Occurence = 1;
            Child = new Dictionary<string, StatCounter>();
        }

        public virtual void IncrementOccurance() => ++Occurence;

        public StatCounter AddChild(string argDictRef, string argName, StatCounter? argParent, bool isEvent = false)
        {
            StatCounter? childStatCounter = null;
            if (!Child.ContainsKey(argDictRef))
            {
                childStatCounter = CreateChildStatCounter(argName, argParent, isEvent);
                Child.Add(argDictRef, childStatCounter);
            }
            else
            {
                childStatCounter = Child[argDictRef];
                childStatCounter.IncrementOccurance();
            }
            return childStatCounter;
        }

        public virtual float CalculateLikelyhood() => 
            Parent is null ? 0F: Occurence / Parent.Occurence;

        public virtual StatCounter CreateChildStatCounter(string argName, StatCounter? argParent, bool isEvent = false) =>
            new FourByteStatCounter(argName, argParent);
    }

    internal class FourByteStatCounter : StatCounter
    {
        public FourByteStatCounter(string argName, StatCounter? argParent) : base(argName, argParent) { }

        public override StatCounter CreateChildStatCounter(string argName, StatCounter? argParent, bool isEvent = false) =>
            isEvent? new EventStatCounter(argName, argParent) : new FunctionStatCounter(argName, argParent);
    }

    internal class FunctionStatCounter : StatCounter
    {
        public FunctionStatCounter(string argName, StatCounter? argParent) : base(argName, argParent) { }

        public override StatCounter CreateChildStatCounter(string argName, StatCounter? argParent, bool isEvent = false) =>
            new InputVariableStatCounter(argName, argParent);
    }

    internal class EventStatCounter : StatCounter
    {
        public EventStatCounter(string argName, StatCounter? argParent) : base(argName, argParent) { }

        public override StatCounter CreateChildStatCounter(string argName, StatCounter? argParent, bool isEvent = false) =>
            new InputVariableStatCounter(argName, argParent);
    }

    internal class InputVariableStatCounter : StatCounter
    {
        public InputVariableStatCounter(string argName, StatCounter? argParent) : base(argName, argParent) { }

        public override StatCounter CreateChildStatCounter(string argName, StatCounter? argParent, bool isEvent = false) =>
            new VariableNameStatCounter(argName, argParent);
    }

    internal class VariableNameStatCounter : StatCounter
    {
        public VariableNameStatCounter(string argName, StatCounter? argParent) : base(argName, argParent) { }

        public override StatCounter CreateChildStatCounter(string argName, StatCounter? argParent, bool isEvent = false) =>
            throw new Exception("No child statcounter type has been defined for VariableNameStatCounter");
    }
}

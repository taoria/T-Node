using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace TNode.Attribute{

    public enum PortNameHandling{
        Auto,
        MemberName,
        Manual,
        Format,
        MemberType

    }
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PortAttribute:System.Attribute{
        public readonly string Name;
        public readonly PortNameHandling NameHandling;
 
        public PortAttribute(string name,PortNameHandling nameHandling=PortNameHandling.Auto){
            this.Name = name;
            this.NameHandling = nameHandling;
        }
    }
}
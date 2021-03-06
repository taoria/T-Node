using System;
using JetBrains.Annotations;

namespace TNodeCore.Runtime.Attributes.Ports{

    public enum PortNameHandling{
        Auto,
        MemberName,
        Manual,
        Format,
        MemberType
    }

    public enum TypeHandling{
        Declared,
        Implemented,
        Specified
    }
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PortAttribute:System.Attribute{
        public readonly string Name;
        public readonly PortNameHandling NameHandling;
        public Type HandledType;
        public bool Multiple = true;
        public TypeHandling TypeHandling{ get; set; }
        public PortAttribute(string name,PortNameHandling nameHandling=PortNameHandling.Auto,TypeHandling typeHandling=TypeHandling.Declared){
            this.Name = name;
            this.NameHandling = nameHandling;
            this.TypeHandling = typeHandling;
        }

       
    }
}
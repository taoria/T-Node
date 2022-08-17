using System;
using UnityEngine;
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
        /// <summary>
        /// What's declared in the field is what port type is
        /// </summary>
        Declared,
        /// <summary>
        /// What's the real type of the watched field is what port type is
        /// </summary>
        Implemented,
        /// <summary>
        /// Specify the port type
        /// </summary>
        Specified,
        /// <summary>
        /// Search  the node's path to find the proper type
        /// </summary>
        Path
    }
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class PortAttribute:System.Attribute{
        public readonly string Name;
        public readonly PortNameHandling NameHandling;
        public Type HandledType;
        public bool Multiple = true;
        public string TypePath;
      
        public TypeHandling TypeHandling{ get; set; }
        public PortAttribute(string name,PortNameHandling nameHandling=PortNameHandling.Auto,TypeHandling typeHandling=TypeHandling.Declared){
            this.Name = name;
            this.NameHandling = nameHandling;
            this.TypeHandling = typeHandling;
        }


       
    }
}
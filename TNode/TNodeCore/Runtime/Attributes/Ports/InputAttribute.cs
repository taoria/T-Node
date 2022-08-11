using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TNodeCore.Runtime.Attributes.Ports{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class InputAttribute : PortAttribute{
        public InputAttribute(string name="", PortNameHandling nameHandling = PortNameHandling.Auto,TypeHandling typeHandling=TypeHandling.Declared) : base(name, nameHandling,typeHandling){
        }
  
    }
}
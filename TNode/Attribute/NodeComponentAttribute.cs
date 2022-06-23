using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;


namespace TNode.Attribute{
    
    //Check if the ide is Rider
    
 
   [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
   [MeansImplicitUse]
    
    public class NodeComponentAttribute:System.Attribute{


        public Type GenericType{ get; set; }
    }
}
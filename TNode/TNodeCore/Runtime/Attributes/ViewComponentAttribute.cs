using System;
using JetBrains.Annotations;

namespace TNodeCore.Runtime.Attributes{
    
    //Check if the ide is Rider
    
 
   [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
   [MeansImplicitUse]
    
    public class ViewComponentAttribute:System.Attribute{


        public Type GenericType{ get; set; }
    }
}
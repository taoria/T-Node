using UnityEngine;

namespace TNodeCore.Runtime.Attributes.Ports{
    public class OutputAttribute:PortAttribute{


        public OutputAttribute(string name="", PortNameHandling nameHandling = PortNameHandling.Auto,TypeHandling typeHandling = TypeHandling.Declared) : base(name, nameHandling,typeHandling){
        }
        public OutputAttribute(Color color):base(color){
        }

 
    }
}
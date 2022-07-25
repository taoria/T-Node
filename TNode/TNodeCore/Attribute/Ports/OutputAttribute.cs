namespace TNodeCore.Attribute.Ports{
    public class OutputAttribute:PortAttribute{


        public OutputAttribute(string name="", PortNameHandling nameHandling = PortNameHandling.Auto,TypeHandling typeHandling = TypeHandling.Declared) : base(name, nameHandling,typeHandling){
        }
    }
}
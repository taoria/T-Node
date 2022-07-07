namespace TNode.Attribute.Ports{
    public class OutputAttribute:PortAttribute{
        public OutputAttribute(string name="", PortNameHandling nameHandling = PortNameHandling.Auto) : base(name, nameHandling){
        }
    }
}
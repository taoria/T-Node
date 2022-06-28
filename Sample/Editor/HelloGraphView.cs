using TNode.Attribute;
using TNode.Editor.BaseViews;
using TNode.Editor.Inspector;

namespace Sample.Editor{
    [NodeComponent]
    public class HelloGraphView : DataGraphView<HelloGraph>{
        public override void OnGraphViewCreate(){
            NodeInspector inspector = new NodeInspector();
            this.Add(inspector);
        }
    }
}
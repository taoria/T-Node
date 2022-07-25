using TNodeCore.Editor.NodeGraphView;

namespace TNodeCore.Editor{
    public interface IGraphEditor{
        public void SetGraphView(IBaseDataGraphView graphView);
        public IBaseDataGraphView GetGraphView();
    }
}
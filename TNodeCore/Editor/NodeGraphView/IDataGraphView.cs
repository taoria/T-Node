using TNodeCore.Models;

namespace TNodeCore.Editor.NodeGraphView{
    public interface IDataGraphView<T> : IBaseDataGraphView where T:GraphData{
        public T Data{ get; set; }
    }
}
using TNode.Models;

namespace TNode.Editor.NodeGraphView{
    public interface IDataGraphView<T> : IBaseDataGraphView where T:GraphData{
        public T Data{ get; set; }
    }
}
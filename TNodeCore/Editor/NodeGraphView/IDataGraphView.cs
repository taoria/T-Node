using TNodeCore.Editor.EditorPersistence;
using TNodeCore.Models;
using TNodeEditor.Editor;

namespace TNodeCore.Editor.NodeGraphView{
    public interface IDataGraphView<T> : IBaseDataGraphView where T:GraphData{
        public T Data{ get; set; }
        GraphEditor<T> Owner{ get; set; }
        void SaveWithEditorData(GraphEditorData graphEditorData);
    }
}
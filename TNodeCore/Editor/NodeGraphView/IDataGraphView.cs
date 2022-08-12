using TNode.TNodeCore.Editor.EditorPersistence;
using TNodeCore.Editor.EditorPersistence;
using TNodeCore.Runtime.Models;

namespace TNodeCore.Editor.NodeGraphView{
    /// <summary>
    /// A generic interface of the graph view.inherited from base data graph view.for a better operation only.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataGraphView<T> : IBaseDataGraphView where T:GraphData{
        /// <summary>
        /// The generalized data of a graph.
        /// </summary>
        public T Data{ get; set; }
        /// <summary>
        /// Editor holds the graph view
        /// </summary>
        GraphEditor<T> Owner{ get; set; }
        /// <summary>
        /// Persist data into the graph editor data.
        /// </summary>
        /// <param name="graphEditorData"></param>
        void SaveWithEditorData(GraphEditorData graphEditorData);
    }
}
namespace TNodeCore.Editor.EditorPersistence{
    public interface IGraphViewPersistence{
        string GetPersistenceId();
        void ResetPos(GraphEditorData editorData);
       void SavePos(GraphEditorData editorData);
        
        void OnRemoveFromGraph(GraphEditorData editorData);
    }
}
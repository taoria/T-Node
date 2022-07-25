namespace TNodeCore.Editor.EditorPersistence{
    public interface IGraphViewPersistence{
        public string GetPersistenceId();
        public void ResetPos(GraphEditorData editorData);
        public void SavePos(GraphEditorData editorData);
        
        public void OnRemoveFromGraph(GraphEditorData editorData);
    }
}
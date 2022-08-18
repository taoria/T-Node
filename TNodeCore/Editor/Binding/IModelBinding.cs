namespace TNodeCore.Editor.Binding{
    public interface IModelBinding<T>{
        public T Data{ get; }
        public void Bind(T data);
        public void OnChange();
    }
}
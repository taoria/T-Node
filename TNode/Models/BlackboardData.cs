namespace TNode.Models{

    public class BlackboardData:IModel{
        public T GetValue<T>(string key){
            return default(T);
        }
    }
}
namespace TNodeCore.Runtime.Interfaces{

    public interface IPortTypeConversion<in TFrom, out TTo>{
        public TTo Convert(TFrom tFrom);
    }

}
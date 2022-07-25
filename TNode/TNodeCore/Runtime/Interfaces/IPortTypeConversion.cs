namespace TNodeCore.Runtime.Interfaces{

    public abstract class PortTypeConversion<TFrom, TTo>{
        public abstract TTo Convert(TFrom tFrom);
    }

}
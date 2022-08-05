namespace TNodeCore.Runtime.Interfaces{

    public abstract class PortTypeConversion<TFrom, TTo>{
        public abstract TTo Convert(TFrom tFrom);
    }
    public abstract class TwoWayPortTypeConversion<TFrom, TTo> : PortTypeConversion<TFrom,TTo>{
    
        public abstract TFrom ConvertBack(TTo tTo);
 
    }

}
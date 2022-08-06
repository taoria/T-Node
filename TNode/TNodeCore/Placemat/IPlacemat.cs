using System.Collections.Generic;
using TNodeCore.Runtime.Models;

namespace TNodeCore.Placemat{
    public interface IPlacemat{
        public List<Model> HoldModels{get;set;}
    }
}
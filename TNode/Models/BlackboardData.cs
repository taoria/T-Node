using System;
using System.Collections.Generic;
using UnityEngine;

namespace TNode{
    [Serializable]
    public class BlackboardData{
        public SortedList<string, MonoBehaviour> Behaviours;
        public SortedList<string, float> Floats;
        public SortedList<string, int> Ints;
        public SortedList<string, string> Strings;
    }
}
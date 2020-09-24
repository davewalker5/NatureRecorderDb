using System;
using System.Collections.Generic;
using System.Linq;

namespace NatureRecorder.Interpreter.Entities
{
    public class Map<K, V> : Dictionary<K, V> where V : IComparable
    {
        public K GetKey(V value)
        {
            return this.First(d => d.Value.CompareTo(value) == 0).Key;
        }
    }
}

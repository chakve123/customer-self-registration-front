using System;

namespace BaseLib.Classes
{
    public class StaticObject<T> where T : class
    {
        public T Value { get; set; }

        public DateTime? WatchedDate { get; set; }
    }
}
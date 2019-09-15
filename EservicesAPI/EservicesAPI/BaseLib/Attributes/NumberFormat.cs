using System;

namespace BaseLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NumberFormat : Attribute
    { 
        public string Format { get; set; }

        public NumberFormat(string format)
        {
            Format = format;
        }
    }
}
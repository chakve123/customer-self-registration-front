using System;

namespace BaseLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DateTimeFormat : Attribute
    {
        public string Format { get; set; }

        public DateTimeFormat(string format)
        {
            Format = format;
        }
    }
}
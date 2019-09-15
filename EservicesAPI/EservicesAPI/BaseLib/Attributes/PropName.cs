using System;

namespace BaseLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PropName : Attribute
    { 
        private string _propName;
        public string Name
        {
            get { return _propName; }
            set { _propName = value; }
        }

        public PropName(string Name)
        {
            _propName = Name;
        }
    }
}
using System;

namespace BaseLib.Attributes
{ 
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewName : System.Attribute
    {
        private string _viewName;
        public string Name
        {
            get { return _viewName; }
            set { _viewName = value; }
        }

        private bool _isTableFunction;
        public bool IsTableFunction
        {
            get { return _isTableFunction; }
            set { _isTableFunction = value; }
        }

        public ViewName(string Name)
        {
            _viewName = Name;
        }
        public ViewName(string Name, bool IsTableFunction)
        {
            _viewName = Name;
            _isTableFunction = IsTableFunction;
        }
    }
}
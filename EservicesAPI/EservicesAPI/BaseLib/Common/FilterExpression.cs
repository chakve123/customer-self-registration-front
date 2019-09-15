using System;
using System.Collections.Generic;
using System.Reflection;
using BaseLib.ExtensionMethods;

namespace BaseLib.Common
{
    [Serializable]
    public class FilterExpression
    {
        public string FieldName { get; set; }

        public int DataType { get; set; }
        
        public object FilterValue { get; set; }

        public int Func { get; set; }

        public bool CompareTo(FilterExpression obj)
        {
            return FieldName == obj.FieldName && FilterValue.ToString() == obj.FilterValue.ToString() && Func == obj.Func && DataType == obj.DataType;
        }
    }

    [Serializable]
    public class PeriodControl
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        private DateType _dateType = DateType.Day;
        public DateType DateType
        {
            get { return _dateType; }
            set { _dateType = value; }
        }

        private int _defaultPeriod = 31;
        public int DefaultPeriod
        {
            get { return _defaultPeriod; }
            set { _defaultPeriod = value; }
        }

        public PeriodControl(int defaultPeriod)
        {
            _defaultPeriod = defaultPeriod;
            StartDate = string.Empty;
            EndDate = string.Empty;
        }

        public PeriodControl()
        {
            StartDate = string.Empty;
            EndDate = string.Empty;
        }

        public static PeriodControl DictionaryToClass(Dictionary<string, object> dic)
        {
            var period = new PeriodControl();

            foreach (PropertyInfo prop in typeof(PeriodControl).GetProperties())
            {
                object value;

                dic.TryGetValue(prop.Name, out value);

                if (prop.Name == "DefaultPeriod" && value != null)
                    prop.SetValue(period, value.ToString().ToNumber<int>(), null);
                else
                    prop.SetValue(period, value, null);
            }

            return period;
        }

        public Dictionary<string, object> ClassToDictionary()
        {
            var dc = new Dictionary<string, object>();

            foreach (PropertyInfo prop in typeof(PeriodControl).GetProperties())
            {
                dc.Add(prop.Name, prop.GetValue(this, null));
            }
            return dc;
        }
    }
}
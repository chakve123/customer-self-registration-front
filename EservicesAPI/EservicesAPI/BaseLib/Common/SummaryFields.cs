using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Common
{
    [Serializable]
    public class SummaryFields
    {
        public string FieldName { get; set; }

        public int SummaryFunction { get; set; }

        private int _summaryFraction = 2;
        public int SummaryFraction
        {
            get { return _summaryFraction; }
            set { _summaryFraction = value; }
        }

        private string _summaryField;
        public string SummaryField
        {
            get
            {
                return string.IsNullOrEmpty(_summaryField) ? FieldName : _summaryField;
            }
            set
            {
                _summaryField = value;
            }
        }
    }
}

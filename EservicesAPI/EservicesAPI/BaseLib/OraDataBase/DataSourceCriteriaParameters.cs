using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.OraDataBase
{
    public class DataSourceCriteriaParameters
    {
        private string _parameterName;
        public string ParameterName
        {
            get { return _parameterName[0] == ':' ? _parameterName : string.Format(":{0}", _parameterName); }
            set { _parameterName = value; }
        }

        public CustomOracleDbType Type { get; set; }

        public object Value { get; set; }

        public CustomParameterDirection Direction { get; set; }

        public DataSourceCriteriaParameters(string ParameterName, object Value, CustomOracleDbType Type, CustomParameterDirection Direction)
        {
            this.ParameterName = ParameterName;
            this.Type = Type;
            this.Value = Value;
            this.Direction = Direction;
        }

        public DataSourceCriteriaParameters(string ParameterName, object Value, CustomOracleDbType Type)
        {
            this.ParameterName = ParameterName;
            this.Type = Type;
            this.Value = Value;
        }

        public DataSourceCriteriaParameters() { }
    }

    public enum CustomOracleDbType
    {
        BFile = 101,
        Blob = 102,
        Byte = 103,
        Char = 104,
        Clob = 105,
        Date = 106,
        Decimal = 107,
        Double = 108,
        Long = 109,
        LongRaw = 110,
        Int16 = 111,
        Int32 = 112,
        Int64 = 113,
        IntervalDS = 114,
        IntervalYM = 115,
        NClob = 116,
        NChar = 117,
        NVarchar2 = 119,
        Raw = 120,
        RefCursor = 121,
        Single = 122,
        TimeStamp = 123,
        TimeStampLTZ = 124,
        TimeStampTZ = 125,
        Varchar2 = 126,
        XmlType = 127,
        Array = 128,
        Object = 129,
        Ref = 130,
        BinaryDouble = 132,
        BinaryFloat = 133,
    }

    public enum CustomParameterDirection
    {
        Default = 0,
        Input = 1,
        Output = 2,
        InputOutput = 3,
        ReturnValue = 6,
    }
}

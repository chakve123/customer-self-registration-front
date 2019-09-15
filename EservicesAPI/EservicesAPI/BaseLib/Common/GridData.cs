using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using BaseLib.Attributes;
using BaseLib.Exceptions;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using BaseLib.OraDataBase.StoredProcedures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Oracle.DataAccess.Client;

namespace BaseLib.Common
{
    public class GridData
    {
        public string View { get; set; }

        public string NotifView { get; set; }

        public int StartRowIndex { get; set; }

        public int MaximumRows = 10;

        public string SortExpression { get; set; }

        private List<FilterExpression> _filterExpression;

        //[JsonProperty("FILTER_EXPRESSION")]
        public List<FilterExpression> FilterExpression
        {
            get
            {
                if (_filterExpression == null) _filterExpression = new List<FilterExpression>();
                return _filterExpression;
            }
            set
            {
                _filterExpression = value;
            }
        }

        private List<SummaryFields> _summaryFields;

        public List<SummaryFields> SummaryFields
        {
            get
            {
                if (_summaryFields == null)
                    _summaryFields = new List<SummaryFields>();
                return _summaryFields;
            }
            set
            {
                _summaryFields = value;
            }
        }

        public bool IgnorePeriod { get; set; }

        public GridDataParts DataPart { get; set; }

        private string _criteria;

        public string Criteria { get { return (_criteria == null) ? string.Empty : _criteria; } set { _criteria = value; } }

        public string CriteriaMs { get { return Criteria.Replace(":", "@"); } }

        public string Filter { get; set; }

        public ExportType DataExportType { get; set; }

        public Dictionary<string, string> ExportFields { get; set; }
        public List<xmlExportFields> ExportFieldsForm { get; set; }

        private List<DataSourceCriteriaParameters> _criteriaParameters;

        public List<DataSourceCriteriaParameters> CriteriaParameters
        {
            get
            {
                if (_criteriaParameters == null)
                    _criteriaParameters = new List<DataSourceCriteriaParameters>();
                return _criteriaParameters;
            }
            set
            {
                _criteriaParameters = value;
            }
        }

        private List<DataSourceCriteriaParameters> _tableFunctionParameters;

        public List<DataSourceCriteriaParameters> TableFunctionParameters
        {
            get
            {
                if (_tableFunctionParameters == null)
                    _tableFunctionParameters = new List<DataSourceCriteriaParameters>();
                return _tableFunctionParameters;
            }
            set
            {
                _tableFunctionParameters = value;
            }
        }

        private Dictionary<string, object> _data;

        public Dictionary<string, object> Data
        {
            get
            {
                if (_data == null)
                    _data = new Dictionary<string, object>();
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public List<Thread> Threads { get; set; }

        public string SortString<TDataSource>()
        {
            string sort = "";

            if (!string.IsNullOrEmpty(SortExpression))
            {
                var sortSanitize = Regex.Replace(SortExpression.ToUpper(), " asc| desc|\\.|,|\\(|\\)", "", RegexOptions.IgnoreCase).Trim();

                if (typeof(TDataSource).GetProperty(sortSanitize) != null) sort = string.Format("ORDER BY {0}", SortExpression);
            }

            return sort;

        }

        public string SummaryString
        {
            get
            {
                string summaryString = "";
                int index = 0;
                foreach (SummaryFields item in SummaryFields)
                {
                    switch ((SummaryFunction)item.SummaryFunction)
                    {
                        case SummaryFunction.Sum:
                            summaryString += string.Format("ROUND(SUM({0}),{1}) {0}", item.SummaryField, item.SummaryFraction);
                            break;
                        case SummaryFunction.Average:
                            summaryString += string.Format("ROUND(AVG({0}),{1}) {0}", item.SummaryField, item.SummaryFraction);
                            break;
                        case SummaryFunction.Count:
                            summaryString += string.Format("COUNT(*) as \"{0}\" ", item.SummaryField);
                            break;
                    }

                    if (index < SummaryFields.Count - 1)
                        summaryString += ", ";

                    index++;
                }

                return summaryString;
            }
        }

        public bool CreateFile { get; set; }

        public void GetFilterString(bool addParameter = true)
        {
            Filter = ""; var index = 0; bool isCustomParam = false;
            foreach (FilterExpression g in FilterExpression)
            {
                index++;
                var function = (FilterFunc)g.Func;

                var cp = new DataSourceCriteriaParameters { ParameterName = string.Format("FiltParam{0}", index) };

                if (function == FilterFunc.Equal)
                {
                    if (g.FilterValue == null)
                    {
                        Filter += string.Format(" AND {0} is null", g.FieldName);
                        isCustomParam = true;
                    }
                    else
                    {
                        Filter += string.Format(" AND {0} = :FiltParam{1}", g.FieldName, index);
                        cp.Value = g.FilterValue;
                        isCustomParam = false;
                    }
                }
                else if (function == FilterFunc.NotEqual)
                {
                    if (g.FilterValue == null)
                    {
                        Filter += string.Format(" AND {0} is not null", g.FieldName);
                        isCustomParam = true;
                    }
                    else
                    {
                        Filter += string.Format(" AND {0} <> :FiltParam{1}", g.FieldName, index);
                        cp.Value = g.FilterValue;
                        isCustomParam = false;
                    }
                }
                else if (function == FilterFunc.Contains)
                {
                    Filter += string.Format(" AND upper({0}) LIKE '%'||:FiltParam{1}||'%'", g.FieldName, index);
                    cp.Value = g.FilterValue.ToString().ToUpper();
                    cp.Type = CustomOracleDbType.Varchar2;
                    isCustomParam = false;
                }
                else if (function == FilterFunc.NotContains)
                {
                    Filter += string.Format(" AND upper({0}) Not LIKE '%'||:FiltParam{1}||'%'", g.FieldName, index);
                    cp.Value = g.FilterValue.ToString().ToUpper();
                    cp.Type = CustomOracleDbType.Varchar2;
                    isCustomParam = false;
                }
                else if (function == FilterFunc.InList)
                {
                    if (!string.IsNullOrEmpty(g.FilterValue.ToString()))
                    {
                        int paramCount = 0;
                        var inListStr = String.Empty;
                        foreach (var filterValue in g.FilterValue.ToString().Split(','))
                        {
                            string param = "InListParam" + g.FieldName + paramCount++;
                            if (string.IsNullOrEmpty(inListStr)) inListStr += string.Format(":{0}", param);
                            else inListStr += string.Format(",:{0}", param);

                            CriteriaParameters.Add(new DataSourceCriteriaParameters(param, filterValue, CustomOracleDbType.Varchar2));

                        }
                        Filter += string.Format(" AND {0} in ({1}) ", g.FieldName, inListStr);

                    }
                    //Filter += string.Format(" AND {0} in ({1}) ", g.FieldName, string.Format("'{0}'", string.Join("','", g.FilterValue.ToString().Split(','))));
                    isCustomParam = true;
                }
                else if (function == FilterFunc.InListContains || function == FilterFunc.MultiContains)
                {
                    if (!string.IsNullOrEmpty(g.FilterValue.ToString()))
                    {
                        var allFilterTrue = g.FilterValue.ToString().StartsWith("*");
                        if (allFilterTrue)
                            g.FilterValue = g.FilterValue.ToString().Substring(1);
                        string[] filter = g.FilterValue.ToString().Split(',');
                        for (int i = 0; i < filter.Length; i++)
                        {
                            if (i == 0)
                                Filter += string.Format(" AND (upper({0}) LIKE '%'||'{1}'||'%'", g.FieldName, filter[i]);
                            else
                                if (allFilterTrue)
                                Filter += string.Format(" AND upper({0}) LIKE '%'||'{1}'||'%'", g.FieldName, filter[i]);
                            else
                                Filter += string.Format(" OR upper({0}) LIKE '%'||'{1}'||'%'", g.FieldName, filter[i]);

                            if (i == filter.Length - 1)
                                Filter += ")";
                        }

                    }
                    isCustomParam = true;
                }
                else if (function == FilterFunc.Between)
                {
                    DateTime startDate = DateTime.Now; DateTime endDate = DateTime.Now; bool notValidPeriod = false;

                    Filter += string.Format(" AND {0} BETWEEN :FiltParam{1} AND :FiltParam{2}", g.FieldName, index, ++index);

                    try
                    {
                        var t = g.FilterValue.GetType();
                        var isDict = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
                        if (!isDict) g.FilterValue = g.FilterValue.ToDictionary<object>();
                    }
                    catch
                    {
                    }

                    PeriodControl pc = PeriodControl.DictionaryToClass(g.FilterValue as Dictionary<string, object>);

                    if (g.DataType == (int)DataType.tpDate)
                    {
                        if (!string.IsNullOrEmpty(pc.StartDate) && !string.IsNullOrEmpty(pc.EndDate))
                        {
                            if (!CommonFunctions.ParseDateTime(pc.StartDate, out startDate) &&
                                !CommonFunctions.ParseDateTime(pc.StartDate, out startDate, "dd-MMM-yy") 
                                && !CommonFunctions.ParseDateTime(pc.StartDate, out startDate, "dd-MM-yyyy"))
                            {
                                notValidPeriod = true;
                            }

                            if (CommonFunctions.ParseDateTime(pc.EndDate, out endDate) || 
                                CommonFunctions.ParseDateTime(pc.EndDate, out endDate, "dd-MMM-yy")
                                || CommonFunctions.ParseDateTime(pc.EndDate, out endDate, "dd-MM-yyyy"))
                                endDate = endDate.AddDays(1).AddSeconds(-1);
                            else
                                notValidPeriod = true;
                        }
                        else
                            notValidPeriod = true;

                        if (notValidPeriod)
                        {
                            if (pc.DateType == DateType.Day) startDate = DateTime.Today.AddDays(-pc.DefaultPeriod);
                            else if (pc.DateType == DateType.Month) startDate = DateTime.Today.AddMonths(-pc.DefaultPeriod);
                            else if (pc.DateType == DateType.Year) startDate = DateTime.Today.AddYears(-pc.DefaultPeriod);
                            else startDate = DateTime.Today.AddDays(-pc.DefaultPeriod);

                            endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

                            var period = new Dictionary<string, object>
                                 {
                                     {
                                         g.FieldName,
                                         string.Format("{0} / {1}", startDate.ToString("dd-MMM-yy"),
                                             endDate.ToString("dd-MMM-yy"))
                                     }
                                 };

                            Data.Add("DefaultValues", period);
                            Data.Add("StartDate", startDate.ToShortDateString());
                            Data.Add("EndDate", endDate.ToShortDateString());
                        }

                        cp.Type = CustomOracleDbType.Date;
                        cp.Value = startDate;
                        cp.ParameterName = string.Format("FiltParam{0}", index - 1);
                        CriteriaParameters.Add(cp);

                        cp = new DataSourceCriteriaParameters
                        {
                            ParameterName = string.Format("FiltParam{0}", index),
                            Type = CustomOracleDbType.Date,
                            Value = endDate
                        };

                        CriteriaParameters.Add(cp);
                    }
                    else
                    {
                        cp.Value = pc.StartDate;
                        cp.ParameterName = string.Format("FiltParam{0}", index - 1);
                        CriteriaParameters.Add(cp);

                        cp = new DataSourceCriteriaParameters { ParameterName = string.Format("FiltParam{0}", index) };

                        if (g.DataType == (int)DataType.tpString)
                            cp.Type = CustomOracleDbType.Varchar2;
                        else if (g.DataType == (int)DataType.tpNumber || g.DataType == (int)DataType.tpFormatNumber)
                            cp.Type = CustomOracleDbType.Decimal;

                        cp.Value = pc.EndDate;
                        CriteriaParameters.Add(cp);
                    }

                    isCustomParam = true;
                }
                else if (function == FilterFunc.LessEqualMonth)
                {
                    if (g.DataType == (int)DataType.tpDate)
                    {
                        DateTime dt;
                        try
                        {
                            dt = DateTime.Parse(g.FilterValue.ToString());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(g.FilterValue.ToString(), e);
                        }

                        Filter += string.Format(" AND {0} < :FiltParam{1}", g.FieldName, index);
                        cp.Type = CustomOracleDbType.Date;
                        cp.Value = new DateTime(dt.Year, dt.Month, 1).AddMonths(1);
                        cp.ParameterName = string.Format("FiltParam{0}", index);
                        CriteriaParameters.Add(cp);
                        isCustomParam = true;
                    }
                    else
                    {
                        Filter += string.Format(" AND {0} < :FiltParam{1}", g.FieldName, index);
                        cp.Value = g.FilterValue;
                        isCustomParam = false;
                    }
                }
                else if (function == FilterFunc.Less)
                {
                    if (g.DataType == (int)DataType.tpDate)
                    {
                        DateTime dt;
                        try
                        {
                            dt = DateTime.Parse(g.FilterValue.ToString());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(g.FilterValue.ToString(), e);
                        }

                        Filter += string.Format(" AND {0} < :FiltParam{1}", g.FieldName, index);
                        cp.Type = CustomOracleDbType.Date;
                        cp.Value = dt;
                        cp.ParameterName = string.Format("FiltParam{0}", index);
                        CriteriaParameters.Add(cp);
                        isCustomParam = true;
                    }
                    else
                    {
                        Filter += string.Format(" AND {0} < :FiltParam{1}", g.FieldName, index);
                        cp.Value = g.FilterValue;
                        isCustomParam = false;
                    }
                }
                else if (function == FilterFunc.Greater)
                {
                    if (g.DataType == (int)DataType.tpDate)
                    {
                        DateTime dt;
                        try
                        {
                            //  gansazogadoebeliaa
                            if (g.FilterValue.ToString() == "year-2") dt = DateTime.Now.AddYears(-2);
                            else dt = DateTime.Parse(g.FilterValue.ToString());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(g.FilterValue.ToString(), e);
                        }

                        Filter += string.Format(" AND {0} > :FiltParam{1}", g.FieldName, index);
                        cp.Type = CustomOracleDbType.Date;
                        cp.Value = dt;
                        cp.ParameterName = string.Format("FiltParam{0}", index);
                        CriteriaParameters.Add(cp);
                        isCustomParam = true;
                    }
                    else
                    {
                        Filter += string.Format(" AND {0} > :FiltParam{1}", g.FieldName, index);
                        cp.Value = g.FilterValue;
                        isCustomParam = false;
                    }
                }
                else if (function == FilterFunc.InInterval)
                {
                    Filter += string.Format(" AND {0} BETWEEN :FiltParam{1} AND :FiltParam{2}", g.FieldName, index, ++index);
                    // rsPeriodControl pc = rsPeriodControl.DictionaryToClass(g.FilterValue as Dictionary<string, object>);

                    cp.Type = CustomOracleDbType.Decimal;
                    cp.Value = ((System.Collections.Generic.Dictionary<string, object>)g.FilterValue)["StartNumber"];
                    cp.ParameterName = string.Format("FiltParam{0}", index - 1);
                    CriteriaParameters.Add(cp);

                    var cp1 = new DataSourceCriteriaParameters
                    {
                        ParameterName = string.Format("FiltParam{0}", index),
                        Type = CustomOracleDbType.Decimal,
                        Value = ((System.Collections.Generic.Dictionary<string, object>)g.FilterValue)["EndNumber"]
                    };

                    CriteriaParameters.Add(cp1);
                    isCustomParam = true;

                }
                else if (function == FilterFunc.Begin)
                {
                    if (g.FilterValue != null)
                    {
                        Filter += string.Format(" AND {0} LIKE :FiltParam{1}||'%'", g.FieldName, index);
                        cp.Value = g.FilterValue;
                        isCustomParam = false;
                    }
                }

                if (addParameter && !isCustomParam)
                {
                    if (g.DataType == (int)DataType.tpString)
                        cp.Type = CustomOracleDbType.Varchar2;
                    else if (g.DataType == (int)DataType.tpDate)
                    {
                        try
                        {
                            cp.Value = DateTime.Parse(cp.Value.ToString());
                        }
                        catch (Exception e)
                        {
                            PeriodControl pc = PeriodControl.DictionaryToClass(g.FilterValue as Dictionary<string, object>);
                            throw new Exception(string.Format("{0} {1}/ function={2}", pc.StartDate, pc.EndDate, function), e);
                        }

                        cp.Type = CustomOracleDbType.Date;
                    }
                    else if (g.DataType == (int)DataType.tpNumber || g.DataType == (int)DataType.tpFormatNumber)
                        cp.Type = CustomOracleDbType.Decimal;

                    CriteriaParameters.Add(cp);
                }
            }
        }

        public Dictionary<string, object> GetData<T>() where T : DataSourceProvider, new()
        {
            GetFilterString();
            if (DataExportType == ExportType.GridBind)
            {
                Threads = DataPart == GridDataParts.All ? new List<Thread>() : null;
                if (DataPart == GridDataParts.All || DataPart == GridDataParts.Data) Data.Add("Data", RetrieveData<T>());


                if (DataPart == GridDataParts.All || DataPart == GridDataParts.Summary)
                {
                    if (SummaryFields == null) SummaryFields = new List<SummaryFields>();

                    SummaryFields.Add(new SummaryFields
                    {
                        SummaryFunction = (int)SummaryFunction.Count,
                        FieldName = "_Count",
                        SummaryField = "_Count"
                    });
                    Data.Add("Summary", RetrieveSummary<T>());
                }


                try
                {

                    if (Threads != null)
                    {
                        foreach (var thread in Threads)
                        {
                            thread.Join();
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }



            }
            else
                ExportData<T>();

            return Data;
        }

        public JObject GetDataJson<T>() where T : DataSourceProvider, new()
        {
            GetFilterString();

            var res = new JObject();

            if (DataExportType == ExportType.GridBind)
            {
                Threads = DataPart == GridDataParts.All ? new List<Thread>() : null;
                if (DataPart == GridDataParts.All || DataPart == GridDataParts.Data)
                {
                    var dataProp = new JProperty("data", RetrieveDataJson<T>());
                    res.Add(dataProp);
                    // Data.Add("Data", RetrieveDataJson<T>());
                }


                if (DataPart == GridDataParts.All || DataPart == GridDataParts.Summary)
                {
                    if (SummaryFields == null) SummaryFields = new List<SummaryFields>();

                    SummaryFields.Add(new SummaryFields
                    {
                        SummaryFunction = (int)SummaryFunction.Count,
                        FieldName = "_Count",
                        SummaryField = "_Count"
                    });
                    res.Add("Summary", JsonConvert.SerializeObject((RetrieveSummary<T>())));
                    // Data.Add("Summary", RetrieveSummary<T>());
                }


                try
                {

                    if (Threads != null)
                    {
                        foreach (var thread in Threads)
                        {
                            thread.Join();
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }



            }
            else
                ExportData<T>();
            return res;
        }


        public Dictionary<string, object> GetData<T>(List<T> list) where T : class, new()
        {
            try
            {
                var dictList = list.Select(CommonFunctions.ClassToDictionary).ToList();
                Data = RetrieveListFromDic(dictList);
                Data.Add("Summary", RetrieveListSummary(dictList));

                if (DataExportType == ExportType.ExportXls)
                    ExportXLS.ExportNew<T>(Data["Data"], ExportFields);
                //else if (DataExportType == ExportType.ExportListCsv)
                //    ExportDataLi<T>();
                return Data;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("startRowIndex={0} maximumRows={1} listCount={2}", StartRowIndex, MaximumRows, 0), e);
            }
        }


        public Dictionary<string, object> RetrieveListFromDic<T>(List<T> list, bool fullData = false) where T : Dictionary<string, object>, new()
        {
            var returnData = new Dictionary<string, object>();
            var returningList = new List<T>();
            returningList.AddRange(list);
            try
            {
                if (FilterExpression != null && FilterExpression.Count > 0)
                {
                    foreach (FilterExpression filter in FilterExpression)
                    {
                        var function = (FilterFunc)filter.Func;

                        if (function == FilterFunc.Equal)
                        {
                            returningList = returningList.FindAll(delegate (T f)
                            {
                                if (f.ContainsKey(filter.FieldName))
                                {
                                    return f[filter.FieldName].ToString() == filter.FilterValue.ToString();
                                }
                                return false;
                            });
                        }
                        else if (function == FilterFunc.NotEqual)
                        {
                            returningList = returningList.FindAll(delegate (T f)
                            {
                                if (f.ContainsKey(filter.FieldName))
                                {
                                    return f[filter.FieldName].ToString() != filter.FilterValue.ToString();
                                }
                                return false;
                            });
                        }
                        else if (function == FilterFunc.Contains)
                        {
                            returningList = returningList.FindAll(delegate (T f)
                            {
                                if (f.ContainsKey(filter.FieldName))
                                {
                                    return f[filter.FieldName].ToString().Contains(filter.FilterValue.ToString());
                                }
                                return false;
                            });
                        }
                        else if (function == FilterFunc.InList)
                        {
                            var filterValueList = filter.FilterValue.ToString().Split(',');
                            returningList = returningList.FindAll(delegate (T f)
                            {
                                if (f.ContainsKey(filter.FieldName))
                                {
                                    return filterValueList.Contains(f[filter.FieldName].ToString());
                                }
                                return false;
                            });
                        }
                        else if (function == FilterFunc.Between)
                        {
                            DateTime startDate = DateTime.Now; DateTime endDate = DateTime.Now; bool notValidPeriod = false;
                            PeriodControl pc = PeriodControl.DictionaryToClass(filter.FilterValue as Dictionary<string, object>);

                            if (filter.DataType == (int)DataType.tpDate)
                            {
                                if (!string.IsNullOrEmpty(pc.StartDate) && !string.IsNullOrEmpty(pc.EndDate))
                                {
                                    if (!CommonFunctions.ParseDateTime(pc.StartDate, out startDate) && !CommonFunctions.ParseDateTime(pc.StartDate, out startDate, "dd-MMM-yy"))
                                        notValidPeriod = true;

                                    if (CommonFunctions.ParseDateTime(pc.EndDate, out endDate) || CommonFunctions.ParseDateTime(pc.EndDate, out endDate, "dd-MMM-yy"))
                                        endDate = endDate.AddDays(1).AddSeconds(-1);
                                    else
                                        notValidPeriod = true;
                                }
                                else
                                    notValidPeriod = true;

                                if (notValidPeriod)
                                {
                                    if (pc.DateType == DateType.Day) startDate = DateTime.Today.AddDays(-pc.DefaultPeriod);
                                    else if (pc.DateType == DateType.Month) startDate = DateTime.Today.AddMonths(-pc.DefaultPeriod);
                                    else if (pc.DateType == DateType.Year) startDate = DateTime.Today.AddYears(-pc.DefaultPeriod);
                                    else startDate = DateTime.Today.AddDays(-pc.DefaultPeriod);

                                    endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

                                    var period = new Dictionary<string, object>
                                         {
                                             {
                                                 filter.FieldName,
                                                 string.Format("{0} / {1}", startDate.ToString("dd-MMM-yy"),
                                                     endDate.ToString("dd-MMM-yy"))
                                             }
                                         };

                                    Data.Add("DefaultValues", period);
                                    Data.Add("StartDate", startDate.ToShortDateString());
                                    Data.Add("EndDate", endDate.ToShortDateString());
                                }

                                returningList = returningList.FindAll(delegate (T f)
                                {
                                    if (f.ContainsKey(filter.FieldName))
                                    {
                                        return notValidPeriod ? DateTime.Parse(f[filter.FieldName].ToString()) >= startDate && DateTime.Parse(f[filter.FieldName].ToString()) <= endDate : DateTime.Parse(f[filter.FieldName].ToString()) >= DateTime.Parse(pc.StartDate) && DateTime.Parse(f[filter.FieldName].ToString()) <= DateTime.Parse(pc.EndDate);
                                    }
                                    return false;
                                });
                            }
                        }
                        else if (function == FilterFunc.InInterval)
                        {

                            decimal startNumber = Decimal.Parse(((System.Collections.Generic.Dictionary<string, object>)filter.FilterValue)["StartNumber"].ToString());
                            decimal endNumber = Decimal.Parse(((System.Collections.Generic.Dictionary<string, object>)filter.FilterValue)["EndNumber"].ToString());


                            returningList = returningList.FindAll(delegate (T f)
                            {
                                if (f.ContainsKey(filter.FieldName))
                                {
                                    return Decimal.Parse(f[filter.FieldName].ToString()) >= startNumber && Decimal.Parse(f[filter.FieldName].ToString()) <= endNumber;
                                }
                                return false;
                            });
                        }
                        else if (function == FilterFunc.MultiContains)
                        {
                            returningList = returningList.FindAll(delegate (T f)
                            {
                                if (f.ContainsKey(filter.FieldName))
                                {
                                    var allFilterTrue = filter.FilterValue.ToString().StartsWith("*");
                                    if (allFilterTrue)
                                        filter.FilterValue = filter.FilterValue.ToString().Substring(1);
                                    var multiFilter = filter.FilterValue.ToString().Split(',').ToList();
                                    if (allFilterTrue)
                                        return multiFilter.All(f[filter.FieldName].ToString().Contains);
                                    else
                                        return multiFilter.Any(f[filter.FieldName].ToString().Contains);
                                }
                                return false;
                            });
                        }
                    }
                }

                if (!string.IsNullOrEmpty(SortExpression))
                {
                    var sortArray = SortExpression.Split(' ');
                    Comparison<T> c = delegate (T param1, T param2)
                    {
                        int returningType = 0;
                        string keyField = sortArray[0];
                        if (param1.ContainsKey(keyField) && param2.ContainsKey(keyField))
                        {
                            var paramTypes = new[] { param1[keyField].GetType(), param2[keyField].GetType() };
                            MethodInfo method = param1[keyField].GetType().GetMethod("Compare", paramTypes);
                            var parameters = new[] { param1[keyField], param2[keyField] };

                            if (method != null)
                            {
                                returningType = (int)method.Invoke(param1[keyField].GetType(), parameters);
                            }
                            else
                            {
                                if (param1[keyField] is double)
                                {
                                    returningType = ((double)param1[keyField]).CompareTo(param2[keyField]);
                                }
                            }
                        }


                        if (sortArray[1].ToLower() == "asc")
                            return returningType;
                        else
                            return returningType * -1;
                    };

                    returningList.Sort(c);
                }

                returnData.Add("Count", returningList.Count);


                returningList = fullData ? GetListWithRNUM(returningList) : GetListWithRNUM(returningList)
                    .GetRange(StartRowIndex,
                        returningList.Count - StartRowIndex < MaximumRows
                            ? returningList.Count - StartRowIndex
                            : MaximumRows);

                var fields = returningList.Count > 0 ? returningList[0].Keys.ToList() : new List<string>();
                var rows = new List<object>();

                foreach (var retVal in returningList)
                {
                    var listVal = new List<object>();
                    foreach (var field in fields)
                    {
                        var prop = retVal as Dictionary<string, object>;
                        listVal.Add(prop[field]);
                    }

                    rows.Add(listVal);
                }

                returnData.Add("Data", new Dictionary<string, object>
                    {
                        {"Fields", fields},
                        {"Rows", rows}
                    });
                return returnData;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("startRowIndex={0} maximumRows={1} listCount={2}", StartRowIndex, MaximumRows, returningList.Count), e);
            }
        }
        public List<T> GetListWithRNUM<T>(List<T> list) where T : Dictionary<string, object>
        {
            decimal index = 1;
            foreach (T item in list)
            {
                if (item.ContainsKey("RNUM"))
                {
                    item["RNUM"] = index;
                    index++;
                }

                if (index == 1)
                    break;
            }

            return list;
        }
        public Dictionary<string, object> RetrieveListSummary<T>(List<T> list) where T : Dictionary<string, object>, new()
        {
            var data = new Dictionary<string, object>();
            if (SummaryFields == null)
                return data;

            var summaryList = (Dictionary<string, object>)RetrieveListFromDic(list, true);
            var fields = (summaryList["Data"] as Dictionary<string, object>)["Fields"] as List<string>;
            var rows = (summaryList["Data"] as Dictionary<string, object>)["Rows"] as List<object>;

            double value = 0;
            foreach (SummaryFields item in SummaryFields)
            {
                int index;
                switch ((SummaryFunction)item.SummaryFunction)
                {
                    case SummaryFunction.Sum:
                        index = fields.IndexOf(item.SummaryField);
                        value = rows.FindAll(r => (r as List<object>)[index] != null)
                                .Sum(r => (r as List<object>)[index].ToString().ToNumber<double>());
                        break;
                    case SummaryFunction.Average:
                        index = fields.IndexOf(item.SummaryField);
                        value = rows.FindAll(r => (r as List<object>)[index] != null)
                                .Average(r => (r as List<object>)[index].ToString().ToNumber<double>());
                        break;
                    case SummaryFunction.Count:
                        if (rows != null) value = rows.Count;
                        break;
                }
                data.Add(item.FieldName, Math.Round(value, item.SummaryFraction));
                value = 0;
            }

            return data;
        }

        public Dictionary<string, object> RetrieveSummary<T>() where T : DataSourceProvider, new()
        {
            var data = new Dictionary<string, object>();
            if (SummaryFields == null)
                return data;

            if (!string.IsNullOrEmpty(Filter))
            {
                if (string.IsNullOrEmpty(Criteria))
                    Criteria = "1=1";//TODO: And Propblem

                Criteria += Filter;
            }

            string summaryString = SummaryString;

            if (string.IsNullOrEmpty(summaryString))
                return data;

            var cmd = new OracleCommand();
            if (string.IsNullOrEmpty(Criteria))
                cmd.CommandText = string.Format("SELECT {0} FROM {1}", summaryString, GetTableName<T>());
            else
                cmd.CommandText = string.Format("SELECT {0} FROM {1} WHERE {2}", summaryString, GetTableName<T>(), Criteria);
            cmd.CommandType = System.Data.CommandType.Text;

            foreach (DataSourceCriteriaParameters parameter in TableFunctionParameters)
            {
                if (parameter.Direction == CustomParameterDirection.Default)
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type).Value = parameter.Value;
                else if (parameter.Direction == CustomParameterDirection.Input || parameter.Direction == CustomParameterDirection.InputOutput)
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type, (System.Data.ParameterDirection)parameter.Direction).Value = parameter.Value;
                else
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type, (System.Data.ParameterDirection)parameter.Direction);
            }


            foreach (DataSourceCriteriaParameters parameter in CriteriaParameters)
            {
                if (parameter.Direction == CustomParameterDirection.Default)
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type).Value = parameter.Value;
                else if (parameter.Direction == CustomParameterDirection.Input || parameter.Direction == CustomParameterDirection.InputOutput)
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type, (System.Data.ParameterDirection)parameter.Direction).Value = parameter.Value;
                else
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type, (System.Data.ParameterDirection)parameter.Direction);
            }

            string error;
            new OracleDb<T>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                foreach (SummaryFields item in SummaryFields)
                {
                    try
                    {
                        data.Add(item.FieldName, reader[item.SummaryField]);
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "Arithmetic operation resulted in an overflow.")
                            data.Add(item.FieldName, "- - -");
                        else
                            throw;
                    }
                }
            }, Threads);

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return data;
        }

        public void ExportData<T>(ExportType exportType) where T : DataSourceProvider, new()
        {
            DataExportType = exportType;
            ExportData<T>();
        }

        public void ExportData<T>() where T : DataSourceProvider, new()
        {
            if (ExportFields == null)
                throw new Exception("Field is empty");
            object[] attr;
            bool firstUse = true;
            string error = string.Empty;
            int index = 1;

            SummaryFields = new List<SummaryFields>{
                new SummaryFields
                {
                    SummaryFunction = (int) SummaryFunction.Count,
                    FieldName = "_Count",
                    SummaryField = "_Count"
                }};

            var thread = new Thread(() =>
            {
                var res = RetrieveSummary<T>();
                //DataProviderManager<PKG_ERROR_LOGS>.Provider.Progress(PageID, null, (res as Dictionary<string, object>)["_Count"].ToString().ToNumber<int>());
            });
            thread.Start();

            var cmd = RetrieveDataCmd<T>();

            if (DataExportType == ExportType.ExportCsv)
            {
                var filePath = ConfigurationManager.AppSettings.GetValues("GlobalTempFolder")[0] + Guid.NewGuid() + ".csv";
                try
                {
                    File.Delete(@filePath);
                }
                catch
                {
                }

                using (var sw = new StreamWriter(@filePath, true, Encoding.UTF8))
                {
                    new OracleDb<T>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
                    {
                        if (firstUse)
                        {
                            foreach (var prop in ExportFields.Values)
                            {
                                sw.Write(prop.MakeValueCsvFriendly() + ",");
                            }

                            sw.WriteLine();
                            firstUse = false;
                        }

                        //if (index % 3000 == 0)
                        //    DataProviderManager<PKG_ERROR_LOGS>.Provider.Progress(PageID, index, null);

                        var dsObject = new T();

                        foreach (var field in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(prop => prop.Name).ToList())
                        {
                            var prop = typeof(T).GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
                            if (prop == null) continue;
                            attr = prop.GetCustomAttributes(typeof(PropName), false);
                            string propName = attr.Length > 0 ? (attr[0] as PropName).Name : prop.Name;
                            object propValue;
                            try
                            {
                                propValue = reader[propName];
                            }
                            catch
                            {
                                continue;
                            }
                            if (!string.IsNullOrEmpty(propValue.ToString()))
                            {
                                attr = prop.GetCustomAttributes(typeof(DateTimeFormat), false);

                                if (attr.Length > 0)
                                {
                                    if (attr[0] is DateTimeFormat)
                                        prop.SetValue(dsObject, DateTime.Parse(propValue.ToString()).ToString((attr[0] as DateTimeFormat).Format), null);
                                    else if (attr[0] is NumberFormat)
                                        prop.SetValue(dsObject, decimal.Parse(propValue.ToString()).ToString((attr[0] as NumberFormat).Format), null);
                                }
                                else prop.SetValue(dsObject, propValue, null);
                            }
                        }

                        foreach (var key in ExportFields.Keys)
                        {
                            PropertyInfo prop = typeof(T).GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
                            if (prop == null) continue;
                            attr = prop.GetCustomAttributes(typeof(PropName), false);
                            var val = prop.GetValue(dsObject, null);

                            sw.Write(val.MakeValueCsvFriendly() + ",");
                        }

                        sw.WriteLine();
                        index++;
                    });
                }
                if (!CreateFile)
                {
                    using (FileStream stream = File.Open(@filePath, FileMode.Open))
                    {
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        HttpContext.Current.Response.ClearContent();
                        HttpContext.Current.Response.ClearHeaders();

                        //var clearProcessingDone = new HttpCookie("rsProccessing_" + PageID)
                        //{
                        //    Expires = DateTime.Now.AddDays(-1)
                        //};
                        //HttpContext.Current.Response.Cookies.Add(clearProcessingDone);
                        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
                        HttpContext.Current.Response.AddHeader("Access-Control-Expose-Headers", "Content-Disposition");

                        string fileName = "Export.csv";
                        HttpContext.Current.Response.ContentType = CommonFunctions.getFilesContentType(fileName);
                        HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                        CommonFunctions.SequantialFlushing(stream);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }

                    File.Delete(@filePath);
                }
            }
            else if (DataExportType == ExportType.ExportXls)
            {
                var filePath = ConfigurationManager.AppSettings.GetValues("GlobalTempFolder")[0] + Guid.NewGuid() + ".xls";
                File.Delete(@filePath);

                var hssfworkbook = new HSSFWorkbook();
                ISheet gridSheet = ExportXLS.CreateSheet("Grid", hssfworkbook);
                ICellStyle dateStyle = ExportXLS.GetDateStyle(hssfworkbook);
                ICellStyle cellStyle = ExportXLS.GetCellStyle(hssfworkbook);

                new OracleDb<T>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
                {
                    if (firstUse)
                    {
                        ExportXLS.SetHeader(ExportFields, gridSheet, hssfworkbook);

                        firstUse = false;
                    }

                    //if (index % 3000 == 0)
                    //    DataProviderManager<PKG_COMMON>.Provider.Progress(PageID, index, null);

                    IRow row = gridSheet.CreateRow(index);

                    int j = 0;
                    var dsObject = new T();

                    foreach (var field in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(prop => prop.Name).ToList())
                    {
                        var prop = typeof(T).GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
                        if (prop == null) continue;
                        attr = prop.GetCustomAttributes(typeof(PropName), false);
                        string propName = attr.Length > 0 ? (attr[0] as PropName).Name : prop.Name;
                        object propValue;
                        try
                        {
                            propValue = reader[propName];
                        }
                        catch
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(propValue.ToString()))
                        {
                            attr = prop.GetCustomAttributes(typeof(DateTimeFormat), false);

                            if (attr.Length > 0)
                            {
                                if (attr[0] is DateTimeFormat)
                                    prop.SetValue(dsObject, DateTime.Parse(propValue.ToString()).ToString((attr[0] as DateTimeFormat).Format), null);
                                else if (attr[0] is NumberFormat)
                                    prop.SetValue(dsObject, decimal.Parse(propValue.ToString()).ToString((attr[0] as NumberFormat).Format), null);
                            }
                            else prop.SetValue(dsObject, propValue, null);
                        }
                    }

                    foreach (string field in ExportFields.Keys)
                    {
                        PropertyInfo prop = typeof(T).GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
                        if (prop == null) continue;
                        var val = prop.GetValue(dsObject, null);
                        attr = prop.GetCustomAttributes(false);


                        ICell cell = row.CreateCell(j);
                        cell.CellStyle = cellStyle;

                        if (val == null || string.IsNullOrEmpty(val.ToString()))
                            cell.SetCellValue("");
                        else if (prop.PropertyType.IsNumericType())
                        {
                            cell.SetCellValue(val.ToString().ToNumber<double>());
                        }
                        else if (attr.Length > 0)
                        {
                            if (attr[0] is DateTimeFormat)
                            {
                                DateTime dt = new DateTime();
                                if (!DateTime.TryParse(val.ToString(), out dt))
                                {
                                    cell.SetCellValue(double.Parse(val.ToString()));
                                }
                                else
                                {
                                    ICell dateCell = row.CreateCell(j);
                                    dateCell.SetCellValue(DateTime.Parse(val.ToString()));
                                    dateCell.CellStyle = dateStyle;
                                }
                            }
                            else if (attr[0] is NumberFormat)
                                cell.SetCellValue(double.Parse(val.ToString()));
                            else
                                cell.SetCellValue(val.ToString());
                        }
                        else
                            cell.SetCellValue(val.ToString());

                        j++;
                    }

                    index++;
                });

                ExportXLS.SetCellSize<T>(ExportFields, gridSheet, hssfworkbook);

                if (!CreateFile)
                {

                    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
                    HttpContext.Current.Response.AddHeader("Access-Control-Expose-Headers", "Content-Disposition");
                    const string filename = "Export.xls";
                    HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

                    var now = DateTime.Now;
                    var cookieRsDownloading = new HttpCookie("rsDownloading")
                    {
                        Value = "Downloading",
                        Expires = now.AddDays(-1)
                    };
                    HttpContext.Current.Response.Cookies.Add(cookieRsDownloading);

                    HttpContext.Current.Response.Clear();

                    var file = new MemoryStream();
                    hssfworkbook.Write(file);

                    file.Position = 0;

                    CommonFunctions.SequantialFlushing(file);
                    file.Close();
                    file.Dispose();
                    HttpContext.Current.Response.End();
                }
                else
                {
                    using (var fileStream = File.Create(filePath))
                    {
                        hssfworkbook.Write(fileStream);
                    }
                }
            }

            //RsDataProviderManager<PKG_COMMON>.Provider.Progress(PageID, -1, null);

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }

        public Dictionary<string, object> RetrieveData<TDataSource>() where TDataSource : DataSourceProvider, new()
        {
            var list = new Dictionary<string, object>();
            object[] attr;

            var cmd = RetrieveDataCmd<TDataSource>();

            string error;

            PropertyInfo[] properties = typeof(TDataSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //Array.Sort(properties, new DeclarationOrderComparator()); // Added by Koka
            var fields = properties.Select(prop => prop.Name).ToList();
            if (fields.Contains("ConnectionString")) fields.Remove("ConnectionString");
            list.Add("Fields", fields);

            var values = new List<object>();
            new OracleDb<TDataSource>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                var dsObject = new TDataSource();

                foreach (var field in fields)
                {
                    var prop = typeof(TDataSource).GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
                    if (prop == null) continue;
                    attr = prop.GetCustomAttributes(typeof(PropName), false);
                    string propName = attr.Length > 0 ? (attr[0] as PropName).Name : prop.Name;
                    object propValue;
                    try
                    {
                        propValue = reader[propName];
                    }
                    catch
                    {
                        continue;
                    }
                    try
                    {
                        if (!string.IsNullOrEmpty(propValue.ToString()))
                        {
                            if (prop.PropertyType == typeof(bool))
                            {
                                propValue = ((decimal)propValue) > 0;
                                prop.SetValue(dsObject, propValue, null);
                            }
                            else
                            {
                                attr = prop.GetCustomAttributes(typeof(DateTimeFormat), false);

                                if (attr.Length > 0)
                                {
                                    if (attr[0] is DateTimeFormat)
                                        prop.SetValue(dsObject, DateTime.Parse(propValue.ToString()).ToString((attr[0] as DateTimeFormat).Format), null);
                                    else if (attr[0] is NumberFormat)
                                        prop.SetValue(dsObject, decimal.Parse(propValue.ToString()).ToString((attr[0] as NumberFormat).Format), null);
                                }
                                else prop.SetValue(dsObject, propValue, null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(propName + " : " + ex.ToString());
                    }
                }

                var value = fields.Select(field => typeof(TDataSource).GetProperty(field)).Select(prop => prop.GetValue(dsObject, null)).ToList();

                values.Add(value);
            }, Threads);

            list.Add("Rows", values);

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return list;
        }

        public JObject RetrieveDataJson<TDataSource>() where TDataSource : DataSourceProvider, new()
        {
            var list = new JObject();
            object[] attr;

            var cmd = RetrieveDataCmd<TDataSource>();

            string error;

            PropertyInfo[] properties = typeof(TDataSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //Array.Sort(properties, new DeclarationOrderComparator()); // Added by Koka
            var fields = properties.Select(prop => prop.Name).ToList();
            if (fields.Contains("ConnectionString")) fields.Remove("ConnectionString");

            var fieldsProp = new JProperty("Fields", JArray.Parse(JsonConvert.SerializeObject(fields)));
            //var jsonRes = new JObject(fieldsProp,fieldsProp);
            list.Add(fieldsProp);

            var values =new JArray();
            new OracleDb<TDataSource>().ProcessEachRow(cmd, out error, delegate (OracleDataReader reader)
            {
                var dsObject = new TDataSource();

                foreach (var field in fields)
                {
                    var prop = typeof(TDataSource).GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
                    if (prop == null) continue;
                    attr = prop.GetCustomAttributes(typeof(PropName), false);
                    string propName = attr.Length > 0 ? (attr[0] as PropName).Name : prop.Name;
                    object propValue;
                    try
                    {
                        propValue = reader[propName];
                    }
                    catch
                    {
                        continue;
                    }
                    try
                    {
                        if (!string.IsNullOrEmpty(propValue.ToString()))
                        {
                            if (prop.PropertyType == typeof(bool))
                            {
                                propValue = ((decimal)propValue) > 0;
                                prop.SetValue(dsObject, propValue, null);
                            }
                            else
                            {
                                attr = prop.GetCustomAttributes(typeof(DateTimeFormat), false);

                                if (attr.Length > 0)
                                {
                                    if (attr[0] is DateTimeFormat)
                                        prop.SetValue(dsObject, DateTime.Parse(propValue.ToString()).ToString((attr[0] as DateTimeFormat).Format), null);
                                    else if (attr[0] is NumberFormat)
                                        prop.SetValue(dsObject, decimal.Parse(propValue.ToString()).ToString((attr[0] as NumberFormat).Format), null);
                                }
                                else prop.SetValue(dsObject, propValue, null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(propName + " : " + ex.ToString());
                    }
                }


                // var value = fields.Select(field => typeof(TDataSource).GetProperty(field)).Select(prop => prop.GetValue(dsObject, null)).ToList();

                values.Add(JObject.FromObject(dsObject));
            }, Threads);

            var valuesProp = new JProperty("Rows", values);

            list.Add(valuesProp);

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return list;
        }


        public OracleCommand RetrieveDataCmd<TDataSource>()
        {
            if (!string.IsNullOrEmpty(Filter))
            {
                if (string.IsNullOrEmpty(Criteria))
                    Criteria = "1=1";//TODO: And Propblem

                Criteria += Filter;
            }

            var cmd = new OracleCommand();
            if (string.IsNullOrEmpty(Criteria))
                cmd.CommandText = string.Format("SELECT * FROM (SELECT t.*,ROWNUM RNUM FROM (SELECT * FROM {0} {1}) t WHERE ROWNUM <={3} ) WHERE RNUM >{2}", GetTableName<TDataSource>(), SortString<TDataSource>(), StartRowIndex, StartRowIndex + MaximumRows);
            else
                cmd.CommandText = string.Format("SELECT * FROM (SELECT t.*,ROWNUM RNUM FROM (SELECT * FROM {0} WHERE {1} {2}) t WHERE ROWNUM <={4} ) WHERE RNUM >{3}", GetTableName<TDataSource>(), Criteria, SortString<TDataSource>(), StartRowIndex, StartRowIndex + MaximumRows);
            cmd.CommandType = System.Data.CommandType.Text;


            foreach (DataSourceCriteriaParameters parameter in TableFunctionParameters)
            {
                if (parameter.Direction == CustomParameterDirection.Default)
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type).Value = parameter.Value;
                else if (parameter.Direction == CustomParameterDirection.Input || parameter.Direction == CustomParameterDirection.InputOutput)
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type, (System.Data.ParameterDirection)parameter.Direction).Value = parameter.Value;
                else
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type, (System.Data.ParameterDirection)parameter.Direction);
            }

            foreach (DataSourceCriteriaParameters parameter in CriteriaParameters)
            {
                if (parameter.Direction == CustomParameterDirection.Default)
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type).Value = parameter.Value;
                else if (parameter.Direction == CustomParameterDirection.Input || parameter.Direction == CustomParameterDirection.InputOutput)
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type, (System.Data.ParameterDirection)parameter.Direction).Value = parameter.Value;
                else
                    cmd.Parameters.Add(parameter.ParameterName, (OracleDbType)parameter.Type, (System.Data.ParameterDirection)parameter.Direction);
            }
            return cmd;
        }





        private string GetTableName<TDataSource>()
        {
            string tableName = "";
            var gridAttribute = CommonFunctions.GetAttribute<ViewName>(typeof(TDataSource));
            if (gridAttribute.IsTableFunction)
            {
                string functionParamters = "";
                foreach (var funcParam in TableFunctionParameters)
                {
                    functionParamters += funcParam.ParameterName + ",";
                }
                tableName = "TABLE(" + gridAttribute.Name + "( " + functionParamters.TrimEnd(',') + "))";
            }
            else
                tableName = gridAttribute.Name;
            return tableName;
        }
    }

    public class xmlExportFields
    {
        public string field { get; set; }
        public string header { get; set; }
    }

    public enum ExportType
    {
        GridBind,
        ExportXls,
        ExportCsv,
        ExportPdf
    }

    public enum GridDataParts
    {
        All,
        Data,
        Summary
    }

    public class GridFilter
    {
        public string Value { get; set; }
        public string MatchMode { get; set; }
    }
}
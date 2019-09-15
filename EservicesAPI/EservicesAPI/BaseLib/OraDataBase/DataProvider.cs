using System;
using System.Configuration;
using Newtonsoft.Json;

namespace BaseLib.OraDataBase
{
    public abstract class DataProvider
    {
        private string _connectionString = String.Empty;

        [JsonIgnore]
        public virtual string ConnectionString
        {
            get
            {
                _connectionString = ConfigurationManager.ConnectionStrings["ConnectionDBDefault"].ConnectionString;

                return _connectionString;
            }
        }
    }
}

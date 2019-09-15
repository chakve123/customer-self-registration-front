using System.Collections.Generic;
using BaseLib.Classes;
using BaseLib.Common;
using BaseLib.OraDataBase;
using EservicesLib.OraDatabase.StoredProcedures;
using System.Data;

namespace EservicesLib.Common
{
    public class StaticData
    {
        private static StaticObject<List<ErrorCode>> _errorCodes;

        public static List<ErrorCode> ErrorCodes
        {
            get
            {
                _errorCodes = CommonFunctions.GetStaticObject(_errorCodes, DataProviderManager<PKG_EXCEPTIONS>.Provider.get_error_codes);

                return _errorCodes.Value;
            }
        }

        private static StaticObject<Dictionary<string, int>> _usersCount;
        public static Dictionary<string, int> UsersCount
        {
            get
            {
                _usersCount = CommonFunctions.GetStaticObject(_usersCount, DataProviderManager<PKG_COMMON>.Provider.GetUsersCount, 1440);
                return _usersCount.Value;
            }
        }

        private static StaticObject<DataTable> _countries;
        public static DataTable Countries
        {
            get
            {
                _countries = CommonFunctions.GetStaticObject(_countries, DataProviderManager<PKG_COUNTRIES>.Provider.get_countries);

                return _countries.Value;
            }
        }

        private static StaticObject<DataTable> _measureUnits;
        public static DataTable MeasureUnits
        {
            get
            {
                _measureUnits = CommonFunctions.GetStaticObject(_measureUnits, DataProviderManager<CMN_PKG_MEASURE_UNITS>.Provider.get_units_list);

                return _measureUnits.Value;
            }
        }

    }
}
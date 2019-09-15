using System.Configuration;

namespace BaseLib.OraDataBase
{
    public class DataProviderManager<C> where C : DataProvider, new()
    {
        private static DataProvider defaultProvider;
        static DataProviderManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
            defaultProvider = new C();

            if (defaultProvider == null)
            {
                throw new ConfigurationErrorsException("You must specify a default provider for the feature.");
            }
        }

        public static C Provider
        {
            get
            {
                return (C)defaultProvider;
            }
        }
    }
}

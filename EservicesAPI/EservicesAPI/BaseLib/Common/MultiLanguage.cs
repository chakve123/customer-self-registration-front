using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib.User;

namespace BaseLib.Common
{
    public class MultiLanguage
    {

        #region Properties
        public static Func<string, string> TranslateFunction { get; set; }
        public static int UpdatePeriod { get; set; }
        private List<MultiLanguageItem> _languageObjects;
        public List<MultiLanguageItem> LanguageObjects
        {
            get
            {
                if (_languageObjects == null) // if null create _translation List
                {
                    _languageObjects = new List<MultiLanguageItem>();
                }
                return _languageObjects;
            }
            set
            {
                _languageObjects = value;
            }
        }

        #endregion

        #region Constructor
        public MultiLanguage(Func<string, string> _translateFunction, int _updatePeriod)
        {
            TranslateFunction = _translateFunction;
            UpdatePeriod = _updatePeriod;
        }
        #endregion

        #region Methods

        #endregion

        public string TranslateText(string textGeo)
        {

            if (AuthUser.CurrLanguage != Language.English) return textGeo;

            if (LanguageObjects.Exists(l => l.Geo == textGeo))
            {
                return LanguageObjects.Find(l => l.Geo == textGeo).Eng;
            }
            else
            {
                var newObject = new MultiLanguageItem(textGeo);
                LanguageObjects.Add(newObject);
                return newObject.Eng;
            }
        }

        [Serializable]
        public class MultiLanguageItem
        {
            public string Geo { get; set; }
            private string _eng;
            public string Eng
            {
                get
                {
                    if (_eng == null || WatchedDate == null || (UpdatePeriod > 0 && WatchedDate < DateTime.Now.AddMinutes(-1 * UpdatePeriod)))
                    {
                        _eng = TranslateFunction(Geo);
                        WatchedDate = DateTime.Now;
                    }
                    return _eng;
                }
            }
            public DateTime? WatchedDate { get; set; }

            public MultiLanguageItem(string _geo)
            {
                Geo = _geo;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Common
{
    public class Translator
    {
        public static string ConvertToUni(string sts)
        {
            string dest = string.Empty;
            foreach (char c in sts)
                dest += translateGeoStsChar(c);
            return dest;
        }

        public static string ConvertToSts(string uni)
        {
            string dest = string.Empty;
            foreach (char c in uni)
                dest += translateUnicodeToSts(c);
            return dest;
        }

        private static bool isBreakChar(char c)
        {
            if (char.IsPunctuation(c) || char.IsSeparator(c) || char.IsWhiteSpace(c))
                return true;
            else
                return false;
        }

        private static char translateGeoStsChar(char c)
        {
            switch (c)
            {
                case 'á':
                    c = (char)0;
                    break;
                case 'À':
                    c = 'ა';
                    break;
                case 'Á':
                    c = 'ბ';
                    break;
                case 'Â':
                    c = 'გ';
                    break;
                case 'Ã':
                    c = 'დ';
                    break;
                case 'Ä':
                    c = 'ე';
                    break;
                case 'Å':
                    c = 'ვ';
                    break;
                case 'Æ':
                    c = 'ზ';
                    break;
                case 'Ç':
                    c = 'თ';
                    break;
                case 'È':
                    c = 'ი';
                    break;
                case 'É':
                    c = 'კ';
                    break;
                case 'Ê':
                    c = 'ლ';
                    break;
                case 'Ë':
                    c = 'მ';
                    break;
                case 'Ì':
                    c = 'ნ';
                    break;
                case 'Í':
                    c = 'ო';
                    break;
                case 'Î':
                    c = 'პ';
                    break;
                case 'Ï':
                    c = 'ჟ';
                    break;
                case 'Ð':
                    c = 'რ';
                    break;
                case 'Ñ':
                    c = 'ს';
                    break;
                case 'Ò':
                    c = 'ტ';
                    break;
                case 'Ó':
                    c = 'უ';
                    break;
                case 'Ô':
                    c = 'ფ';
                    break;
                case 'Õ':
                    c = 'ქ';
                    break;
                case 'Ö':
                    c = 'ღ';
                    break;
                case '×':
                    c = 'ყ';
                    break;
                case 'Ø':
                    c = 'შ';
                    break;
                case 'Ù':
                    c = 'ჩ';
                    break;
                case 'Ú':
                    c = 'ც';
                    break;
                case 'Û':
                    c = 'ძ';
                    break;
                case 'Ü':
                    c = 'წ';
                    break;
                case 'Ý':
                    c = 'ჭ';
                    break;
                case 'Þ':
                    c = 'ხ';
                    break;
                case 'ß':
                    c = 'ჯ';
                    break;
                case 'à':
                    c = 'ჰ';
                    break;
                case '\'':
                    c = '\'';
                    break;
                case '~':
                    c = '~';
                    break;
            }
            return c;
        }
        private static char translateUnicodeToSts(char c)
        {
            switch (c)
            {
                case (char)0:
                    c = 'á';
                    break;
                case 'ა':
                    c = 'À';
                    break;
                case 'ბ':
                    c = 'Á';
                    break;
                case 'გ':
                    c = 'Â';
                    break;
                case 'დ':
                    c = 'Ã';
                    break;
                case 'ე':
                    c = 'Ä';
                    break;
                case 'ვ':
                    c = 'Å';
                    break;
                case 'ზ':
                    c = 'Æ';
                    break;
                case 'თ':
                    c = 'Ç';
                    break;
                case 'ი':
                    c = 'È';
                    break;
                case 'კ':
                    c = 'É';
                    break;
                case 'ლ':
                    c = 'Ê';
                    break;
                case 'მ':
                    c = 'Ë';
                    break;
                case 'ნ':
                    c = 'Ì';
                    break;
                case 'ო':
                    c = 'Í';
                    break;
                case 'პ':
                    c = 'Î';
                    break;
                case 'ჟ':
                    c = 'Ï';
                    break;
                case 'რ':
                    c = 'Ð';
                    break;
                case 'ს':
                    c = 'Ñ';
                    break;
                case 'ტ':
                    c = 'Ò';
                    break;
                case 'უ':
                    c = 'Ó';
                    break;
                case 'ფ':
                    c = 'Ô';
                    break;
                case 'ქ':
                    c = 'Õ';
                    break;
                case 'ღ':
                    c = 'Ö';
                    break;
                case 'ყ':
                    c = '×';
                    break;
                case 'შ':
                    c = 'Ø';
                    break;
                case 'ჩ':
                    c = 'Ù';
                    break;
                case 'ც':
                    c = 'Ú';
                    break;
                case 'ძ':
                    c = 'Û';
                    break;
                case 'წ':
                    c = 'Ü';
                    break;
                case 'ჭ':
                    c = 'Ý';
                    break;
                case 'ხ':
                    c = 'Þ';
                    break;
                case 'ჯ':
                    c = 'ß';
                    break;
                case 'ჰ':
                    c = 'à';
                    break;
                case '\'':
                    c = '\'';
                    break;
                case '~':
                    c = '~';
                    break;
            }
            return c;
        }
        public static string TranslateGeoLat(string text)
        {
            int[] eng = new int[] { 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 87, 82, 84, 83, 67, 74, 90, 91, 93, 59, 39, 44, 46, 96 };
            int[] geo = new int[] { 4304, 4305, 4330, 4307, 4308, 4324, 4306, 4336, 4312, 4335, 4313, 4314, 4315, 4316, 4317, 4318, 4325, 4320, 4321, 4322, 4323, 4309, 4332, 4334, 4327, 4310, 4333, 4326, 4311, 4328, 4329, 4319, 4331, 91, 93, 59, 39, 44, 46, 96 };
            string result = string.Empty;
            int ind = 0;
            for (int i = 0; i <= text.Length - 1; i++)
            {
                ind = -1;
                for (int j = 0; j < eng.Length; j++)
                {
                    if (eng[j] == text[i]) { ind = j; break; }

                }
                if (ind == -1)
                    result = result + text[i];
                else
                    result = result + ((char)geo[ind]).ToString();


            }
            return result;
        }
        public static string TranslateLatGeo(string text)
        {
            int[] geo = new int[] { 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 87, 82, 84, 83, 67, 74, 90, 91, 93, 59, 39, 44, 46, 96 };
            int[] eng = new int[] { 4304, 4305, 4330, 4307, 4308, 4324, 4306, 4336, 4312, 4335, 4313, 4314, 4315, 4316, 4317, 4318, 4325, 4320, 4321, 4322, 4323, 4309, 4332, 4334, 4327, 4310, 4333, 4326, 4311, 4328, 4329, 4319, 4331, 91, 93, 59, 39, 44, 46, 96 };
            string result = string.Empty;
            int ind = 0;
            for (int i = 0; i <= text.Length - 1; i++)
            {
                ind = -1;
                for (int j = 0; j < eng.Length; j++)
                {
                    if (eng[j] == text[i]) { ind = j; break; }

                }
                if (ind == -1)
                    result = result + text[i];
                else
                    result = result + ((char)geo[ind]).ToString();

            }
            return result;
        }
    }
}

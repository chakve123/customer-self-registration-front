using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using BaseLib.OraDataBase;
using EservicesLib.OraDatabase.StoredProcedures;

namespace EservicesAPI.App_Start
{
    /// <summary>
    /// ბაზიდან მომაქვს Key და IV აპლიკაციის დასტარტვის დროს.
    ///
    /// RouterValues - ყოველი დაშიფრული route ის განშიფრვის შემდეგ ინახავს მეხსიერებაში.
    ///
    /// ყოველ  PeriodToRefreshDict ით პერიოდულად წაიშალოს ყველა ჩანაწერი 
    /// 
    /// </summary>
    public static class RouteEncryptionService
    {
        private static byte[] Key { get; set; }
        private static byte[] IV { get; set; }
        
        /*გამოიძახება appStart ის დროს*/
        public static void InitServiceVariables()
        {
            var keysFromDb = DataProviderManager<PKG_CONFIGURATION>.Provider.GetRouteEncryptionKeys();

            keysFromDb.TryGetValue("RouteEncryptionKey", out byte[] key);
            keysFromDb.TryGetValue("RouteEncryptionIV", out byte[] iv);
            Key = key;
            IV = iv;

            /*ToDO: წამოიღე Key და IV ბაზიდან*/

            #region ახალი key სა და IV ს დაგენერირება და ბაზაში შენახვა

            //string keyString = "ჩემი უძალიან უსაიდუმლოესი გასაღები და კიდევ ტექსტი იმიტომ რომ მოკლეაო მითხრეს";

            //var keyBytesFull = Encoding.UTF32.GetBytes(keyString);
            //var keyBytes = keyBytesFull.Take(32).ToArray();

            //var rj = new RijndaelManaged()
            //{
            //    Key = keyBytes
            //};
            //Key = rj.Key;
            //IV = rj.IV;


            //DataProviderManager<PKG_CONFIGURATION>.Provider.SetRouteEncryptionKeys(Key, IV);

            #endregion


        }

        public static string GetEncrypted(string routerPlainTxt)
        {
            if (Key == null || IV == null)
            {
                return routerPlainTxt;
            }
            var decryptedBytes = EncryptStringToBytes(routerPlainTxt, Key, IV);
            var decryptedString = Convert.ToBase64String(decryptedBytes);

            return decryptedString;
        }

        public static bool EncryptionUsed()
        {
            if (IV == null || Key == null)
            {
                return false;
            }
            if (IV.Length == 0|| Key.Length == 0)
            {
                return false;
            }

            return true;
        }
        
        private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {

                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

    }
}
using System;
using System.Configuration;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EservicesAPI.Auth
{
    public class TokenManager
    {
        private static readonly int tokenExpiresIn = ConfigurationManager.AppSettings["token_expires_in"].ToNumber<int>();

        private static void FillAdditionalProp(AuthUser authUser)
        {
            if (authUser.SubUserID != 0 && authUser.HasPermission(Modules.DutyFree))
                authUser.AdditionalProperties.Add("DutyFree", DataProviderManager<PKG_AUTHENTICATION>.Provider.get_dutyfree_point(authUser.SubUserID));

            if (authUser.SubUserID != 0 && authUser.HasPermission(Modules.TaxFree))
                authUser.AdditionalProperties.Add("TaxFree", DataProviderManager<PKG_AUTHENTICATION>.Provider.get_taxfree_license(authUser.SubUserID));
        }

        public static void CreateToken(AuthUser authUser)
        {
            authUser.AccessToken = Guid.NewGuid() + "-" + DateTime.Now.ToString("ddMMyyyyhhmss");
            authUser.ExpiresIn = tokenExpiresIn * 60;
            authUser.CreateDate = DateTime.Now;

            FillAdditionalProp(authUser);

            var redisKey = $"AuthUser:{authUser.AccessToken}";
            var redisConnection = RedisCache.Current();
            var tokenTimeOut = TimeSpan.FromMinutes(tokenExpiresIn);

            redisConnection.SetObject(redisKey, JObject.FromObject(authUser), tokenTimeOut);
        }

        public static void SetToken(AuthUser authUser, string token, TimeSpan? ttl = null)
        {
            authUser.AccessToken = token;
            authUser.ExpiresIn = tokenExpiresIn * 60;
            authUser.CreateDate = DateTime.Now;

            var redisKey = $"AuthUser:{authUser.AccessToken}";
            var redisConnection = RedisCache.Current();

            redisConnection.SetObject(redisKey, JObject.FromObject(authUser),ttl);
        }
        
        public static AuthUser GetAuthUser(string accessToken, bool refresh = true)
        {
            var redisConnection = RedisCache.Current();
            var redisKey = $"AuthUser:{accessToken}";

            var authUser = redisConnection.GetObject<AuthUser>(redisKey);
            if (authUser != null && refresh)
            {
                var tokenTimeOut = TimeSpan.FromMinutes(tokenExpiresIn);
                redisConnection.KeyTimeToLiveAsync(redisKey, tokenTimeOut);

                //var tokenTimeOut = TimeSpan.FromMinutes(tokenExpiresIn);
                //authUser.ModifiedDate = DateTime.Now;
                //redisConnection.SetObject(redisKey, JObject.FromObject(authUser), tokenTimeOut);
            }
            return authUser;
        }

        public static void UpdateAuthUser(string accessToken, AuthUser authUser)
        {
            var redisConnection = RedisCache.Current();
            var redisKey = $"AuthUser:{accessToken}";

            if (authUser != null)
            {
                var tokenTimeOut = TimeSpan.FromMinutes(tokenExpiresIn);
                authUser.ModifiedDate = DateTime.Now;
                //var json = JsonConvert.SerializeObject(authUser);
                redisConnection.SetObject(redisKey, JObject.FromObject(authUser), tokenTimeOut);
            }
        }

        public static void DeleteToken(string accessToken)
        {
            var redisConnection = RedisCache.Current();
            var redisKey = $"AuthUser:{accessToken}";
            redisConnection.Remove(redisKey);

            //var authInfo = redisConnection.GetObject<AuthInfo>(redisKey);
            //if (authInfo != null) redisConnection.Remove(redisKey);
        }

        public static string GetConfirmCode(string redisKey)
        {
            var redisConnection = RedisCache.Current();
            return redisConnection.GetObject<string>(redisKey);
        }

        public static void SetConfirmCode(string redisKey, string code, int minutes = 0)
        {
            var redisConnection = RedisCache.Current();
            if (code != null)
            {
                var tokenTimeOut = TimeSpan.FromMinutes(minutes == 0 ? tokenExpiresIn : minutes);
                redisConnection.SetObject<string>(redisKey, code, tokenTimeOut);
            }
        }

        public static void DeleteConfirmCode(string redisKey)
        {
            var redisConnection = RedisCache.Current();
            redisConnection.Remove(redisKey);
        }
    }
}
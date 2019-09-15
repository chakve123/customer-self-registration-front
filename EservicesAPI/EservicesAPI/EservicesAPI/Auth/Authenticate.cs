using System;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BaseLib.Common;
using BaseLib.OraDataBase;
using EservicesAPI.Common;
using EservicesLib.OraDatabase.StoredProcedures;
using EservicesLib.User;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Util;

namespace EservicesAPI.Auth
{
    public class Authenticate : AuthorizationFilterAttribute
    {
        public bool Refresh = true;

        public Modules Module { get; set; }

        public Permissions Permission { get; set; }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            AuthUser authUser;

            var authHeader = actionContext.Request.Headers.Authorization;
            if (authHeader != null)
            {
                var schema = actionContext.Request.Headers.Authorization.Scheme;
                var token = actionContext.Request.Headers.Authorization.Parameter;

                /*ჩვენთვის დეველოპერებისთვის ტესტირების გასაადვილებლად*/
                if (token == Guid.Empty.ToString())
                {
                    var redisConnection = RedisCache.Current();
                    var redisKey = $"AuthUser:{token}";

                    authUser = redisConnection.GetObject<AuthUser>(redisKey);

                    if (authUser == null)
                    {
                        var ttl = TimeSpan.FromDays(1);
                        authUser = GetTestUser();
                        TokenManager.SetToken(authUser, token, ttl);
                    }
                }
                else
                {   /*ყველა სხვა*/
                    authUser = TokenManager.GetAuthUser(token, Refresh);
                }

                if (authUser == null || schema.ToLower() != "bearer")
                {
                    ResponseBuilder.ThrowError(-104);
                    return Task.FromResult<object>(null);
                }
            }
            else
            {
                ResponseBuilder.ThrowError(-104);
                return Task.FromResult<object>(null);
            }

            authUser.PermitForAnyRecord = HasPermission(authUser, Module, Permissions.PermitForAnyRecord);
            
            actionContext.Request.Properties.Add("AuthUser", authUser);

            if (Module == Modules.None) return Task.FromResult<object>(null);

            var hasPermission = HasPermission(authUser, Module, Permission);
            if (!hasPermission)
            {
                ResponseBuilder.ThrowError(-105);
                return Task.FromResult<object>(null);
            }

            return Task.FromResult<object>(null);
        }

        private bool HasPermission(AuthUser authUser, Modules module, Permissions permission = Permissions.Read)
        {
            return authUser.Modules.Exists(m => m.ID == (int)module) &&
                authUser.Modules.Find(m => m.ID == (int)module).HasPermission(permission);
        }



        private AuthUser GetTestUser()
        {
            AuthUser authUser;
            authUser = DataProviderManager<PKG_AUTHENTICATION>.Provider.authenticate("206322102", "123456", 1, null, "0.0.0.0", string.Empty, string.Empty);
            DataProviderManager<PKG_AUTHENTICATION>.Provider.get_user_data(
                authUser.SubUserID == 0 ? authUser.ID : authUser.SubUserID,
                authUser.UserType,
                authUser);

            return authUser;
        }
    }
}
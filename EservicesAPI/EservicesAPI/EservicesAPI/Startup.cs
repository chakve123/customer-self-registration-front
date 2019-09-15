using System;
using System.Web.Http;
using EservicesAPI.Auth;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(EservicesAPI.Startup))]
namespace EservicesAPI
{
    public class Startup
    {
        //public static OAuthAuthorizationServerOptions oAuthOptions { get; set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            //var oAuthOptions = new OAuthAuthorizationServerOptions
            //{
            //    AllowInsecureHttp = true,
            //    TokenEndpointPath = new PathString("/Authenticate"),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(480),
            //    Provider = new OAuthServerProvider()
            //};

            //oAuthOptions = new OAuthAuthorizationServerOptions
            //{
            //    AllowInsecureHttp = true,
            //    TokenEndpointPath = new PathString("/Authenticate"),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(480),
            //    Provider = new OAuthServerProvider()
            //};

            //app.Use<AuthenticationMiddleware>();
            //app.UseOAuthAuthorizationServer(oAuthOptions);
            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            app.UseWebApi(config);
        }
    }
}

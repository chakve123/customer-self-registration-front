namespace EservicesAPI.Auth
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    //public class OAuthServerProvider : OAuthAuthorizationServerProvider
    //{
    //    public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
    //    {
    //        context.Validated();
    //        return base.ValidateClientAuthentication(context);
    //    }

    //    public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
    //    {
    //        int error;
    //        string errorDesc;

    //        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

    //        try
    //        {
    //            var request = context.Request.ReadFormAsync().Result;
    //            var authToken = request.Get("auth_token");

    //            if (authToken == null || string.IsNullOrEmpty(authToken))
    //            {
    //                error = -3;
    //                errorDesc = ConfigurationManager.AppSettings["invalid_auth_key"];
    //                context.SetError(error.ToString(), errorDesc);
    //                context.Response.Headers.Add(Constants.OwinChallengeFlag, new[] { ((int)HttpStatusCode.Unauthorized).ToString() });
    //                return base.GrantResourceOwnerCredentials(context);
    //            }

    //            var mobileCode = request.Get("pin_code");
    //            //var deviceInfo = JsonConvert.DeserializeObject<DeviceInfo>(request.Get("device_info"));

    //            var deviceInfo = new DeviceInfo
    //            {
    //                device_code = request.Get("device_code"),
    //                address = request.Get("address"),
    //                browser = request.Get("browser"),
    //                oper_system = request.Get("oper_system")
    //            };

    //            var currUser = DataProviderManager<PKG_USERS>.Provider.auth_user(authToken, mobileCode, out error, out errorDesc);
    //            if (currUser != null)
    //            {
    //                var data = new Dictionary<string, string>();
    //                data.Add("error", error.ToString());
    //                data.Add("error_description","");

    //                var props = new AuthenticationProperties(data);
    //                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

    //                //Add User Data into Token String
    //                identity.AddClaim(new Claim("UserID", currUser.ID.ToString(), ClaimValueTypes.String));
    //                identity.AddClaim(new Claim("SubUserID", currUser.SubUserID.ToString(), ClaimValueTypes.String));
    //                identity.AddClaim(new Claim("UnID", currUser.UnID.ToString(), ClaimValueTypes.String));
    //                identity.AddClaim(new Claim("Tin", currUser.Tin, ClaimValueTypes.String));
    //                identity.AddClaim(new Claim("IsVatPayer", currUser.IsVatPayer.ToString(), ClaimValueTypes.Boolean));
    //                identity.AddClaim(new Claim("UserType", currUser.UserType.ToString(), ClaimValueTypes.String));

    //                if (!string.IsNullOrEmpty(deviceInfo.device_code))
    //                {
    //                    DataProviderManager<PKG_USERS>.Provider.save_device_code(currUser.ID, currUser.SubUserID, deviceInfo);
    //                }

    //                // Generate Token
    //                var ticket = new AuthenticationTicket(identity, props);
    //                context.Validated(ticket);
    //                context.Request.Context.Authentication.SignIn(identity);
    //            }
    //            else
    //            {
    //                context.Rejected();
    //                context.SetError(error.ToString(), errorDesc);
    //                context.Response.Headers.Add(Constants.OwinChallengeFlag, new[] {((int) HttpStatusCode.Unauthorized).ToString()});
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            CommonFunctions.CatchExceptions(ex);

    //            error = -1;
    //            errorDesc = ConfigurationManager.AppSettings["error_text"];
    //            context.SetError(error.ToString(), errorDesc);
    //            context.Response.Headers.Add(Constants.OwinChallengeFlag, new[] { ((int)HttpStatusCode.InternalServerError).ToString() });
    //        }
    //        return base.GrantResourceOwnerCredentials(context);
    //    }

    //    public override Task TokenEndpoint(OAuthTokenEndpointContext context)
    //    {
    //        foreach (var property in context.Properties.Dictionary)
    //        {
    //            context.AdditionalResponseParameters.Add(property.Key, property.Value);
    //        }
    //        return Task.FromResult<object>(null);
    //    }
    //}
}
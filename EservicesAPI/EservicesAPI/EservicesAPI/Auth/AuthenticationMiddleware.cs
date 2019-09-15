using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace EservicesAPI.Auth
{
    //public class Constants
    //{
    //    public const string OwinChallengeFlag = "X-Challenge";
    //}

    //public class AuthenticationMiddleware : OwinMiddleware
    //{
    //    public AuthenticationMiddleware(OwinMiddleware next) : base(next) { }

    //    public override async Task Invoke(IOwinContext context)
    //    {
    //        await Next.Invoke(context);

    //        //if (context.Response.StatusCode == 400 && context.Response.Headers.ContainsKey(Constants.OwinChallengeFlag))
    //        //{
    //        //    var headerValues = context.Response.Headers.GetValues(Constants.OwinChallengeFlag);
    //        //    context.Response.StatusCode = Convert.ToInt16(headerValues.FirstOrDefault());
    //        //    context.Response.Headers.Remove(Constants.OwinChallengeFlag);
    //        //}

    //        //var status = context.Response.StatusCode;
    //    }
    //}
}
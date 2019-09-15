using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EservicesAPI.Common
{
    public class CustomMessageHandler : DelegatingHandler
    {
        //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    var response = await base.SendAsync(request, cancellationToken);

        //    //if (request.Headers.Authorization != null
        //    //    && request.Headers.Authorization.Scheme == "bearer"
        //    //    && response.StatusCode == HttpStatusCode.Unauthorized)
        //    //{
        //    //    //Build Common Response Message All UnAuthorized Actions

        //    //    response = ResponseBuilder.CreateErrorResponse(HttpStatusCode.Unauthorized, -4, ConfigurationManager.AppSettings["invalid_token"]);
        //    //}

        //    return response;
        //}
    }
}
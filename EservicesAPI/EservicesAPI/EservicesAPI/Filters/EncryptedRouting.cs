using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using EservicesAPI.App_Start;

namespace EservicesAPI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class EncryptedRouting : DirectRouting, IDirectRouteFactory
    {
       public EncryptedRouting():base(true)
        {
            
        }

       // public bool DirectRoutingEnabled = true;

        public RouteEntry CreateRoute(DirectRouteFactoryContext context)
        {
            throw new NotImplementedException();
        }

        RouteEntry IDirectRouteFactory.CreateRoute(DirectRouteFactoryContext context)
        {
            
            return base.createRouteInternal(context);
        }
    }
}
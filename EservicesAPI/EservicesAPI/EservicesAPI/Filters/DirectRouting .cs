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

//using EservicesAPI.App_Start;

namespace EservicesAPI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class DirectRouting : Attribute, IDirectRouteFactory, IHttpRouteInfoProvider
    {
        public DirectRouting(bool directRoutingEnabled = false)
        {
            this.DirectRoutingEnabled = directRoutingEnabled;
        }

        public DirectRouting()
        {

        }

        public string Name { get; }

        public string Template { get; }

        public int Order { get; }

        public bool DirectRoutingEnabled { get; }

        protected RouteEntry CreateRoute(DirectRouteFactoryContext context)
        {
            throw new NotImplementedException();
        }

        RouteEntry IDirectRouteFactory.CreateRoute(DirectRouteFactoryContext context)
        {
            return createRouteInternal(context);
        }


        protected RouteEntry createRouteInternal(DirectRouteFactoryContext context)
        {

            string actionName = context.Actions.FirstOrDefault().ActionName;
            string controllerName = context.Actions.FirstOrDefault().ControllerDescriptor.ControllerName;
            string route = $"{controllerName}/{actionName}";

            if (DirectRoutingEnabled && RouteEncryptionService.EncryptionUsed())
            {

                route = RouteEncryptionService.GetEncrypted(route);
            }

            IDirectRouteBuilder builder = context.CreateBuilder(route);
            //builder.Name = this.Name;
            //builder.Order = this.Order;
            return builder.Build();
        }
    }
}
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace EservicesAPI.Filters
{
    public class Grid : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //object data;

            //var content = actionContext.Request.Content;


            //if ()
            //{
            //    GridData gridData = (GridData) data;
            //    gridData.Criteria = "Fitler Criteria";
            //    actionContext.Request.Properties.Add("GridData", gridData);
            //}
        }
    }
}
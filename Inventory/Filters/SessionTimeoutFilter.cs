using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Inventory.Filters
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["LoginUserID"] == null)
            {
                var Url = new UrlHelper(filterContext.RequestContext);
                var url = Url.Action("Login", "User");
                filterContext.Result = new RedirectResult(url);
                return;
             
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
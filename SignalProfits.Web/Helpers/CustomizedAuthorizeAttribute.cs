using SignalProfits.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalProfits.Web.Helpers
{
    public class CustomizedAuthorizeAttribute : AuthorizeAttribute
    {
        public int Permission { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "" || HttpContext.Current.Session["User"] == null)
            {
                filterContext.Result = new RedirectResult("~/login");
            }
            else
            {
                try
                {
                    Login login = HttpContext.Current.Session["User"] as Login;
                    var rd = HttpContext.Current.Request.RequestContext.RouteData;                    
                    string currentController = rd.GetRequiredString("controller");
                    if ((currentController == "Customer" || currentController == "Log") && !login.IsAdmin)
                        filterContext.Result = new HttpUnauthorizedResult();

                    if (HttpContext.Current.User != null)
                    {
                        int userID = Convert.ToInt32(HttpContext.Current.User.Identity.Name);                       
                    }
                }
                catch
                {
                    return;
                }
            }
        }
    }
}
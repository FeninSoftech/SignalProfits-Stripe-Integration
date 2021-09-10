using SignalProfits.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalProfits.Web.Helpers
{
    public class SessionManager
    {
        public static Login LoginSession
        {
            get
            {
                Login login = HttpContext.Current.Session["User"] as Login;
                if (login != null)
                    return login;
                else
                    return new Login();
            }
        }
        
    }
}
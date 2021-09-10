using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SignalProfits.Web.Helpers
{
    public class AppConfiguration
    {
        public static string StripeKey { get { return ConfigurationManager.AppSettings["StripeKey"].ToString(); } }
        public static string StripeBaseUrl { get { return ConfigurationManager.AppSettings["StripeBaseUrl"].ToString(); } }
        public static string EmailFrom { get { return ConfigurationManager.AppSettings["EmailFrom"].ToString(); } }
        public static Int32 PasswordResetExpiry { get { return Convert.ToInt32(ConfigurationManager.AppSettings["PasswordResetExpiry"]); } }
    }
}
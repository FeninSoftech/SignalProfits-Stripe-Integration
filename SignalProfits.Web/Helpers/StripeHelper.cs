using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using SignalProfits.Web.Model;

namespace SignalProfits.Web.Helpers
{
    public class StripeHelper
    {
        public static List<CustomerModel> SearchCustomer(string email)
        {
            List<CustomerModel> customers = new List<CustomerModel>();

            string searchUrl = Path.Combine(AppConfiguration.StripeBaseUrl, "search?query='" + email + "'&prefix=false");
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(searchUrl);
            objRequest.Method = WebRequestMethods.Http.Get;
            objRequest.Accept = "application/x-www-form-urlencoded";
            objRequest.Headers["Authorization"] = "bearer " + AppConfiguration.StripeKey;

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            if (objResponse.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = objResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {

                        var customerStr = reader.ReadToEnd();
                        Rootobject stripeObjects = JsonConvert.DeserializeObject<Rootobject>(customerStr);

                        if (stripeObjects != null && stripeObjects.data != null && stripeObjects.data.Count() > 0)
                        {                           
                            foreach (var stripeObj in stripeObjects.data.Where(X => X._object == "customer"))
                            {
                                customers.Add(new CustomerModel
                                {
                                    CustomerId = stripeObj.id,
                                    Email = stripeObj.email,
                                    Description = stripeObj.description
                                });

                                return customers;
                            }
                        }
                    }
                }
            }

            return customers;
        }
    }

    public class Rootobject
    {
        public int count { get; set; }
        public StripeObject[] data { get; set; }
        public string url { get; set; }
        public object scores { get; set; }
        public object facets { get; set; }
    }

    public class StripeObject
    {
        public string id { get; set; }
        [JsonProperty("object")]
        public string _object { get; set; }
        public object business_logo { get; set; }
        public string description { get; set; }
        public object business_url { get; set; }
        public bool charges_enabled { get; set; }
        public string country { get; set; }
        public int created { get; set; }
        public string default_currency { get; set; }
        public bool details_submitted { get; set; }
        public object display_name { get; set; }
        public string email { get; set; }
    }
}
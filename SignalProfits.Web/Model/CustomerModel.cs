using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalProfits.Web.Model
{
    public class CustomerModel
    {
        public string Currency { get; internal set; }
        public string CustomerId { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
    }

    public class CustomerSubscriptionModel
    {
        public string CustomerId { get; set; }
        public string SubscriptionId { get; set; }
        public string CreatedDate { get; set; }
        public string Interval { get; internal set; }
        public int IntervalCount { get; internal set; }
        public string PlanName { get; internal set; }    
        public string CurrentPlan { get; internal set; }
        public string SpMnthPlanID { get; set; }
        public string SpSemiPlanID { get; set; }

    }    
}
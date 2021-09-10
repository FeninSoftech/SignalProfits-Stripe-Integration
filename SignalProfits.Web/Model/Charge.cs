using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalProfits.Web.Model
{
    public class Charge
    {
       public string ChargeId { get; set; }
       public string CustomerId { get; set; }
       public decimal Amount { get; set; }
       public bool Paid { get; set; }
       public string Receipt_email { get; set; }
       public string Currency { get; set; }
       public string PaymentDate { get; set; }
        public string RefundDate { get; set; }
        public string ChargedStatus { get; set; }
       public bool RefundedStatus { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stripe;
using SignalProfits.Web.Model;
using SignalProfits.Web.Helpers;

namespace SignalProfits.Web.Controllers
{
    public class ChargeController : Controller
    {
        // GET: Charge
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCustomerCharges(string id)
        {
            Stripe.StripeChargeService stripeService = new Stripe.StripeChargeService();

            StripeList<StripeCharge> charges = stripeService.List(new StripeChargeListOptions()
            {
                CustomerId = id
            });

            List<Charge> cuscharges = new List<Charge>();

            var cus_charges = charges.Where(X => X.Object == "charge");
            foreach (var stripeObj in cus_charges)
            {
                Charge newCharge = new Charge();

                newCharge.PaymentDate = stripeObj.Created.ToString("MM/dd/yyyy HH:mm");
                newCharge.ChargeId = stripeObj.Id;
                newCharge.Amount = (stripeObj.Amount / 100.00M);
                newCharge.Paid = stripeObj.Paid;
                newCharge.Receipt_email = stripeObj.ReceiptNumber;
                newCharge.Currency = stripeObj.Currency;
                newCharge.ChargedStatus = stripeObj.Status;
                newCharge.CustomerId = id;
                newCharge.RefundedStatus = stripeObj.Refunded;
                if (stripeObj.Source != null && stripeObj.Source.Card != null)
                {
                    newCharge.CardName = stripeObj.Source.Card.Name;
                    newCharge.CardNumber = stripeObj.Source.Card.Last4;
                }
                if (stripeObj.Refunds != null && stripeObj.Refunds.Count() > 0)
                {
                    newCharge.RefundDate = stripeObj.Refunds.ElementAt(0).Created.ToString("MM/dd/yyyy HH:mm");
                }

                cuscharges.Add(newCharge);
            }

            return View("_CustomerCharges", cuscharges);
        }


        public JsonResult CancelCharges(string ChargeId)
        {
            try
            {
                Stripe.StripeRefundService stripeService = new Stripe.StripeRefundService();
                stripeService.Create(ChargeId);
                ErrorNotesHelper.AddNote("A refund is created for charge '" + ChargeId + "' by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                return Json(new { status = true, message = "<strong>Success!</strong> Refund created successfully!" });
            }
            catch (Exception ex)
            {
                ErrorNotesHelper.AddNote("Error '" + ex.Message + "' in creating refund for charge '" + ChargeId + "' by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), NoteType.Error);
                ErrorNotesHelper.AddError(ex);
                return Json(new { status = false, message = "<strong>Error!</strong> " + ex.Message });
            }
        }

    }
}
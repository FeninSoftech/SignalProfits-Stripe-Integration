using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalProfits.Web.Model;
using SignalProfits.Web.Helpers;

namespace SignalProfits.Web.Controllers
{
    [CustomizedAuthorizeAttribute]
    public class CustomerCardController : Controller
    {
        public ActionResult LoadCustomerCard(string id)
        {
            Stripe.StripeCustomerService stripeService = new Stripe.StripeCustomerService();
            StripeCustomer customer = stripeService.Get(id);
            string defaultSourceId = customer.DefaultSourceId;
            CustomerCard custCard = new CustomerCard();
            custCard.CustomerId = id;
            if (customer.Sources != null && customer.Sources.Count() > 0)
            {
                custCard.Cards = new List<Card>();
                foreach (var card in customer.Sources.Select(X => X.Card))
                {
                    bool isDefault = !string.IsNullOrWhiteSpace(defaultSourceId) && card.Id == defaultSourceId ? true : false;
                    custCard.Cards.Add(new Card() { IsDefault = isDefault, CardId = card.Id, CardNo = card.Last4, ExpMonth = card.ExpirationMonth, ExpYear = card.ExpirationYear, Name = card.Name ?? string.Empty });
                }
            }

            return PartialView("Index", custCard);
        }

        [HttpPost]
        public JsonResult EditCard(CustomerCard custCard)
        {
            Stripe.StripeCardService stripeService = new Stripe.StripeCardService();
            try
            {
                if (custCard != null)
                {
                    StripeCardUpdateOptions updateCard = new StripeCardUpdateOptions();
                    updateCard.ExpirationMonth = custCard.ExpMonth;
                    updateCard.ExpirationYear = custCard.ExpYear;
                    updateCard.Name = custCard.Name;
                    stripeService.Update(custCard.CustomerId, custCard.CardId, updateCard);

                    Stripe.StripeCustomerService stripeCustomer = new Stripe.StripeCustomerService();
                    StripeCustomerUpdateOptions cusUpdate = new StripeCustomerUpdateOptions();
                    cusUpdate.DefaultSource =  custCard.IsDefault ? custCard.CardId : null;
                    stripeCustomer.Update(custCard.CustomerId, cusUpdate);
                }
                ErrorNotesHelper.AddNote("Customer card ('" + custCard.Name + "') updated by user " + SessionManager.LoginSession.Name + " on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                return Json(new { status = true, message = "<strong>Success!</strong> Card updated successfully!" });
            }
            catch (Exception ex)
            {
                ErrorNotesHelper.AddNote("Error '" + ex.Message + "' in edit card '" + custCard.CustomerId + "' by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), NoteType.Error);
                ErrorNotesHelper.AddError(ex);
                return Json(new { status = false, message = "<strong>Error!</strong> " + ex.Message });
            }
        }


        public JsonResult AddCard(Card custCard)
        {
            Stripe.StripeCardService stripeService = new Stripe.StripeCardService();
            try
            {
                if (custCard != null)
                {
                    StripeCardCreateOptions AddCard = new StripeCardCreateOptions();
                    AddCard.SourceCard = new SourceCard();
                    AddCard.SourceCard.Number = custCard.CardNo;
                    AddCard.SourceCard.ExpirationMonth = custCard.ExpMonth;
                    AddCard.SourceCard.ExpirationYear = custCard.ExpYear;
                    AddCard.SourceCard.Name = custCard.Name;
                    StripeCard newCard = stripeService.Create(custCard.CustomerId, AddCard);

                    Stripe.StripeCustomerService stripeCustomer = new Stripe.StripeCustomerService();
                    StripeCustomerUpdateOptions cusUpdate = new StripeCustomerUpdateOptions();
                    cusUpdate.DefaultSource = custCard.IsDefault ? newCard.Id : null;
                    stripeCustomer.Update(custCard.CustomerId, cusUpdate);
                }

                ErrorNotesHelper.AddNote("Customer card ('" + custCard.Name + "') added by user " + SessionManager.LoginSession.Name + " on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                return Json(new { status = true, message = "<strong>Success!</strong> Card added successfully!" });
            }
            catch (Exception ex)
            {
                ErrorNotesHelper.AddNote("Error '" + ex.Message + "' in add card '" + custCard.CustomerId + "' by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), NoteType.Error);
                ErrorNotesHelper.AddError(ex);
                return Json(new { status = false, message = "<strong>Error!</strong> " + ex.Message });
            }
        }

    }
}
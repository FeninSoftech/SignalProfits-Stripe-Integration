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
    public class CustomerAddressController : Controller
    {
        public ActionResult LoadAddressDetails(string id)
        {
            Stripe.StripeCustomerService stripeService = new Stripe.StripeCustomerService();
            StripeCustomer customer = stripeService.Get(id);

            CustomerAddress custadds = new CustomerAddress();
            custadds.CustomerId = id;
            if (customer.Shipping != null && customer.Shipping.Address != null)
            {                
                custadds.Name = customer.Shipping.Name;
                custadds.Phone = customer.Shipping.Phone;
                custadds.City = customer.Shipping.Address.City;
                custadds.Country = customer.Shipping.Address.Country;             
                custadds.AddressLine1 = customer.Shipping.Address.Line1;
                custadds.AddressLine2 = customer.Shipping.Address.Line2;
                custadds.ZipCode = customer.Shipping.Address.PostalCode;
                custadds.State = customer.Shipping.Address.State;
            }
            return PartialView("Index", custadds);
        }

        [HttpPost]
        public ActionResult UpdateCustomerAddress(CustomerAddress custadds)
        {
            try
            {
                Stripe.StripeCustomerService stripeService = new Stripe.StripeCustomerService();
                if (custadds != null)
                {                    
                    StripeShippingOptions shipping = new StripeShippingOptions();
                    shipping.Name = custadds.Name;
                    shipping.Country = custadds.Country;
                    shipping.Phone = custadds.Phone;
                    shipping.Line1 = custadds.AddressLine1;
                    shipping.Line2 = custadds.AddressLine2;
                    shipping.CityOrTown = custadds.City;
                    shipping.PostalCode = custadds.ZipCode;
                    shipping.State = custadds.State;
                    StripeCustomerUpdateOptions updateadds = new StripeCustomerUpdateOptions();
                    updateadds.Shipping = shipping;
                    stripeService.Update(custadds.CustomerId, updateadds);
                    TempData["SuccessMsg"] = "Customer address updated successfully";
                    ErrorNotesHelper.AddNote("Customer address updated by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                }
            }
            catch(Exception ex)
            {
                ErrorNotesHelper.AddNote("Error '" + ex.Message + "' in updating customer address for customer '" + custadds.CustomerId + "' by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), NoteType.Error);
                ErrorNotesHelper.AddError(ex); 
            }

            return RedirectToAction("Index", "Home");
        }
           
    }
}
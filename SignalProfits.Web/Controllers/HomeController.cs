using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SignalProfits.Web.Model;
using SignalProfits.Web.Helpers;
using Stripe;
using SignalProfits.Web.Data;

namespace SignalProfits.Web.Controllers
{
    [CustomizedAuthorizeAttribute]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string email = string.Empty;
            if (TempData["CustomeEmail"] != null)
            {
                email = TempData["CustomeEmail"].ToString();               
            }

            if (Session["User"] != null)
            {
                Login login = Session["User"] as Login;
                if (login != null)
                {
                    email = login.Email;                  
                }
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                List<CustomerModel> customers = GetStripeCustomer(email);
                ViewBag.showCutomerList = 1;
                return View("Index", customers);
            }
            
            List<CustomerModel> customerModel = new List<CustomerModel>();
            ViewBag.showCutomerList = 0;
            return View(customerModel);
        }

        private List<CustomerModel> GetStripeCustomer(string email)
        {
            List<CustomerModel> customers = new List<CustomerModel>();

            if (Session["User"] != null)
            {
                Login login = Session["User"] as Login;
                if (login != null && !string.IsNullOrWhiteSpace(login.StripeCustomerId))
                {
                    Stripe.StripeCustomerService stripeService = new Stripe.StripeCustomerService();
                    StripeCustomer customer = stripeService.Get(login.StripeCustomerId);
                    if (customer != null)
                    {
                        CustomerModel model = new CustomerModel();
                        model.Description = customer.Description;
                        model.Email = customer.Email;
                        model.CustomerId = customer.Id;
                        model.Currency = customer.Currency;

                        customers.Add(model);

                        return customers;
                    }
                }
            }
            else
            {
                customers = StripeHelper.SearchCustomer(email);
            }

            return customers;
        }

        [HttpPost]
        public ActionResult SearchCustomer(FormCollection frm)
        {
            List<CustomerModel> customerModel = GetStripeCustomer(frm["email"].ToString());

            ViewBag.showCutomerList = 1;

            return View("Index", customerModel);
        }

        public ActionResult LoadCustomerDetail(string id)
        {
            Stripe.StripeCustomerService stripeService = new Stripe.StripeCustomerService();
            StripeCustomer customer = stripeService.Get(id);

            CustomerModel model = new CustomerModel();
            model.Description = customer.Description;
            model.Email = customer.Email;
            model.CustomerId = customer.Id;
            model.Currency = customer.Currency;

            return PartialView("_customerDetails", model);
        }

        public ActionResult LoadCustomerSubscriptions(string id)
        {
            Stripe.StripeSubscriptionService stripeService = new Stripe.StripeSubscriptionService();

            StripeList<StripeSubscription> subscription = stripeService.List(new StripeSubscriptionListOptions()
            {
                CustomerId = id
            });

            Stripe.StripePlanService stripePlans = new Stripe.StripePlanService();
            StripeList<StripePlan> plans = stripePlans.List();

            CustomerSubscriptionModel cusSubs = new CustomerSubscriptionModel();

            var subscriptions = subscription.Where(X => X.Object == "subscription");
            foreach (var stripeObj in subscriptions)
            {
                if (stripeObj.StripePlan != null)
                {
                    cusSubs.CreatedDate = stripeObj.Created.HasValue ? stripeObj.Created.Value.ToString("MM/dd/yyyy") : string.Empty;
                    cusSubs.SubscriptionId = stripeObj.Id;
                    cusSubs.Interval = stripeObj.StripePlan.Interval;
                    cusSubs.IntervalCount = stripeObj.StripePlan.IntervalCount;
                    cusSubs.PlanName = stripeObj.StripePlan.Name;

                    if (stripeObj.StripePlan.Metadata != null && stripeObj.StripePlan.Metadata.Any(X => X.Key.ToLower() == CommonEnums.spmnth.ToLower()))
                    {
                        cusSubs.CurrentPlan = CommonEnums.spmnth.ToLower();
                        break;
                    }
                    else if (stripeObj.StripePlan.Metadata != null && stripeObj.StripePlan.Metadata.Any(X => X.Key.ToLower() == CommonEnums.spsemi.ToLower()))
                    {
                        cusSubs.CurrentPlan = CommonEnums.spsemi.ToLower();
                        break;
                    }
                }
            }


            foreach (var planObj in plans)
            {
                if (planObj.Metadata != null && planObj.Metadata.Any(X => X.Key.ToLower() == CommonEnums.spmnth.ToLower()))
                {
                    cusSubs.SpMnthPlanID = planObj.Id;
                }

                if (planObj.Metadata != null && planObj.Metadata.Any(X => X.Key.ToLower() == CommonEnums.spsemi.ToLower()))
                {
                    cusSubs.SpSemiPlanID = planObj.Id;
                }
            }

            cusSubs.CustomerId = id;

            return PartialView("_customerSubscription", cusSubs);
        }

        public ActionResult GetAllSubscriptions(string id)
        {
            Stripe.StripeSubscriptionService stripeService = new Stripe.StripeSubscriptionService();

            StripeList<StripeSubscription> subscription = stripeService.List(new StripeSubscriptionListOptions()
            {
                CustomerId = id
            });

            List<CustomerSubscriptionModel> cusSubs = new List<CustomerSubscriptionModel>();

            var subscriptions = subscription.Where(X => X.Object == "subscription");
            foreach (var stripeObj in subscriptions)
            {
                cusSubs.Add(new CustomerSubscriptionModel
                {
                    CreatedDate = stripeObj.Created.HasValue ? stripeObj.Created.Value.ToString("MM/dd/yyyy") : string.Empty,
                    SubscriptionId = stripeObj.Id,
                    Interval = stripeObj.StripePlan.Interval,
                    IntervalCount = stripeObj.StripePlan.IntervalCount,
                    PlanName = stripeObj.StripePlan.Name,
                    CustomerId = id
                });
            }

            return PartialView("_AllSubscriptions", cusSubs);
        }

        [HttpPost]
        public JsonResult CancelSubscription(string subscriptionId)
        {
            try
            {
                Stripe.StripeSubscriptionService stripeService = new Stripe.StripeSubscriptionService();
                stripeService.Cancel(subscriptionId);
                ErrorNotesHelper.AddNote("Customer subscription '" + subscriptionId + "' canceled by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                return Json(new { status = true, message = "<strong>Success!</strong> Subscription cancelled successfully!" });
            }
            catch (Exception ex)
            {
                ErrorNotesHelper.AddNote("Error  '" + ex.Message + "' in cancelling customer subscription for subscription '" + subscriptionId + "' by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), NoteType.Error);
                ErrorNotesHelper.AddError(ex);
                return Json(new { status = false, message = "<strong>Error!</strong> " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ManagePlan(string action, string custId, string currSubId, string planId)
        {
            try
            {
                Stripe.StripeSubscriptionService stripeService = new Stripe.StripeSubscriptionService();
                if (action == "cancel")
                {
                    stripeService.Cancel(currSubId);

                    ErrorNotesHelper.AddNote("Subscription '" + currSubId + "' has been cancelled by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    return Json(new { status = true, message = "<strong>Success!</strong> Subscription cancelled successfully!" });
                }
                else if (action == "subscribe")
                {
                    StripeSubscription newSub = stripeService.Create(custId, planId);
                    ErrorNotesHelper.AddNote("New subscription '" + newSub.StripePlan.Name + "' has been subscribed by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    return Json(new { status = true, message = "<strong>Success!</strong> Plan subscribed successfully!" });
                }
                else if (action == "upgrade")
                {
                    //Cancel current subscription and assign new
                    stripeService.Cancel(currSubId);
                    StripeSubscription newSub = stripeService.Create(custId, planId);

                    ErrorNotesHelper.AddNote("Subscription '" + currSubId + "' has been upgdared to new plan '" + newSub.StripePlan.Name + "' by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    return Json(new { status = true, message = "<strong>Success!</strong> Plan upgraded successfully!" });
                }
                else if (action == "downgrade")
                {
                    //Cancel current subscription and assign new
                    stripeService.Cancel(currSubId);
                    StripeSubscription newSub = stripeService.Create(custId, planId);

                    ErrorNotesHelper.AddNote("Subscription '" + currSubId + "' has been downgraded to new plan '" + newSub.StripePlan.Name + "' by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    return Json(new { status = true, message = "<strong>Success!</strong> Plan downgraded successfully!" });
                }

                return Json(new { status = true, message = "<strong>Success!</strong> Invalid action!" });
            }
            catch (Exception ex)
            {
                ErrorNotesHelper.AddNote("Error '" + ex.Message + "' in plan '" + action + "' for customer by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), NoteType.Error);
                ErrorNotesHelper.AddError(ex);
                return Json(new { status = false, message = "<strong>Error!</strong> " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateCustomer(CustomerModel data)
        {
            try
            {
                StripeCustomerUpdateOptions custUpdate = new StripeCustomerUpdateOptions();
                custUpdate.Description = data.Description;
                custUpdate.Email = data.Email;

                Stripe.StripeCustomerService stripeService = new Stripe.StripeCustomerService();
                stripeService.Update(data.CustomerId, custUpdate);
                ErrorNotesHelper.AddNote("Customer '" + data.Email + "(" + data.CustomerId + ")' updated by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                return Json(new { status = true, message = "<strong>Success!</strong> Customer updated successfully!" });
            }
            catch (Exception ex)
            {
                ErrorNotesHelper.AddNote("Error '" + ex.Message + "' in updating customer for customer '" + data.CustomerId + "' by user '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), NoteType.Error);
                ErrorNotesHelper.AddError(ex);
                return Json(new { status = false, message = "<strong>Error!</strong> " + ex.Message });
            }
        }
    }
}
using SignalProfits.Web.Data;
using SignalProfits.Web.Helpers;
using SignalProfits.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SignalProfits.Web.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (TempData["RegisterModel"] != null)
                return View(TempData["RegisterModel"]);
            else
                return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Login model)
        {
            if (model.Email == null || model.Password == null)
            {
                if (model.Email == null)
                    TempData["ErrorMsg"] = "Please enter email";
                else
                    TempData["ErrorMsg"] = "Please enter password";
            }
            //string password = EncryptionManager.Decrypt("pUqLQ8w6BCEM1eYFgV7DvA==");
            string encryptedPass = EncryptionManager.Encrypt(model.Password);
            using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
            {
                var loginDb = dbEntity.Users.Where(X => X.Email == model.Email && X.Password == encryptedPass && X.IsActive).FirstOrDefault();
                if (loginDb != null)
                {
                    Login loginUsed = new Login();
                    loginUsed.Id = loginDb.Id;
                    loginUsed.Email = loginDb.Email;
                    loginUsed.FirstName = loginDb.FirstName;
                    loginUsed.LastName = loginDb.LastName;
                    loginUsed.Phone = loginDb.Phone;
                    loginUsed.Address = loginDb.Address;
                    loginUsed.IsAdmin = loginDb.IsAdmin;

                    if (!string.IsNullOrWhiteSpace(loginDb.StripeCustomerId))
                    {
                        loginUsed.StripeCustomerId = loginDb.StripeCustomerId;
                    }
                    else
                    {
                        //Update Stripe CustomerID in database
                        CustomerModel customer = StripeHelper.SearchCustomer(model.Email).FirstOrDefault();
                        if (customer != null)
                        {
                            loginDb.StripeCustomerId = customer.CustomerId;

                            dbEntity.SaveChanges();

                            loginUsed.StripeCustomerId = customer.CustomerId;
                        }
                    }

                    Session["User"] = loginUsed;

                    var authTicket = new FormsAuthenticationTicket(1, loginDb.Email.ToString(), DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "", "/");
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                    if (authTicket.IsPersistent)
                    {
                        cookie.Expires = authTicket.Expiration;
                    }

                    Response.Cookies.Add(cookie);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMsg"] = "Invalid credentials";
                    return View();
                }
            }
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(FormCollection frm)
        {
            string email = frm["txtEmail"].ToString();

            //Send email
            using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
            {
                var loginDb = dbEntity.Users.Where(X => X.Email == email && X.IsActive).FirstOrDefault();
                if (loginDb != null)
                {
                    var hostname = HttpContext.Request.Url.Host;

                    Guid token = Guid.NewGuid();

                    UserPasswordToken userToken = new UserPasswordToken();
                    userToken.Email = email;
                    userToken.Token = token;
                    userToken.CreatedDate = DateTime.Now;

                    dbEntity.UserPasswordTokens.Add(userToken);
                    dbEntity.SaveChanges();

                    string activatioLink = hostname + "/Login/ResetPassword?token=" + token.ToString();
                    string body = "Hello " + loginDb.FirstName + " " + loginDb.LastName + ",<br/><br/> Please click the following link to reset your account password<br/>";
                    body += "<a href='" + activatioLink + "'>Click here to rest your password.</a>";
                    body += "<br/><br/> Thanks <br/><br/>";

                    if (EmailHelper.SendEmail(body, "Reset your password", new string[] { email }, null))
                        ViewBag.SuccessMsg = "Email sent successful. Please click on password reset link in your mailbox to activate your account.";
                    else
                    {
                        ViewBag.ErrorMsg = "Error in sending email. Please contact administrator.";
                    }

                    ErrorNotesHelper.AddNote("Reset password requested by user '" + loginDb.FirstName + " " + loginDb.LastName + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                }
                else
                {
                    ViewBag.ErrorMsg = "We didn't found an account with this email!";
                }
            }

            return View();
        }

        public ActionResult ResetPassword(string token)
        {
            RestPassword resetPass = new RestPassword();
            using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
            {
                Guid tokenGuid = new Guid(token);
                var loginDb = dbEntity.UserPasswordTokens.Where(X => X.Token == tokenGuid && !X.IsUsed).FirstOrDefault();
                if (loginDb != null)
                {
                    if (((TimeSpan)DateTime.Now.Subtract(loginDb.CreatedDate)).TotalDays <= AppConfiguration.PasswordResetExpiry)
                    {
                        var userDb = dbEntity.Users.Where(X => X.Email == loginDb.Email && X.IsActive).FirstOrDefault();
                        if (userDb != null)
                        {
                            resetPass.Id = userDb.Id;
                        }

                        resetPass.IsLinkExpired = false;
                        resetPass.ActivationToken = token;

                        return View(resetPass);
                    }
                    else
                    {
                        resetPass.IsLinkExpired = true;
                        ViewBag.ErrorMsg = "This link has expired.Please try again.";
                    }
                }
                else
                {
                    resetPass.IsLinkExpired = true;
                    ViewBag.ErrorMsg = "This link already used/expired.";
                }
            }

            return View(resetPass);
        }

        [HttpPost]
        public ActionResult ResetPassword(RestPassword model)
        {
            if (model.Password == null)
            {
                ViewBag.ErrorMsg = "Please enter password";
            }
            string encryptedPass = EncryptionManager.Encrypt(model.Password);
            using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
            {
                var loginDb = dbEntity.Users.Where(X => X.Id == model.Id).FirstOrDefault();
                if (loginDb != null)
                {
                    loginDb.Password = encryptedPass;
                    dbEntity.SaveChanges();

                    //Update activation link   
                    var userToken = dbEntity.UserPasswordTokens.Where(X => X.Token == new Guid(model.ActivationToken)).FirstOrDefault();
                    if (userToken != null)
                        userToken.IsUsed = true;

                    dbEntity.SaveChanges();

                    ErrorNotesHelper.AddNote("Password has been updated by user '" + loginDb.FirstName + " " + loginDb.LastName + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

                    TempData["SuccessMsg"] = "Password updated successfully. Please login";

                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ViewBag.Error = "Invalid credentials";
                    return View();
                }                
            }
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Index", "login");
        }
    }
}
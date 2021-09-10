using SignalProfits.Web.Data;
using SignalProfits.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalProfits.Web.Model;

namespace SignalProfits.Web.Controllers
{
    [CustomizedAuthorizeAttribute]
    public class CustomerController : Controller
    {
        public ActionResult Index()
        {
            using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
            {
                var usersDb = dbEntity.Users.ToList();
                if (usersDb != null)
                {
                    List<Login> users = new List<Login>();
                    foreach (var user in usersDb)
                    {
                        Login loginUsed = new Login();
                        loginUsed.Id = user.Id;
                        loginUsed.Email = user.Email;
                        loginUsed.FirstName = user.FirstName;
                        loginUsed.LastName = user.LastName;
                        loginUsed.Phone = user.Phone;
                        loginUsed.Address = user.Address;
                        loginUsed.IsAdmin = user.IsAdmin;
                        loginUsed.IsActive = user.IsActive;

                        users.Add(loginUsed);
                    }

                    return View(users);
                }
            }

            return View();
        }

        public ActionResult GetCustomer(int id)
        {
            using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
            {
                Login loginUsed = new Login();
                var usersDb = dbEntity.Users.Where(X => X.Id == id).FirstOrDefault();
                if (usersDb != null)
                {
                    loginUsed.Id = usersDb.Id;
                    loginUsed.Email = usersDb.Email;
                    loginUsed.FirstName = usersDb.FirstName;
                    loginUsed.LastName = usersDb.LastName;
                    loginUsed.Phone = usersDb.Phone;
                    loginUsed.Address = usersDb.Address;
                    loginUsed.IsAdmin = usersDb.IsAdmin;
                    loginUsed.IsActive = usersDb.IsActive;
                }

                return PartialView("_UpdateCustomer", loginUsed);
            }
        }
        public ActionResult ManageSubscription(string email)
        {
            TempData["CustomeEmail"] = email;
            return RedirectToAction("Index", "Home");
        }        

        [HttpPost]
        public ActionResult UpdateCustomer(Login model)
        {
            try
            {
                string hashed_password = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    hashed_password = EncryptionManager.Encrypt(model.Password);
                }

                using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
                {
                    Login login = Session["User"] as Login;

                    var userDb = dbEntity.Users.Where(X => X.Id == model.Id).FirstOrDefault();
                    if (userDb != null)
                    {
                        userDb.FirstName = model.FirstName;
                        userDb.LastName = model.LastName;
                        //userDb.Email = login.Email;
                        if (!string.IsNullOrWhiteSpace(hashed_password))
                        userDb.Password = hashed_password;
                        userDb.Phone = model.Phone;
                        userDb.Address = model.Address;
                        userDb.ModifiedBy = login.Name;
                        userDb.ModifiedDate = DateTime.Now;
                        userDb.IsActive = model.IsActive;
                        userDb.IsAdmin = model.IsAdmin;
                        dbEntity.SaveChanges();
                        ErrorNotesHelper.AddNote("Customer ('" + model.Name + "') update by user " + SessionManager.LoginSession.Name + " on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                        TempData["SuccessMessage"] = " User updated successfully";                        
                    }                  
                }
            }
            catch (Exception ex)
            {
                ErrorNotesHelper.AddError(ex);
                TempData["ErrorMsg"] = " Error in updating user:" + ex.Message;
               
            }

            return RedirectToAction("Index", "Customer");
        }
    }
}
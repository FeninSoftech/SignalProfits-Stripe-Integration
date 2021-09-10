using SignalProfits.Web.Data;
using SignalProfits.Web.Helpers;
using SignalProfits.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalProfits.Web.Controllers
{
    public class RegisterController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Login model)
        {
            ViewBag.IsRegister = true;
            try
            {
                string hashed_password = EncryptionManager.Encrypt(model.Password);

                using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
                {
                    var loginDb = dbEntity.Users.Where(X => X.Email == model.Email).FirstOrDefault();
                    if (loginDb != null)
                    {
                        TempData["RegisterModel"] = model;
                        TempData["ErrorMsg"] = "This email already exists.";
                        return RedirectToAction("Index", "login");
                    }
                    Data.User newUser = new Data.User();
                    newUser.FirstName = model.FirstName;
                    newUser.LastName = model.LastName;
                    newUser.Email = model.Email;
                    newUser.Password = hashed_password;
                    newUser.Phone = model.Phone;
                    newUser.Address = model.Address;
                    newUser.CreatedBy = "System";
                    newUser.CreatedDate = DateTime.Now;
                    newUser.IsActive = true;
                    dbEntity.Users.Add(newUser);
                    dbEntity.SaveChanges();
                    TempData["SuccessMsg"] = "User registered successfully. Please login";                    

                    ErrorNotesHelper.AddNote("New user registered with email: '" + newUser.Email + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

                    return RedirectToAction("Index", "login");
                }
            }
            catch (Exception ex)
            {
                ErrorNotesHelper.AddError(ex);
                TempData["ErrorMsg"] = "Error in creating user:" + ex.Message;
                return View(model);
            }
        }

        public ActionResult UserProfile()
        {
            ViewBag.IsProfile = true;
            Login login = Session["User"] as Login;
            if (login != null)
            {
                return View(login);
            }
            else
            {
                // TempData["ErrorMsg"] = "Session expired. Please login";
                return RedirectToAction("Index", "login");
            }
        }
        [HttpPost]
        public ActionResult UserProfile(Login model)
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

                    var userDb = dbEntity.Users.Where(X => X.Id == login.Id).FirstOrDefault();
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

                        dbEntity.SaveChanges();

                        Login loginUsed = new Login();
                        loginUsed.Id = login.Id;
                        loginUsed.Email = login.Email;
                        loginUsed.FirstName = model.FirstName;
                        loginUsed.LastName = model.LastName;
                        loginUsed.Phone = model.Phone;
                        loginUsed.Address = model.Address;
                        loginUsed.IsAdmin = login.IsAdmin;

                        Session["User"] = loginUsed;

                        ErrorNotesHelper.AddNote("User profile updated for user  '" + SessionManager.LoginSession.Name + "' on " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                        ViewBag.SuccessMessage = "Profile updated successfully";

                        return View(login);
                    }
                    else
                    {
                        ViewBag.Error = "Error in getting user";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorNotesHelper.AddError(ex);
                ViewBag.Error = "Error in creating user:" + ex.Message;
                return View(model);
            }
        }
    }
}

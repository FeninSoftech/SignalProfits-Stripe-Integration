using SignalProfits.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalProfits.Web.Controllers
{
    [CustomizedAuthorizeAttribute]
    public class PlansController : Controller
    {
        // GET: Plans
        public ActionResult Index()
        {
            return View();
        }

        // GET: Plans/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Plans/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Plans/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Plans/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Plans/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Plans/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Plans/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

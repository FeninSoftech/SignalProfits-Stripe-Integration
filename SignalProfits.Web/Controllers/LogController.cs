using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalProfits.Web.Model;
using SignalProfits.Web.Data;
using SignalProfits.Web.Helpers;
using System.IO;
using System.Data.Entity;

namespace SignalProfits.Web.Controllers
{
    [CustomizedAuthorizeAttribute]
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Index()
        {
            using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
            {
                var users = (from user in dbEntity.Users
                             select new SelectListItem
                             {
                                 Text = user.FirstName + " " + user.LastName,
                                 Value = user.Id.ToString()
                             }).OrderBy(X => X.Text).ToList();

                users.Insert(0, new SelectListItem() { Text = "All Users", Value = "-1" });

                ViewData["Users"] = users;
                ViewData["StartDate"] = DateTime.Now.AddDays(-7).ToString("MM/dd/yyyy"); ;
                ViewData["EndDate"] = DateTime.Now.ToString("MM/dd/yyyy");
                return View();
            }
        }

        public ActionResult Search(DateTime startDate, DateTime endDate, int userId)
        {
            using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
            {
                List<Log> notes = (from note in dbEntity.Notes
                                   join use in dbEntity.Users on note.CreatedUserID equals use.Id
                                   where (DbFunctions.TruncateTime(note.CreatedDate) >= startDate && DbFunctions.TruncateTime(note.CreatedDate) <= endDate)
                                   select new Log
                                   {
                                       UserId = use.Id,
                                       FirstName = use.FirstName,
                                       LastName = use.LastName,
                                       Date = note.CreatedDate,
                                       Note = note.Notes
                                   }).ToList();
                if (notes != null && notes.Count > 0)
                {
                    var notesFiltered = notes.Where(X => X.UserId == (userId == -1 ? X.UserId : userId));

                    return PartialView("_Log", notesFiltered.ToList<Log>());
                }
                return PartialView("_Log", new List<Log>());
            }
        }

        public FileResult ExportLog(DateTime startDate, DateTime endDate, int userId)
        {
            using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
            {
                List<Log> notes = (from note in dbEntity.Notes
                                   join use in dbEntity.Users on note.CreatedUserID equals use.Id
                                   where (DbFunctions.TruncateTime(note.CreatedDate) >= startDate && DbFunctions.TruncateTime(note.CreatedDate) <= endDate)
                                   select new Log
                                   {
                                       UserId = use.Id,
                                       FirstName = use.FirstName,
                                       LastName = use.LastName,
                                       Date = note.CreatedDate,
                                       Note = note.Notes
                                   }).ToList();
                if (notes != null && notes.Count > 0)
                {
                    var notesFiltered = notes.Where(X => X.UserId == (userId == -1 ? X.UserId : userId)).OrderByDescending(X => X.Date);

                    var swUsers = new StringWriter();

                    swUsers.WriteLine(String.Format("{0},{1},{2}", "Log Date", "Name", "Log"));

                    foreach (var log in notesFiltered)
                    {
                        swUsers.WriteLine(String.Format("{0},{1},{2}", log.Date, log.Name, log.Note));
                    }

                    return File(new System.Text.UTF8Encoding().GetBytes(swUsers.ToString()), "text/csv", "ApplicationLog_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv");
                }
            }

            return null;
        }
    }
}
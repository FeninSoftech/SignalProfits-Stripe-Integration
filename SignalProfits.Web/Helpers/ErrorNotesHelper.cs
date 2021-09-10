using SignalProfits.Web.Data;
using SignalProfits.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalProfits.Web.Helpers
{
    public class ErrorNotesHelper
    {
        public static void AddNote(string notes, NoteType noteType = NoteType.Success)
        {
            try
            {
                using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
                {
                    Login login = HttpContext.Current.Session["User"] as Login;
                    if (login != null)
                    {

                        Note newNote = new Note();
                        newNote.Notes = notes;
                        newNote.CreatedUserID = login.Id;
                        newNote.CreatedBy = login.Name;
                        newNote.CreatedDate = DateTime.Now;
                        newNote.NoteType = (byte)noteType;

                        dbEntity.Notes.Add(newNote);
                        dbEntity.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
        }

        public static void AddError(string errorMsg, string errorDetail)
        {
            try
            {
                using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
                {
                    string loggedInUser = string.Empty;
                    Login login = HttpContext.Current.Session["User"] as Login;
                    if (login != null)
                    {
                        loggedInUser = login.Name;
                    }

                    Error newError = new Error();
                    newError.ErrorMessage = errorMsg;
                    newError.ErrorDetail = errorDetail;
                    newError.LoggedInUser = loggedInUser;
                    newError.CreatedDate = DateTime.Now;

                    dbEntity.Errors.Add(newError);
                    dbEntity.SaveChanges();
                }
            }
            catch { }           
        }

        public static void AddError(Exception ex)
        {
            try
            {
                if (ex != null)
                {
                    using (SignalProfitsEntities dbEntity = new SignalProfitsEntities())
                    {
                        string loggedInUser = string.Empty;
                        Login login = HttpContext.Current.Session["User"] as Login;
                        if (login != null)
                        {
                            loggedInUser = login.Name;
                        }

                        Error newError = new Error();
                        newError.ErrorMessage = ex.Message;
                        newError.ErrorDetail =  ex.StackTrace + Environment.NewLine + ex.InnerException.ToString();
                        newError.LoggedInUser = loggedInUser;
                        newError.CreatedDate = DateTime.Now;

                        dbEntity.Errors.Add(newError);
                        dbEntity.SaveChanges();
                    }
                }
            }
            catch { }
        }
    }
}
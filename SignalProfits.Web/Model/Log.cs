using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalProfits.Web.Model
{
    public class Log
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public  string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get { return FirstName + " " + LastName; } }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
    }
}
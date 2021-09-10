using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalProfits.Web.Helpers
{
    public class CommonEnums
    {
        public const string spmnth = "sp-mnth";
        public const string spsemi = "sp-semi";
    }

    public enum NoteType
    {
        Success = 1,
        Error = 0
    }
}
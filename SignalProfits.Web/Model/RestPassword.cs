using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SignalProfits.Web.Model
{
    public class RestPassword
    {
        public int Id { get; set; }       
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        [DataType(DataType.Password)]
        public String ConfirmPassword { get; set; }
        public String ActivationToken { get; set; }
        public bool IsLinkExpired { get; set; }
    }
}
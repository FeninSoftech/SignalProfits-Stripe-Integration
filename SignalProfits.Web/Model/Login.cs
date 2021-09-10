using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SignalProfits.Web.Model
{
    public class Login
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Name { get { return FirstName + " " + LastName; } }
        [Required(ErrorMessage = "Email is required.")]
        //[System.Web.Mvc.Remote("ValidateEmail", "Login", HttpMethod = "POST", ErrorMessage = "Email already exists.")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        [DataType(DataType.Password)]
        public String ConfirmPassword { get; set; }
        public String Phone { get; set; }
        public String Address { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public String StripeCustomerId { get; set; }
    }
}
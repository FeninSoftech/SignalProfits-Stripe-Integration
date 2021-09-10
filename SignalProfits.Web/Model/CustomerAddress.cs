using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SignalProfits.Web.Model
{
    public class CustomerAddress
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
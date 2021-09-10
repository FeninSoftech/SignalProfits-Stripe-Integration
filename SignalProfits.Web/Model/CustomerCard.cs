using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SignalProfits.Web.Model
{
    public class CustomerCard
    {        
        public string CustomerId { get; set; }
        public string CardId { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Card Number is required.")]
        public string CardNo { get; set; }
        [Required(ErrorMessage = "Month is required.")]
        public int ExpMonth { get;  set; }
        [Required(ErrorMessage = "Year is required.")]
        public int ExpYear { get; set; }
        [Required(ErrorMessage = "CVV is required.")]
        public string CVV { get;  set; }
        public bool IsDefault { get; set; }
        public List<Card> Cards { get; set; }
    }

    public class Card
    {
        public string CustomerId { get; set; }
        public string CardId { get; set; }
        public string Name { get; set; }
        public string CardNo { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public string CVV { get; set; }
        public bool IsDefault { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class PaymentViewModel
    {
        public PaymentViewModel()
        {
            this.PayTypes = new List<SelectListItem>();
            this.PayMethods = new List<SelectListItem>();
            this.LimitedDays = new List<SelectListItem>();
            this.BankPayments = new List<SelectListItem>();
        }
        public List<SelectListItem> PayTypes { get; set; }
        public List<SelectListItem> PayMethods { get; set; }
        public List<SelectListItem> LimitedDays { get; set; }
        public List<SelectListItem> BankPayments { get; set; }
    }
}
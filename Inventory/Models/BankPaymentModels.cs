using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class BankPaymentModels
    {
        public BankPaymentModels()
        {
            this.LstBankPayment = new List<BankPaymentModels>();
            this.Banks = new List<SelectListItem>();
        }
        public int BankID { get; set; }
        public int BankPaymentID { get; set; }
        public string Name { get; set; }
        public string BankName { get; set; }
        public List<BankPaymentModels> LstBankPayment { get; set; }
        public List<SelectListItem> Banks { get; set; }
    }
}
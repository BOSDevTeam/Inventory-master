using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class MasterSaleVoucherViewModel
    {
        public MasterSaleVoucherViewModel()
        {
            this.MasterSaleModel = new MasterSaleModels();
        }
        public MasterSaleModels MasterSaleModel { get; set; }
        public string UserName { get; set; }
        public string CustomerName { get; set; }
        public string LocationName { get; set; }
        public string CurrencyKeyword { get; set; }
        public string Payment { get; set; }
        public string PayMethod { get; set; }
        public string BankPayment { get; set; }
        public string LimitedDay { get; set; }
        public string Remark { get; set; }
    }
}
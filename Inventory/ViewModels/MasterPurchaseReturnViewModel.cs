using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class MasterPurchaseReturnViewModel
    {
        public MasterPurchaseReturnViewModel()
        {
            this.MasterPurchaseReturnModel = new MasterPurchaseReturnModels();
        }
        public MasterPurchaseReturnModels MasterPurchaseReturnModel { get; set; }
        public string UserName { get; set; }
        public string ReturnDateTime { get; set; }
        public string LocationName { get; set; }
        public string Payment { get; set; }
        public string PayMethod { get; set; }
        public string BankPayment { get; set; }
        
        
    }
}
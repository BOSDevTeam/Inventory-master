using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class MasterPurchaseViewModel
    {
        public MasterPurchaseViewModel()
        {
            this.MasterPurchaseModel = new MasterPurchaseModels();
        }
        public MasterPurchaseModels MasterPurchaseModel { get; set; }
        public string UserName { get; set; }
        public string SupplierName { get; set; }
        public string LocationName { get; set; }
        public string Payment { get; set; }
        public string PayMethod { get; set; }
        public string BankPayment { get; set; }
    }
}
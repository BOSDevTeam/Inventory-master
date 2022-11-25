using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class MasterPurOrderViewModel
    {
        public MasterPurOrderViewModel()
        {
            this.MasterPurchaseOrderModel = new MasterPurchaseOrderModels();
        }
        public MasterPurchaseOrderModels MasterPurchaseOrderModel { get; set; }
        public string UserName { get; set; }
        public string SupplierName { get; set; }
        public string LocationName { get; set; }
    }

}
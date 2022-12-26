using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class PurchaseReturnViewModel
    {
        public PurchaseReturnViewModel()
        {
            
        }
        public class MasterPurchaseReturnViewModel
        {
            public int PurchaseReturnID { get; set; }
            public string UserVoucherNo { get; set; }
            public string ReturnDateTime { get; set; }
            public int Total { get; set; }           
            public string ReturnVoucherNo { get; set; }
        }
    }
}
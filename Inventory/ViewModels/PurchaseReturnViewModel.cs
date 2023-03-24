using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class PurchaseReturnViewModel
    {
        public PurchaseReturnViewModel()
        {
            this.Units = new List<SelectListItem>();
            this.Currencies = new List<SelectListItem>();
        }
        public List<SelectListItem> Units { get; set; }
        public List<SelectListItem> Currencies { get; set; }
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
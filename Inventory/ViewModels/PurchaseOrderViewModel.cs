using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class PurchaseOrderViewModel
    {
        public PurchaseOrderViewModel()
        {
            this.Suppliers = new List<SelectListItem>();
            this.Locations = new List<SelectListItem>();
            this.Units = new List<SelectListItem>();
            this.Currencies = new List<SelectListItem>();
            this.ProductMenus = new ProductMenuViewModel();
        }
        public List<SelectListItem> Suppliers { get; set; }
        public List<SelectListItem> Locations { get; set; }
        public List<SelectListItem> Units { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public ProductMenuViewModel ProductMenus { get; set; }

        public class MasterPurchaseOrderModels
        {
            public int PurchaseOrderID { get; set; }
            public string OrderDateTime { get; set; }
            public string UserVoucherNo { get; set; }
            public string SupplierName { get; set; }
            public int Total { get; set; }
        }
    }
}
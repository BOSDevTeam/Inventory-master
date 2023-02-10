using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class SupplierOpeningViewModel
    {
        public SupplierOpeningViewModel()
        {
            this.Suppliers = new List<SelectListItem>();
            this.Locations = new List<SelectListItem>();
            this.Currencies = new List<SelectListItem>();
        }

        public List<SelectListItem> Suppliers { get; set; }
        public List<SelectListItem> Locations { get; set; }
        public List<SelectListItem> Currencies { get; set; }

        public class MasterSupplierOpeningViewModel
        {
            public int SupplierOpeningID { get; set; }
            public string UserVoucherNo { get; set; }
            public string OpeningDateTime { get; set; }
            public string VoucherID { get; set; }
            public string UserName { get; set; }
            public int LocationID { get; set; }
            public int CurrencyID { get; set; }
        }
    }
}
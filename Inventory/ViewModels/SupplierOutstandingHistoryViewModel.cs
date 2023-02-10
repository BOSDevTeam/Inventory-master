using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class SupplierOutstandingHistoryViewModel
    {
        public SupplierOutstandingHistoryViewModel()
        {
            this.Suppliers = new List<SelectListItem>();
        }
        public List<SelectListItem> Suppliers { get; set; }

        public class ListViewModel
        {
            public int SupplierID { get; set; }
            public string SupplierName { get; set; }
            public int Payment { get; set; }
        }

        public class DetailViewModel
        {
            public int LedgerID { get; set; }
            public string Date { get; set; }
            public string UserVoucherNo { get; set; }
            public string PayDate { get; set; }
            public int Payment { get; set; }
            public short PayType { get; set; }
        }
    }
}
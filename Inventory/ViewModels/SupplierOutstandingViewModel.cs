using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class SupplierOutstandingViewModel
    {
        public SupplierOutstandingViewModel()
        {
            this.Suppliers = new List<SelectListItem>();
            this.Payments = new PaymentViewModel();
        }
        public List<SelectListItem> Suppliers { get; set; }

        public int PayType { get; set; }
        public PaymentViewModel Payments { get; set; }

        public class SupplierOutstandingListViewModel
        {
            public int SupplierID { get; set; }
            public string SupplierName { get; set; }
            public int AccountOpening { get; set; }
            public int OutstandingOpening { get; set; }
            public int Purchase { get; set; }
            public int Balance { get; set; }
            public int OpeningPayment { get; set; }
            public int PurchasePayment { get; set; }
        }

        public class SupplierOutstandingPaymentViewModel
        {
            public string Date { get; set; }
            public string UserVoucherNo { get; set; }
            public int Opening { get; set; }
            public int Purchase { get; set; }
            public DateTime PayDate { get; set; }
            public int Payment { get; set; }
            public bool IsOpening { get; set; }
        }
    }
}
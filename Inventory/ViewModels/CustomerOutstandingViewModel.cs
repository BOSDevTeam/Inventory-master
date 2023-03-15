using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class CustomerOutstandingViewModel
    {
        public CustomerOutstandingViewModel()
        {
            this.Customers = new List<SelectListItem>();
            this.Payments = new PaymentViewModel();
        }
        public List<SelectListItem> Customers { get; set; }

        public int PayType { get; set; }
        public PaymentViewModel Payments { get; set; }

        public class CustomerOutstandingListViewModel
        {
            public int CustomerID { get; set; }
            public string CustomerName { get; set; }
            public int AccountOpening { get; set; }
            public int OutstandingOpening { get; set; }
            public int Sale { get; set; }
            public int Balance { get; set; }
            public int OpeningPayment { get; set; }
            public int SalePayment { get; set; }
        }

        public class CustomerOutstandingPaymentViewModel
        {
            public string Date { get; set; }
            public string UserVoucherNo { get; set; }
            public int Opening { get; set; }
            public int Sale { get; set; }
            public DateTime PayDate { get; set; }
            public int Payment { get; set; }
            public bool IsOpening { get; set; }
        }
    }
}
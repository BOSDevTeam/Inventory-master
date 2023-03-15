using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            this.lstTopSaleProduct = new List<TopSaleProductView>();
            this.lstTodaySaleQuantity = new List<TodaySaleQuantityView>();
            this.lstBankingAmount = new List<BankingAmountView>();
        }
        public List<TopSaleProductView> lstTopSaleProduct { get; set; }
        public List<TodaySaleQuantityView> lstTodaySaleQuantity { get; set; }
        public List<BankingAmountView> lstBankingAmount { get; set; }
        public List<CustomerOutstandingOverDue> lstCustomerOutstandingOverDue { get; set; }

        public class TopSaleProductView
        {
            public int Number { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
        }

        public class TodaySaleQuantityView
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
            public int Amount { get; set; }
            public bool IsSummary { get; set; }
        }

        public class BankingAmountView
        {
            public string BankPaymentName { get; set; }
            public int Amount { get; set; }
        }

        public class CustomerOutstandingOverDue
        {
            public int CustomerID { get; set; }
            public string CustomerName { get; set; }
            public string TownshipName { get; set; }
            public int TotalDueVoucher { get; set; }
        }

        public class CustomerOutstandingPayment
        {
            public int CustomerID { get; set; }
            public string CustomerName { get; set; }
            public string TownshipName { get; set; }
            public double Amount { get; set; }
        }

        public class CustomerOutstandingOverDueDetail
        {
            public string DateTime { get; set; }
            public string UserVoucherNo { get; set; }
            public string LimitedDayName { get; set; }
            public int OverDueDay { get; set; }
            public double Amount { get; set; }
        }
    }
}
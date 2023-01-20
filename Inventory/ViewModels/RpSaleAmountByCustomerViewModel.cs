using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpSaleAmountByCustomerViewModel
    {
        public RpSaleAmountByCustomerViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstRptSaleAmountByCustomer = new List<CustomerViewModel>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalCash { get; set; }
        public int TotalCredit { get; set; }
        public int TotalTax { get; set; }
        public int TotalCharges { get; set; }
        public int TotalVouDis { get; set; }
        public int TotalAdvancedPay { get; set; }
        public int TotalPayPercent { get; set; }
        public int TotalVouFOC { get; set; }
        public int TotalAmount { get; set; }

        public List<CustomerViewModel> lstRptSaleAmountByCustomer { get; set; }
        public class CustomerViewModel
        {
            public int CustomerID { get; set; }
            public string CustomerName { get; set; }
            public List<SaleItemViewModel> lstSaleItem { get; set; }
        }
        public class SaleItemViewModel
        {
            public int SaleID { get; set; }
            public DateTime SaleDate { get; set; }
            public string SaleVoucherNO { get; set; }
            public int PaymentID { get; set; }
            public string PaymentName { get; set; }
            public int SubTotal { get; set; }
            public int TaxAmount { get; set; }
            public int ChargesAmount { get; set; }
            public int VouDiscount { get; set; }
            public int AdvancedPay { get; set; }
            public int PayPercentAmount { get; set; }
            public int VouFOC { get; set; }
            public int GrandTotal { get; set; }


        }
    }
}
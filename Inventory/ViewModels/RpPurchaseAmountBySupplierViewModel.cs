using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpPurchaseAmountBySupplierViewModel
    {
     
        public RpPurchaseAmountBySupplierViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstRptPurchaseAmountBySupplier = new List<SupplierViewModel>();
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
        public List<SupplierViewModel> lstRptPurchaseAmountBySupplier { get; set; }
        public class SupplierViewModel
        {
            public int SupplierID { get; set; }
            public string SupplierName { get; set; }
            public List<PurchaseItemViewModel> lstPurchaseItem { get; set; }
        }
        public class PurchaseItemViewModel
        {
            public int PurchaseID { get; set; }
            public DateTime PurDate { get; set; }
            public string PurVoucherNO { get; set; }
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
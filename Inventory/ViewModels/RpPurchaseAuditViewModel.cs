using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpPurchaseAuditViewModel
    {
        public RpPurchaseAuditViewModel()
        {
            this.lstMasterPurchase = new List<MasterPurchaseView>();
            this.lstTranPurchase = new List<TranPurchaseView>();
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<MasterPurchaseView> lstMasterPurchase { get; set; }
        public List<TranPurchaseView> lstTranPurchase { get; set; }
        public class MasterPurchaseView
        {
            public int PurchaseID { get; set; }
            public DateTime PurDateTime { get; set; }
            public string User { get; set; }
            public int SupplierID { get; set; }
            public string SupplierName { get; set; }       
            public int LocationID { get; set; }
            public string LocationName { get; set; }
            public int CurrencyID { get; set; }
            public string CurrencyName { get; set; }
            public int PaymentID { get; set; }
            public string PaymentKeyword { get; set; }
            public int PayMethodID { get; set; }
            public string PayMethodName { get; set; }
            public string BankPayment { get; set; }
            
            
            public int Subtotal { get; set; }
            public int TaxAmt { get; set; }
            public int ChargesAmt { get; set; }
            public int Total { get; set; }
            public int VouDisPercent { get; set; }
            public int VouDisAmount { get; set; }
            public int VoucherDiscount { get; set; }
            public int AdvancedPay { get; set; }
            public int PaymentPercent { get; set; }
            public int PayPercentAmt { get; set; }
            public int Grandtotal { get; set; }
            public bool IsVouFOC { get; set; }
            public int VoucherFOC { get; set; }
        }
        public class TranPurchaseView
        {
            public int PurchaseID { get; set; }
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public int PurchasePrice { get; set; }
            public int Discount { get; set; }
            public int Amount { get; set; }
            public int UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public int CurrencyID { get; set; }
            public string CurrencyKeyword { get; set; }
            public int DiscountPercent { get; set; }
            public bool IsFOC { get; set; }
            public string Accessories { get; set; }
        }
    }
}
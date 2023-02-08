using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpPurchaseAmountOnlyViewModel
    {
        public RpPurchaseAmountOnlyViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstPayMethod = new List<PayMethod>();
            this.lstBankPayment = new List<BankPayment>();
            this.lstMasterPurchase = new List<MasterPurchaseModels>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<PayMethod> lstPayMethod { get; set; }

        public List<BankPayment> lstBankPayment { get; set; }
        public List<MasterPurchaseModels> lstMasterPurchase { get; set; }

        public class PayMethod
        {
            public int PayMethodID { get; set; }
            public string PayMethodName { get; set; }
        }

        public class BankPayment
        {
            public int BankPaymentID { get; set; }
            public string BankPaymentName { get; set; }

        }

        public class MasterPurchaseModels
        {
            public int PurchaseID { get; set; }
            public string UserVoucherNo { get; set; }
            public string PurchaseDateTime { get; set; }
            public int PaymentID { get; set; }
            public string PaymentName { get; set; }
            public int VoucherDiscount { get; set; }
            public int AdvancedPay { get; set; }
            public int Tax { get; set; }
            public int TaxAmt { get; set; }
            public int Charges { get; set; }
            public int ChargesAmt { get; set; }
            public int Subtotal { get; set; }
            public int Total { get; set; }
            public int Grandtotal { get; set; }
            public int VouDisPercent { get; set; }
            public int VouDisAmount { get; set; }
            public int PayMethodID { get; set; }
            public int BankPaymentID { get; set; }
            public int PaymentPercent { get; set; }
            public int PayPercentAmt { get; set; }
            public int VoucherFOC { get; set; }
            public bool IsVouFOC { get; set; }

        }
    }
}
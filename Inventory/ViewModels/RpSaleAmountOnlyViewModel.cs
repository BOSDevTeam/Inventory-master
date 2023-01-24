using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class RpSaleAmountOnlyViewModel
    {
        public RpSaleAmountOnlyViewModel()
        {
            this.lstBankPayment = new List<BankPayment>();
            this.lstMasterSaleRpt = new List<MasterSaleModels>();
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
        }

        public List<BankPayment> lstBankPayment { get; set; }

        public List<MasterSaleModels> lstMasterSaleRpt { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string PayMethodName { get; set; }
        public string PayMethodName2 { get; set; }

        public class MasterSaleModels
        {
            public int SaleID { get; set; }
            public string UserVoucherNo { get; set; }
            public string SaleDateTime { get; set; }
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

        public class BankPayment
        {
            public int BankPaymentID { get; set; }
            public string BankPaymentName { get; set; }
        }
    }
}
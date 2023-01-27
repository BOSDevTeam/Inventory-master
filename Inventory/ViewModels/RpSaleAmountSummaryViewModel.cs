using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class RpSaleAmountSummaryViewModel
    {
        public RpSaleAmountSummaryViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstBankItem = new List<BankItem>();
            this.lstSaleItem = new List<SaleItem>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string PayMethodName { get; set; }
        public string PayMethodName2 { get; set; }

        public List<BankItem> lstBankItem { get; set; }
        public List<SaleItem> lstSaleItem { get; set; }

        public class BankItem
        {
            public int BankPaymentID { get; set; }
            public string BankPaymentName { get; set; }
        }

        public class SaleItem
        {
            public int SaleID { get; set; }
            public int SlipID { get; set; }
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
    }
}
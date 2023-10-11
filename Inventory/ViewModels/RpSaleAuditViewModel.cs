using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpSaleAuditViewModel
    {
        public RpSaleAuditViewModel()
        {
            this.lstMasterSale = new List<MasterSaleView>();
            this.lstTranSale = new List<TranSaleView>();
            this.lstMultiPay = new List<MultiPayView>();
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<MasterSaleView> lstMasterSale { get; set; }
        public List<TranSaleView> lstTranSale { get; set; }
        public List<MultiPayView> lstMultiPay { get; set; }

        public class MultiPayView
        {          
            public int SaleID { get; set; }
            public int PayMethodID { get; set; }
            public string PayMethodName { get; set; }          
            public string BankPaymentName { get; set; }
            public int PaymentPercent { get; set; }           
            public int Amount { get; set; }
        }

        public class MasterSaleView
        {
            public int SaleID { get; set; }
            public string SaleDateTime { get; set; }
            public string User { get; set; }
            public string Client { get; set; }
            public string CustomerName { get; set; }
            public string Location { get; set; }
            public string PaymentKeyword { get; set; }
            public string PayMethodName { get; set; }
            public string BankPayment { get; set; }
            public int SlipID { get; set; }
            public string LimitedDay { get; set; }
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
            public string CurrencyKeyword { get; set; }
        }

        public class TranSaleView
        {
            public int SaleID { get; set; }
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public int SalePrice { get; set; }
            public int Discount { get; set; }
            public int Amount { get; set; }
            public string UnitKeyword { get; set; }
            public string CurrencyKeyword { get; set; }
            public int DiscountPercent { get; set; }
            public bool IsFOC { get; set; }           
        }
    }
}
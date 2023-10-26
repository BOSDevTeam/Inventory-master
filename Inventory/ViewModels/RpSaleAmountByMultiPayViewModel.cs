using Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpSaleAmountByMultiPayViewModel
    {
        public RpSaleAmountByMultiPayViewModel()
        {
            this.lstMultiPayAmount = new List<MultiPayAmountView>();
            this.lstBankPayment = new List<BankPaymentModels>();
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<MultiPayAmountView> lstMultiPayAmount { get; set; }
        public List<BankPaymentModels> lstBankPayment { get; set; }

        public class MultiPayAmountView
        {
            public int SaleID { get; set; }
            public string UserVoucherNo { get; set; }
            public int SlipID { get; set; }
            public int PayMethodID { get; set; }
            public int BankPaymentID { get; set; }
            public int Grandtotal { get; set; }
            public int MultiPayMethodID { get; set; }
            public int MultiBankPaymentID { get; set; }
            public int MultiGrandtotal { get; set; }          
        }
    }
}
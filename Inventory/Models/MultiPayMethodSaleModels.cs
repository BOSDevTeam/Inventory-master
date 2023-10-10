using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class MultiPayMethodSaleModels
    {
        public int MultiPayMethodID { get; set; }
        public int SaleID { get; set; }
        public int PayMethodID { get; set; }
        public string PayMethodName { get; set; }
        public int BankPaymentID { get; set; }
        public string BankPaymentName { get; set; }
        public int PaymentPercent { get; set; }
        public int PayPercentAmt { get; set; }
        public int Amount { get; set; }
    }
}
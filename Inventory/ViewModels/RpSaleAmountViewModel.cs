using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class RpSaleAmountViewModel
    {
        public RpSaleAmountViewModel()
        {           
            this.lstMasterSaleRpt = new List<MasterSaleView>();
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
        }

        public List<MasterSaleView> lstMasterSaleRpt { get; set; }    
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public class MasterSaleView
        {
            public int SaleID { get; set; }
            public string UserVoucherNo { get; set; }
            public string SaleDateTime { get; set; }
            public int SlipID { get; set; }
            public int CashInHand { get; set; }
            public int Banking { get; set; }
            public int VoucherDiscount { get; set; }
            public int AdvancedPay { get; set; }            
            public int TaxAmt { get; set; }          
            public int ChargesAmt { get; set; }           
            public int Grandtotal { get; set; }           
            public int PayPercentAmt { get; set; }
            public int VoucherFOC { get; set; }
            public bool IsVouFOC { get; set; }
        }
    }
}
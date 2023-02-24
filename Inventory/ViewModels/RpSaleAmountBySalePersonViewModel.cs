using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpSaleAmountBySalePersonViewModel
    {
        public RpSaleAmountBySalePersonViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstSalePerson = new List<SalePerson>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalCash { get; set; }
        public int TotalCredit { get; set; }
        public int TotalTaxAmt { get; set; }
        public int TotalChargesAmt { get; set; }
        public int TotalVouDis { get; set; }
        public int TotalAdvancedPay { get; set; }
        public int TotalPaypercentAmt { get; set; }
        public int TotalVouFOC { get; set; }
        public int TotalGrandTotal { get; set; }
        public List<SalePerson> lstSalePerson { get; set; }

        public class SalePerson
        {
            public int SalePersonID { get; set; }
            public string SalePersonName { get; set; }
            public int SalePersonID2 { get; set; }
            public string SalePersonName2 { get; set; }
            public List<SaleItem> lstSaleItem { get; set; }
        }
        public class SaleItem
        {
            public int SaleID { get; set; }
            public DateTime SaleDateTime { get; set; }
            public string UserVoucherNo { get; set; }
            public string VoucherID { get; set; }
            public int PaymentID { get; set; }
            public  int Subtotal { get; set; }
            public int TaxAmt { get; set; }
            public int ChargesAmt { get; set; }
            public int VouDiscount { get; set; }
            public int AdvancedPay { get; set; }
            public int PayPercentAmt { get; set; }
            public int VouFOC { get; set; }
            public int Grandtotal { get; set; }
        }
    }
}
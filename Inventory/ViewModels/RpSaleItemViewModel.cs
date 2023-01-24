using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class RpSaleItemViewModel
    {
         
        public RpSaleItemViewModel()
        {          
            this.lstMasterSaleRpt = new List<MasterSaleViewModel>();
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
        }
        public List<MasterSaleViewModel> lstMasterSaleRpt { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsAll { get; set; }
        public bool IsCash { get; set; }
        public bool IsCredit { get; set; }
        public class MasterSaleViewModel
        {
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public List<SaleItemViewModel> lstSaleItem { get; set; }
        }
        public class SaleItemViewModel
        {
            public int PaymentID { get; set; }
            public string PaymentName { get; set; }
            public bool IsFOC { get; set; }
            public int DiscountPercent { get; set; }
            public int Discount { get; set; }
            public int Quantity { get; set; }
            public int SalePrice { get; set; }
            public int Amount { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpSaleItemByStaffViewModel
    {
        public RpSaleItemByStaffViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstRptSaleItemByStaff = new List<StaffViewModel>();            
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<StaffViewModel> lstRptSaleItemByStaff { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalAmount { get; set; }
        public int TotalDis { get; set; }
        public class StaffViewModel
        {
            public int StaffID { get; set; }
            public string StaffName { get; set; }
            public List<SaleItemViewModel> lstSaleItem { get; set; }
        }
        public class SaleItemViewModel
        {
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public int Quantity { get; set; }
            public int SalePrice { get; set; }
            public int Discount { get; set; }
            public int Amount { get; set; }
        }
    }
}
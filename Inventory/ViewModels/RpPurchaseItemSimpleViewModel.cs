using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpPurchaseItemSimpleViewModel
    {
        public List<MainMenuViewModel> lstPurchaseRpt { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalDis { get; set; }
        public int TotalAmount { get; set; }
        public class MainMenuViewModel
        {
            public int MainMenuID { get; set; }
            public string MainMenuName { get; set; }
            public List<SubMenuViewModel> lstSubMenu { get; set; }
        }
        public class SubMenuViewModel
        {
            public int SubMenuID { get; set; }
            public string SubMenuName { get; set; }
            public List<PurchaseItemViewModel> lstPurchaseItem { get; set; }
        }
        public class PurchaseItemViewModel
        {
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public int PaymentID { get; set; }
            public string PaymentName { get; set; }           
            public int DiscountPercent { get; set; }
            public int Discount { get; set; }
            public int Quantity { get; set; }
            public int PurchasePrice { get; set; }
            public int Amount { get; set; }
            public string Accessories { get; set; }
            public byte[] Photo { get; set; }
        }
    }
}
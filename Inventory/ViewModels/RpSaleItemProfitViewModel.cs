using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpSaleItemProfitViewModel
    {
        public RpSaleItemProfitViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstSaleItemProfit = new List<RpSaleItemProfitViewModel>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ProductID { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int SalePrice { get; set; }
        public int PurchasePrice { get; set; }
        public int Balance { get; set; }
        public int UnitID { get; set; }
        public string UnitKeyword { get; set; }
        public bool IsFOC { get; set; }
        public int SaleDiscount { get; set; }
        public int PurchaseDiscount { get; set; }
        public List<RpSaleItemProfitViewModel> lstSaleItemProfit { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpPurchaseItemBySupplierViewModel
    {
        public RpPurchaseItemBySupplierViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstTranPurchase = new List<TranPurchaseModels>();
            this.lstSupplier = new List<SupplierModels>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<TranPurchaseModels> lstTranPurchase { get; set; }
        public List<SupplierModels> lstSupplier { get; set; }

        public class SupplierModels
        {
            public int SupplierID { get; set; }
            public string SupplierName { get; set; }
        }

        public class TranPurchaseModels
        {
            public int ID { get; set; }
            public int SaleID { get; set; }
            public int ProductID { get; set; }
            public int SupplierID { get; set; }
            public int Quantity { get; set; }
            public int PurchasePrice { get; set; }
            public int Amount { get; set; }
            public bool IsFOC { get; set; }
            public int? UnitID { get; set; }
            public int? CurrencyID { get; set; }
            public int Discount { get; set; }
            public int DiscountPercent { get; set; }
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
            public string UnitKeyword { get; set; }
            public string CurrencyKeyword { get; set; }
            public string Accessories { get; set; }
        }
    }
}
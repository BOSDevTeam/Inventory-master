using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class TranPurchaseOrderModels
    {
        public int ID { get; set; }
        public int PurchaseOrderID { get; set;} 
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public int? UnitID { get; set; }
        public string UnitKeyword { get; set; }
        public int PurPrice { get; set; }
        public int? CurrencyID { get; set; }
        public string CurrencyKeyword { get; set; }
        public int Amount { get; set; }
        public bool IsFOC { get; set; }
        public int? Number { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class TranPurchaseModels
    {
        public int ID { get; set; }
        public int PurchaseID { get; set; }
        public int ProductID { get; set; }
        public int UnitID { get; set; }
        public int Quantity { get; set; }
        public int SalePrice { get; set; }
        public int TotalAmount { get; set; }
    }
}
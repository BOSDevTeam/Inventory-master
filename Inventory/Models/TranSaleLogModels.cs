using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class TranSaleLogModels
    {     
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int SalePrice { get; set; }
        public int Amount { get; set; }
        public bool IsFOC { get; set; }
        public int? UnitID { get; set; }
        public int? CurrencyID { get; set; }
        public int Discount { get; set; }
        public int DiscountPercent { get; set; }
        public string ActionCode { get; set; }
    }
}
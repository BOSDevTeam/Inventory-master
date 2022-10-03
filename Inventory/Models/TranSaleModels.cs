﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class TranSaleModels
    {
        public int ID { get; set; }
        public int SaleID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int SalePrice { get; set; }
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
        public int? Number { get; set; }
    }
}
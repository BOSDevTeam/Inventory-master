using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            this.lstTopSaleProduct = new List<TopSaleProductView>();
            this.lstTodaySaleQuantity = new List<TodaySaleQuantityView>();
        }
        public List<TopSaleProductView> lstTopSaleProduct { get; set; }
        public List<TodaySaleQuantityView> lstTodaySaleQuantity { get; set; }
        public class TopSaleProductView
        {
            public int Number { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
        }

        public class TodaySaleQuantityView
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
            public int Amount { get; set; }
            public bool IsSummary { get; set; }
        }
    }
}
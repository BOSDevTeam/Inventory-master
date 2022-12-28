using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class StockStatusViewModel
    {
        public int PriceType { get; set; }

        public class ItemViewModel
        {
            public ItemViewModel()
            {
                lstLocationBalance = new List<int>();
            }
            public int ProductID { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public int Balance { get; set; }
            public List<int> lstLocationBalance { get; set; }
            public int Price { get; set; }
            public int Amount { get; set; }
        }
        
        public class HeaderViewModel
        {
            public HeaderViewModel()
            {
                lstLocationName = new List<string>();
            }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public List<string> lstLocationName { get; set; }
            public string Balance { get; set; }
            public string Price { get; set; }
            public string Amount { get; set; }
        }
    }
}
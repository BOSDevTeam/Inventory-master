using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class StockStatusViewModel
    {
        public int LocationID { get; set; }
        public string ShortName { get; set; }
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Status { get; set; }

        public class HeaderViewModel
        {
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public List<string> lstLocationName { get; set; }
        }
    }
}
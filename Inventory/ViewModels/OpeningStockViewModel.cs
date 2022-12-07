using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class OpeningStockViewModel
    {
        public OpeningStockViewModel()
        {
            this.Locations = new List<SelectListItem>();
            this.ProductMenus = new ProductMenuViewModel();
        }

        public List<SelectListItem> Locations { get; set; }
        public ProductMenuViewModel ProductMenus { get; set; }
        public TranOpeningStockModels TranOpeningStock { get; set; }

        public class MasterOpeningStockViewModel
        {
            public int OpeningStockID { get; set; }
            public string UserVoucherNo { get; set; }
            public string OpeningDateTime { get; set; }
            public string VoucherID { get; set; }
            public string LocationName { get; set; }
            public string UserName { get; set; }
            public int LocationID { get; set; }
        }
    }
}
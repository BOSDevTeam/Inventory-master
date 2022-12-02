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
    }
}
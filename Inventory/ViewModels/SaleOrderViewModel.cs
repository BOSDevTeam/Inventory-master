using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;
using System.Web.Mvc;


namespace Inventory.ViewModels
{
    public class SaleOrderViewModel
    {
        public SaleOrderViewModel()
        {
            this.Customers = new List<SelectListItem>();
            this.Locations = new List<SelectListItem>();
            this.Units = new List<SelectListItem>();
            this.Currencies = new List<SelectListItem>();
            this.ProductMenus = new ProductMenuViewModel();
        }
        public List<SelectListItem> Customers { get; set; }
        public List<SelectListItem> Locations { get; set; }
        public List<SelectListItem> Units { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public ProductMenuViewModel ProductMenus { get; set; }

        public class MasterSaleOrderViewModel
        {
            public int SaleOrderID { get; set; }
            public string OrderDateTime { get; set; }
            public string UserVoucherNo { get; set; }
            public string   CustomerName { get; set; }
            public int Total { get; set; }
        }
    }
}
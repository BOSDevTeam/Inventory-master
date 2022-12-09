using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Inventory.ViewModels
{
    public class CustomerConsignViewModel
    {
        public CustomerConsignViewModel()
        {
            this.Customers = new List<SelectListItem>();
            this.ProductMenus = new ProductMenuViewModel();
            this.Locations = new List<SelectListItem>();
            this.SalePersons = new List<SelectListItem>();
            this.Divisions = new List<SelectListItem>();
            this.Units = new List<SelectListItem>();
        }
        public List<SelectListItem> Customers { get; set; }
        public List<SelectListItem> Locations { get; set; }
        public List<SelectListItem> SalePersons { get; set; }
        public List<SelectListItem> Divisions { get; set; }
        public List<SelectListItem> Units { get; set; }
        public ProductMenuViewModel ProductMenus { get; set; }
        public class MasterCustomerConsignViewModel
        {
            public int CustomerConsignID { get; set; }
            public string UserVoucherNo { get; set; }
            public string ConsignDateTime { get; set; }
            public string DueDateTime { get; set; }
            public int CustomerID { get; set; }
            public string CustomerName { get; set; }
            
        }
    }
}
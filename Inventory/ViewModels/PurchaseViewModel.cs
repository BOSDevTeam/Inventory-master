using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class PurchaseViewModel
    {
        public PurchaseViewModel()
        {
            this.Suppliers = new List<SelectListItem>();
            this.Locations = new List<SelectListItem>();
            this.Currencies = new List<SelectListItem>();
            this.Units = new List<SelectListItem>();
            this.ProductMenus = new ProductMenuViewModel();
            this.Payments = new PaymentViewModel();
        }
        public List<SelectListItem> Suppliers { get; set; }
        public List<SelectListItem> Locations { get; set; }
        public ProductMenuViewModel ProductMenus { get; set; }
        public List<SelectListItem> Units { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public PaymentViewModel Payments { get; set; }
    }
}
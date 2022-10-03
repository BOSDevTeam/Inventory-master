using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class SaleViewModel
    {
        public SaleViewModel()
        {
            this.Customers = new List<SelectListItem>();
            this.Locations = new List<SelectListItem>();
            this.Units = new List<SelectListItem>();
            this.Currencies = new List<SelectListItem>();
            this.ProductMenus = new ProductMenuViewModel();
            this.Payments = new PaymentViewModel();
            this.VoucherSettings = new VoucherSettingModels();          
        }
        public List<SelectListItem> Customers { get; set; }
        public List<SelectListItem> Locations { get; set; }
        public List<SelectListItem> Units { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public ProductMenuViewModel ProductMenus { get; set; }
        public PaymentViewModel Payments { get; set; }
        public VoucherSettingModels VoucherSettings { get; set; }

        public class MasterSaleViewModel
        {
            public int SaleID { get; set; }
            public string UserVoucherNo { get; set; }
            public string SaleDateTime { get; set; }
            public int Grandtotal { get; set; }
            public string CustomerName { get; set; }
            public string PaymentKeyword { get; set; }
        }
    }
}
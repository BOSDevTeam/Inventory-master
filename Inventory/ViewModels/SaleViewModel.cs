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
            public string SystemVoucherNo { get; set; }
            public int LocationID { get; set; }
        }

        public class MasterOpenBillViewModel
        {
            public int OpenBillID { get; set; }
            public string UserVoucherNo { get; set; }
            public string OpenDateTime { get; set; }
            public int Total { get; set; }
            public string CustomerName { get; set; }
            public string Note { get; set; }
            public string VoucherID { get; set; }
            public int CustomerID { get; set; }
            public int LocationID { get; set; }
            public int CurrencyID { get; set; }
            public int Subtotal { get; set; }
            public int TaxAmt { get; set; }
            public int ChargesAmt { get; set; }
        }

        public class CLMasterSaleOrderViewModel
        {
            public int SaleOrderID { get; set; }
            public int CustomerID { get; set; }
            public int Total { get; set; }                             
            public int Subtotal { get; set; }
            public int TaxAmt { get; set; }
            public int ChargesAmt { get; set; }
        }
    }
}
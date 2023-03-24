using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;


namespace Inventory.ViewModels
{
    public class MasterSaleReturnViewModel
    {
        public MasterSaleReturnViewModel()
        {
            this.MasterSaleReturnModel = new MasterSaleReturnModels();
            this.Units = new List<SelectListItem>();
            this.Currencies = new List<SelectListItem>();
        }
        public List<SelectListItem> Units { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public MasterSaleReturnModels MasterSaleReturnModel { get; set; }
        public int SaleReturnID { get; set; }
        public string ReturnDateTime { get; set; } 
        public string UserVoucherNo { get; set; }
        public string ReturnVoucherNo { get; set; }
        public int Total { get; set; }
        public int LocationID { get; set; }
        public string ShortName { get; set; }
        public string UserName { get; set; }
        public string Remark { get; set; }
    }
}
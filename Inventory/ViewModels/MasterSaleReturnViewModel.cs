using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;


namespace Inventory.ViewModels
{
    public class MasterSaleReturnViewModel
    {
        public MasterSaleReturnViewModel()
        {
            this.MasterSaleReturnModel = new MasterSaleReturnModels();
        }
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
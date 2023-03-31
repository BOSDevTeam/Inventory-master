using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpStockStatusByModuleViewModel
    {
        public RpStockStatusByModuleViewModel()
        {
            this.lstDetail = new List<DetailView>();
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<DetailView> lstDetail { get; set; }

        public class DetailView
        {
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int OpeningStock { get; set; }
            public int Sale { get; set; }
            public int Purchase { get; set; }
            public int AdjustmentIncrease { get; set; }
            public int AdjustmentDecrease { get; set; }
            public int SaleReturn { get; set; }
            public int PurchaseReturn { get; set; }
            public int Balance { get; set; }
        }
    }
}
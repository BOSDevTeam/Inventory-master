using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpAdjustmentItemViewModel
    {
        public RpAdjustmentItemViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstAdjustment = new List<AdjustmentTypeViewModel>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<AdjustmentTypeViewModel> lstAdjustment { get; set; }
        public class AdjustmentTypeViewModel
        {
            public int AdjustTypeID { get; set; }
            public string AdjustTypeName { get; set; }
            public string Sign { get; set; }
            public List<AdjustmentItemViewModel> lstAdjustmentItem { get; set; }
        }
        public class AdjustmentItemViewModel
        {
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int SalePrice { get; set; }
            public int PurchasePrice { get; set; }
            public int Quantity { get; set; }
            public int UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public int AdjustTypeID { get; set; }
        }
    }
}
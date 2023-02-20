using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpAdjustmentVoucherViewModel
    {
        public RpAdjustmentVoucherViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstMasterAdjustment = new List<MasterAdjustmentView>();
            this.lstTranAdjustment = new List<TranAdjustmentView>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<MasterAdjustmentView> lstMasterAdjustment { get; set; }
        public List<TranAdjustmentView> lstTranAdjustment { get; set; }

        public class MasterAdjustmentView
        {
            public int AdjustmentID { get; set; }
            public DateTime AdjustDate { get; set; }
            public string VoucherNo { get; set; }
            public string VoucherID { get; set; }
            public string UserName { get; set; }
            public string LocationName { get; set; }
            public string Remark { get; set; }
        }
        public class TranAdjustmentView
        {
            public int AdjustmentID { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public string UnitName { get; set; }
            public string AdjustTypeName { get; set; }
        }
    }
}
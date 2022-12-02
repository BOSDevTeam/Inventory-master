using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class MasterAdjustmentModels
    {
        public int AdjustmentID { get; set; }
        public string SystemVoucherNo { get; set; }
        public string UserVoucherNo { get; set; }
        public string VoucherID { get; set; }
        public string AdjustDateTime { get; set; }
        public int UserID { get; set; }
        public int LocationID { get; set; }          
        public string Remark { get; set; }
    }
}
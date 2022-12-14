using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class MasterSaleReturnModels
    {
        public int SaleReturnID { get; set; }
        public string ReturnVoucherNo { get; set; }
        public string ReturnDateTime { get; set; }
        public string UserVoucherNo { get; set; }
        public string VoucherID { get; set; }
        public int UserID { get; set; }
        public string Remark { get; set; }
        public int Total { get; set; }
    }
}
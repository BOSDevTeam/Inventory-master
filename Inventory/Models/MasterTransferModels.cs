using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class MasterTransferModels
    {
        public int TransferID { get; set; }
        public string TransferDateTime { get; set; }
        public string SystemVoucherNo { get; set; }
        public string UserVoucherNo { get; set; }
        public string VoucherID { get; set; }
        public int UserID { get; set; }
        public int FromLocationID { get; set; }
        public int ToLocationID { get; set; }
        public int TotalQuantity { get; set; }
        public string Remark { get; set; }



    }
}
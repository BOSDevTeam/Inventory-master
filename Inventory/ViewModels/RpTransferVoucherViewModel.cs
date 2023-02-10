using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpTransferVoucherViewModel
    {
        public RpTransferVoucherViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstMasterTransfer = new List<MasterTransferModels>();
            this.lstTranTransfer = new List<TranTransferModels>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<MasterTransferModels> lstMasterTransfer { get; set; }

        public List<TranTransferModels> lstTranTransfer { get; set; }

        public class MasterTransferModels
        {
            public int TransferID { get; set; }
            public string TransferDateTime { get; set; }
            public string SystemVoucherNo { get; set; }
            public string UserVoucherNo { get; set; }
            public string VoucherID { get; set; }
            public int UserID { get; set; }
            public string UserName { get; set; }
            public int FromLocationID { get; set; }
            public string FromLocationName { get; set; }
            public int ToLocationID { get; set; }
            public string ToLocationName { get; set; }
            public string Remark { get; set; }
        }

        public class TranTransferModels
        {
            public int ID { get; set; }
            public int TransferID { get; set; }
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
            public int Quantity { get; set; }
            public int? UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public int Amount { get; set; }

        }
    }
}
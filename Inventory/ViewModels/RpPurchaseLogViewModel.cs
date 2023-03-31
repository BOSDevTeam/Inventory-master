using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpPurchaseLogViewModel
    {
        public RpPurchaseLogViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstMasterPurchaseLog = new List<MasterPurchaseLogViewModel>();
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int LogType { get; set; }
        public List<MasterPurchaseLogViewModel> lstMasterPurchaseLog { get; set; }
        public class MasterPurchaseLogViewModel
        {
            public int PurchaseLogID { get; set; }
            public DateTime PurchaseDateTime { get; set; }
            public string UserVoucherNo { get; set; }
            public string EntryUserName { get; set; }
            public string UpdateUserName { get; set; }
            public List<PurchaseLogViewModel> lstPurchaseLog { get; set; }
        }
        public class PurchaseLogViewModel
        {
            public int TranPurchaseLogID { get; set; }
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public int OrginalQuantity { get; set; }
            public int UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public int Amount { get; set; }
            public bool IsFOC { get; set; }

        }
    }
}
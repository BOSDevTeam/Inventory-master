using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpSaleLogViewModel
    {
        public RpSaleLogViewModel()
        {
            this.lstMasterSaleLog = new List<MasterSaleLogViewModel>();
            this.ToDate = new DateTime();
            this.FromDate = new DateTime();
        }
        public List<MasterSaleLogViewModel> lstMasterSaleLog { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int LogType { get; set; }
        public class MasterSaleLogViewModel
        {
            public int LogID { get; set; }
            public DateTime SaleDate { get; set; }
            public string UserVoucherNo { get; set; }
            public string EntryUserName { get; set; }
            public string UpdatedUserName { get; set; }
            public List<SaleLogViewModel> lstSaleLog { get; set; }
        }
        public class SaleLogViewModel
        {
            public int TranSaleID { get; set; }
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
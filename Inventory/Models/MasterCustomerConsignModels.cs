using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class MasterCustomerConsignModels
    {
        public int CustomerConsignID { get; set; }
        public string SystemVoucherNo { get; set; }
        public string UserVoucherNo { get; set; }
        public string VoucherID { get; set; }
        public string ConsignDateTime { get; set; }
        public string DueDateTime { get; set; }
        public int UserID { get; set; }
        public int LocationID { get; set; }
        public int CustomerID { get; set; }
        public int DivisionID { get; set; }
        public int SalePersonID { get; set; }
        public string Remark { get; set; }
    }
}
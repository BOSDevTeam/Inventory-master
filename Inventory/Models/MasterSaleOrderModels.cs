using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class MasterSaleOrderModels
    {
        public int SaleOrderID { get; set; }
        public string OrderDateTime { get; set; }
        public string SystemVoucherNo { get; set; }
        public string UserVoucherNo { get; set; }
        public string VoucherID { get; set; }
        public int UserID { get; set; }
        public int CustomerID { get; set; }
        public int LocationID { get; set; }
        public int Subtotal { get; set; }
        public int Tax { get; set; }
        public int TaxAmt { get; set; }
        public int Charges { get; set; }
        public int ChargesAmt { get; set; }
        public int Total { get; set; }

    }
}
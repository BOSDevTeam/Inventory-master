using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class MasterPurchaseModels
    {
        public MasterPurchaseModels()
        {
            this.Suppliers = new List<SelectListItem>();
        }
        public int PurchaseID { get; set; }
        public DateTime Date { get; set; }
        public int UserID { get; set; }
        public int SupplierID { get; set; }
        public int TotalAmount { get; set; }
        public int DiscountAmount { get; set; }
        public int TaxAmount { get; set; }
        public int ChargesAmount { get; set; }
        public int PaidAmount { get; set; }
        public int Balance { get; set; }
        public string Remark { get; set; }
  
        public List<SelectListItem> Suppliers { get; set; }
    }
}
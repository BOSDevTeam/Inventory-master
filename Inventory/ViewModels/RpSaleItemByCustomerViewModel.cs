using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class RpSaleItemByCustomerViewModel
    {
        public RpSaleItemByCustomerViewModel()
        {
            this.FromDate = new DateTime();
            this.ToDate = new DateTime();
            this.lstRptSaleItemByCustomer = new List<CustomerViewModel>();            
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<CustomerViewModel> lstRptSaleItemByCustomer { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalAmount { get; set; }
        public int TotalDis { get; set; }
        public class CustomerViewModel
        {
            public int CustomerID { get; set; }
            public string CustomerName { get; set; }
            public List<SaleItemViewModel> lstSaleItem { get; set; }
        }
        public class SaleItemViewModel
        {
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public int Quantity { get; set; }
            public int SalePrice { get; set; }
            public int Discount { get; set; }
            public int Amount { get; set; }
        }
    }
}
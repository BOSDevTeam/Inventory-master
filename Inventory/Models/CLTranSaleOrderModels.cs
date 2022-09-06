using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class CLTranSaleOrderModels
    {
        public CLTranSaleOrderModels()
        {
            this.TsoList = new List<CLTranSaleOrderModels>();
        }
        public List<CLTranSaleOrderModels> TsoList { get; set; }
        public int ID { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDateTime { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int SalePrice { get; set; }
        public int Amount { get; set; }
        public int Subtotal { get; set; }
        public int Tax { get; set; }
        public int Charges { get; set; }
        public int TaxAmt { get; set; }
        public int ChargesAmt { get; set; }
        public string DefaultCurrency { get; set; }
        public string Remark { get; set; }
        public int Total { get; set; }
    }
}
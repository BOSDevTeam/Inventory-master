using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class CLMasterSaleOrderModels
    {
        public CLMasterSaleOrderModels()
        {
            this.Lstmso = new List<CLMasterSaleOrderModels>();
        }
        public List<CLMasterSaleOrderModels> Lstmso { get; set; }
        public int SaleOrderID { get; set; }
        public string OrderDateTime { get; set; }
        public string OrderNumber { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public bool? IsClientSalePerson { get; set; }
        public int Subtotal { get; set; }
        public int Tax { get; set; }
        public int TaxAmt { get; set; }
        public int Charges { get; set; }
        public int ChargesAmt { get; set; }
        public int Total { get; set; }
        public string DefaultCurrency { get; set; }
        public string Remark { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int Counts { get; set; }

    }
}
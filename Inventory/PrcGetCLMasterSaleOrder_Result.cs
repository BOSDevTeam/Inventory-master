//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inventory
{
    using System;
    
    public partial class PrcGetCLMasterSaleOrder_Result
    {
        public string OrderNumber { get; set; }
        public int SaleOrderID { get; set; }
        public string Date { get; set; }
        public string Remark { get; set; }
        public Nullable<int> Subtotal { get; set; }
        public Nullable<int> Tax { get; set; }
        public Nullable<int> TaxAmt { get; set; }
        public Nullable<int> Charges { get; set; }
        public Nullable<int> ChargesAmt { get; set; }
        public Nullable<int> Total { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Currency { get; set; }
    }
}

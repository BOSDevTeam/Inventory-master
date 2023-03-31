using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class TranSaleReturnModels
    {
        public int ID { get; set; }
        public int SaleReturnID { get; set; }
        public int SaleID { get; set; }
        public string ReturnVoucherNo { get; set; }
        public int ProductID { get; set; }
        public int PaymentID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int SysProductID { get; set; }
        public int InputQuantity { get; set; }
        public int Quantity { get; set; }
        public int? UnitID { get; set; }
        public string UnitKeyword { get; set; }
        public int SalePrice { get; set; }
        public int? CurrencyID { get; set; }
        public string CurrencyKeyword { get; set; }
        public int DiscountPercent { get; set; }
        public int Discount { get; set; }
        public int Amount { get; set; }
        public bool IsFOC { get; set; }
        public int LocationID { get; set; }


    }
}
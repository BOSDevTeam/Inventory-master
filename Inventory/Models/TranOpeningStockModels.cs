using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class TranOpeningStockModels
    {
        public int ID { get; set; }
        public int OpeningStockID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }              
    }
}
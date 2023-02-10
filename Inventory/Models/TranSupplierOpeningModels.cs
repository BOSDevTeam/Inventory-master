using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class TranSupplierOpeningModels
    {
        public int ID { get; set; }
        public int SupplierID { get; set; }
        public int Balance { get; set; }
        public string SupplierName { get; set; }
    }
}
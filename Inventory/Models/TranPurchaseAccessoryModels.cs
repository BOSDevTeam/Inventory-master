using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class TranPurchaseAccessoryModels
    {
        public int ID { get; set; }
        public int TranPurchaseID { get; set; }
        public int? Gold { get; set; }
        public int? Pearl { get; set; }
        public int? Diamond { get; set; }
        public int? Stone { get; set; }
        public int? Palatinum { get; set; }       
        public byte[] Photo { get; set; }
        public string Base64Photo { get; set; }
    }
}
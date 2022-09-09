using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class VoucherFormatModels
    {
        public VoucherFormatModels()
        {
            this.LstVoucherFormat = new List<VoucherFormatModels>();         
        }      
        public List<VoucherFormatModels> LstVoucherFormat { get; set; }
        public int ID { get; set; }      
        public string PreFormat { get; set; }
        public string MidFormat { get; set; }
        public int PostFormat { get; set; }
    }
}
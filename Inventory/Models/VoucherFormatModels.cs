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
            this.Branches = new List<SelectListItem>();
        }
        public List<SelectListItem> Branches { get; set; }
        public List<VoucherFormatModels> LstVoucherFormat { get; set; }
        public int ID { get; set; }
        public int? BranchID { get; set; }
        public string BranchName { get; set; }
        public string PreFormat { get; set; }
        public string MidFormat { get; set; }
        public int PostFormat { get; set; }
    }
}
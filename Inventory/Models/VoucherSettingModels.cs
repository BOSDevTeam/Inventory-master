using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class VoucherSettingModels
    {
        public VoucherSettingModels()
        {
            this.LstVoucherSetting = new List<VoucherSettingModels>();
            this.Branches = new List<SelectListItem>();
            this.Locations = new List<SelectListItem>();
        }
        public List<SelectListItem> Branches { get; set; }
        public List<SelectListItem> Locations { get; set; }
        public List<VoucherSettingModels> LstVoucherSetting { get; set; }
        public int ID { get; set; }
        public int? BranchID { get; set; }
        public string BranchName { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string HeaderName { get; set; }
        public string HeaderDesp { get; set; }
        public string HeaderPhone { get; set; }
        public string HeaderAddress { get; set; }
        public string OtherHeader1 { get; set; }
        public string OtherHeader2 { get; set; }
        public string FooterMessage1 { get; set; }
        public string FooterMessage2 { get; set; }
        public string FooterMessage3 { get; set; }
        public byte[] VoucherLogo { get; set; }
        public string Base64Photo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class TechnicianSettingModels
    {
        public TechnicianSettingModels()
        {
            this.ShopTypes = new List<SelectListItem>();
        }
        public bool IsMultiCurrency { get; set; }
        public bool IsMultiUnit { get; set; }
        public bool IsBankPayment { get; set; }
        public bool IsClientPhoneVerify { get; set; }
        public List<SelectListItem> ShopTypes { get; set; }
        public int ShopTypeID { get; set; }
        public bool IsLimitUser { get; set; }
        public short LimitedUserCount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class AdjustmentViewModel
    {
        public AdjustmentViewModel()
        {
            this.Locations = new List<SelectListItem>();
            this.ProductMenus = new ProductMenuViewModel();
            this.Units = new List<SelectListItem>();
            this.AdjustmentsType = new List<SelectListItem>();
        }
        public List<SelectListItem> Locations { get; set; }
        public ProductMenuViewModel ProductMenus { get; set; }
        public List<SelectListItem> Units { get; set; }
        public List<SelectListItem> AdjustmentsType { get; set; }
        public class MasterAdjustmentViewModel
        {
            public int AdjustmentID { get; set; }
            public string UserVoucherNo { get; set; }
            public string AdjustDateTime { get; set; }
            public string Remark { get; set; }
        }
    }
}
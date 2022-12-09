using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class TransferViewModel
    {
        public TransferViewModel()
        {
            this.Locations = new List<SelectListItem>();
            this.Units = new List<SelectListItem>();
            this.ProductMenus = new ProductMenuViewModel();

        }
        public List<SelectListItem> Locations { get; set; }
        public List<SelectListItem> Units { get; set; }
        public ProductMenuViewModel ProductMenus { get; set; }
        public TranTransferModels TranTransferModel { get; set; }

        public class MasterTransferModels
        {
            public int TransferID { get; set; }
            public string TransferDateTime { get; set; }
            public string UserVoucherNo { get; set; }
            public int FromLocationID { get; set; }
            public string FromLocationName { get; set; }
            public int ToLocationID { get; set; }
            public string ToLocationName { get; set; }
            public int TotalQuantity { get; set; }


        }
    }
}
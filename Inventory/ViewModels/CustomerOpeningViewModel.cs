using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.ViewModels
{
    public class CustomerOpeningViewModel
    {
        public CustomerOpeningViewModel(){
            this.Customers = new List<SelectListItem>();
        }

        public List<SelectListItem> Customers { get; set; }

        public class MasterCustomerOpeningViewModel
        {
            public int CustomerOpeningID { get; set; }
            public string UserVoucherNo { get; set; }
            public string OpeningDateTime { get; set; }
            public string VoucherID { get; set; }          
            public string UserName { get; set; }          
        }
    }
}
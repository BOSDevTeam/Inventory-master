using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class MasterCustomerConsignViewModel
    {
        public MasterCustomerConsignViewModel()
        {
            this.MasterCustomerConsignModel = new MasterCustomerConsignModels();
        }
        public MasterCustomerConsignModels MasterCustomerConsignModel { get; set; }
        public string UserName { get; set; }
        public string LocationName { get; set; }
        public string CustomerName { get; set; }
        public string DivisionName { get; set; }
        public string SalePersonName { get; set; }

    }
}
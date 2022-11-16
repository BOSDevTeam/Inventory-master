using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class MasterSaleOrderViewModel
    {
        public MasterSaleOrderViewModel()
        {
            this.MasterSaleOrderModel = new MasterSaleOrderModels();
        }
        public MasterSaleOrderModels MasterSaleOrderModel { get; set; }
        public string UserName { get; set; }
        public string CustomerName { get; set; }
        public string LocationName { get; set; }
    }
}
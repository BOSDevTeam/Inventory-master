using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class MasterAdjustmentViewModel
    {
        public MasterAdjustmentViewModel()
        {
            this.MasterAdjustmentModel = new MasterAdjustmentModels();
        }
        public MasterAdjustmentModels MasterAdjustmentModel { get; set; }
        public string UserName { get; set; }
        public string LocationName { get; set; }
    }
}
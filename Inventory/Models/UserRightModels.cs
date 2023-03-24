using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class UserRightModels
    {
        public UserRightModels()
        {
            this.Users = new List<SelectListItem>();
            //this.LstSetupModule = new List<SetupModuleModels>();
        }
        public string Layout { get; set; }
        public short IsTechnician { get; set; }
        public List<SelectListItem> Users { get; set; }
        //public List<SetupModuleModels> LstSetupModule { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class TownshipModels
    {
        public class TownshipModel
        {
            public TownshipModel()
            {
                this.LstTownship = new List<TownshipModels.TownshipModel>();
                this.Division = new List<SelectListItem>();
            }
            public List<TownshipModels.TownshipModel> LstTownship { get; set; }
            public List<SelectListItem> Division { get; set; }
            public int TownshipID { get; set; }
            public string Code { get; set; }
            public string TownshipName { get; set; }
            public int DivisionID { get; set; }
            public string DivisionName { get; set; }
            public bool? IsDefault { get; set; }
        }
    }
}
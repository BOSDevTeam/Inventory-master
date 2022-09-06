using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class UnitModels
    {
        public UnitModels()
        {
            this.LstUnit = new List<UnitModels>();
            this.UnitLevels = new List<SelectListItem>();
        }
        public List<UnitModels> LstUnit { get; set; }
        public List<SelectListItem> UnitLevels { get; set; }
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public string Keyword { get; set; }
        public int ULID { get; set; }
        public string ULName { get; set; }
        public int ULOrder { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class StaffModels
    {      
        public StaffModels()
        {
            this.Townships = new List<SelectListItem>();
            this.Divisions = new List<SelectListItem>();
        }
        public int StaffID { get; set; }     
        public string Code { get; set; }      
        public string StaffName { get; set; }
        public int TownshipID { get; set; }
        public int DivisionID { get; set; }
        public string TownshipName { get; set; }
        public string DivisionName { get; set; }
        public string Address { get; set; }      
        public string Phone { get; set; }
        public List<SelectListItem> Townships { get; set; }
        public List<SelectListItem> Divisions { get; set; }
    }
}
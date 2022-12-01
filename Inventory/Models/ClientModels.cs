using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Inventory.Models
{
    public class ClientModels
    {
        public ClientModels()
        {
            this.LstClient = new List<ClientModels>();
            this.Townships = new List<SelectListItem>();
            this.Divisions = new List<SelectListItem>();
        }

        public List<ClientModels> LstClient { get; set; }
        public List<SelectListItem> Townships { get; set; }
        public List<SelectListItem> Divisions { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientPassword { get; set; }
        public string ShopName { get; set; }
        public string Phone { get; set; }
        public int DivisionID { get; set; }
        public int VDivisionID { get; set; }
        public string DivisionName { get; set; }
        public int TownshipID { get; set; }
        public int VTownshipID { get; set; }
        public string TownshipName { get; set; }
        public string Address { get; set; }
        public bool? IsSalePerson { get; set; }

    }
}
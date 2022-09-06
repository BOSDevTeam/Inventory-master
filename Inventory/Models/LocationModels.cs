using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Inventory.Models
{
    public class LocationModels
    {
        public class LocationModel
        {
            public LocationModel()
            {
                this.Branches = new List<SelectListItem>();
                this.LstLocation = new List<LocationModels.LocationModel>();
            }
            public List<LocationModels.LocationModel> LstLocation { get; set; }
            public List<SelectListItem> Branches { get; set; }
            public int LocationID { get; set; }
            [DisplayName("Name")]
            [Required(ErrorMessage = "Please Enter Location Name")]
            public string LocationName { get; set; }
            [DisplayName("Short Name")]
            [Required(ErrorMessage = "Please Enter Location Short Name")]
            public string ShortName { get; set; }
            public string Description { get; set; }
            [Required(ErrorMessage = "Please Enter Location Code")]
            public string Code { get; set; }
            [RegularExpression(@"^(\d{11})$", ErrorMessage = "Invalid Phone Number")]
            public string Phone { get; set; }
            public string Address { get; set; }
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; }
            public int? BranchID { get; set; }
            [DisplayName("Branch")]
            public string BranchName { get; set; }
        }
    }
}
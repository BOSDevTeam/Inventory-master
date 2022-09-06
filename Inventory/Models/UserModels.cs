using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Inventory.Models
{
    public class UserModels
    {
        public class UserModel
        {
            public UserModel()
            {
                this.Branches = new List<SelectListItem>();
                this.Users = new List<SelectListItem>();
                this.Locations = new List<SelectListItem>();
                this.LstUser = new List<UserModel>();
            }
            public List<SelectListItem> Branches { get; set; }
            public List<SelectListItem> Users { get; set; }
            public List<SelectListItem> Locations { get; set; }
            public List<UserModels.UserModel> LstUser { get; set; }

            public int UserID { get; set; }
            public string UserName { get; set; }
            public int? BranchID { get; set; }
            public string BranchName { get; set; }
            public int AdminID { get; set; }
            public int? LocationID { get; set; }
            public string LocationName { get; set; }
            public bool IsDefaultLocation { get; set; }
            public bool ClickedLogin { get; set; }                  
            [DataType(DataType.Password)]
            public string UserPassword { get; set; }
            [Required(ErrorMessage = "Please enter Admin.")]
            public string AdminName { get; set; }
            [Required(ErrorMessage = "Please enter Password.")]
            [DataType(DataType.Password)]
            public string AdminPassword { get; set; }
        }
    }
}
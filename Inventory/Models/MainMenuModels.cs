using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Inventory.Models
{
    public class MainMenuModels
    {
        public class MainMenuModel
        {
            public MainMenuModel()
            {
                this.LstMainMenu = new List<MainMenuModels.MainMenuModel>();
            }
            public List<MainMenuModels.MainMenuModel> LstMainMenu { get; set; }
            public int MainMenuID { get; set; }

            [Required(ErrorMessage = "Please Enter Code")]
            public string Code { get; set; }

            [Required(ErrorMessage = "Please Enter Sort Code")]
            public int SortCode { get; set; }

            [DisplayName("Name")]
            [Required(ErrorMessage = "Please Enter Main Menu")]
            public string MainMenuName { get; set; }

            public byte[] Photo { get; set; }
            public string Base64Photo { get; set; }
        }
    }
}
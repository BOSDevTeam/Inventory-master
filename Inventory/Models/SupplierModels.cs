using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class SupplierModels
    {
        public class SupplierModel
        {
            public SupplierModel()
            {               
                this.Townships = new List<SelectListItem>();
                this.LstSupplier = new List<SupplierModels.SupplierModel>();
                this.Divisions = new List<SelectListItem>();
            }
            public List<SupplierModels.SupplierModel> LstSupplier { get; set; }           
            public List<SelectListItem> Divisions { get; set; }
            public List<SelectListItem> Townships { get; set; }
            public int SupplierID { get; set; }
            [DisplayName("Name")]
            [Required(ErrorMessage = "Please Enter Supplier Name")]
            public string SupplierName { get; set; }
            [Required(ErrorMessage = "Please Enter Code")]
            public string Code { get; set; }
            public string Contact { get; set; }
            public string Address { get; set; }
            [RegularExpression(@"^(\d{11})$", ErrorMessage = "Invalid Phone Number")]
            public string Phone { get; set; }
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; }
            public bool IsCredit { get; set; }
            public string Credit { get; set; }
            public bool? IsDefault { get; set; }           
            public int TownshipID { get; set; }
            [DisplayName("Township")]
            public string TownshipName { get; set; }
            public int DivisionID { get; set; }
            public string DivisionName { get; set; }
        }
    }
}
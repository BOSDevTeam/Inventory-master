using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Inventory.Models
{
    public class CustomerModels
    {
        public class CustomerModel
        {
            public CustomerModel()
            {
                this.Branches = new List<SelectListItem>();
                this.Townships = new List<SelectListItem>();
                this.LstCustomer = new List<CustomerModels.CustomerModel>();
                this.Divisions = new List<SelectListItem>();
            }
            public List<CustomerModels.CustomerModel> LstCustomer { get; set; }
            public List<SelectListItem> Branches { get; set; }
            public List<SelectListItem> Townships { get; set; }
            public List<SelectListItem> Divisions { get; set; }
            public int CustomerID { get; set; }
            [Required(ErrorMessage = "Please Enter Code")]
            public string Code { get; set; }
            [DisplayName("Name")]
            [Required(ErrorMessage = "Please Enter Customer Name")]
            public string CustomerName { get; set; }
            public int TownshipID { get; set; }
            [DisplayName("Township")]
            public string TownshipName { get; set; }
            public string Contact { get; set; }
            public string Address { get; set; }
            [RegularExpression(@"^(\d{11})$", ErrorMessage = "Invalid Phone Number")]
            public string Phone { get; set; }
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; }
            public int BranchID { get; set; }
            [DisplayName("Branch")]
            public string BranchName { get; set; }
            public bool IsCredit { get; set; }
            public string Credit { get; set; }
            public bool? IsDefault { get; set; }
            public int DivisionID { get; set; }
            public string DivisionName { get; set; }
        }
    }
}
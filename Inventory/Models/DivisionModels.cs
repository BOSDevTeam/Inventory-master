using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Inventory.Models
{
    public class DivisionModels
    {
            public DivisionModels()
            {
                this.LstDivision = new List<DivisionModels>();
            }
            public List<DivisionModels> LstDivision { get; set; }
            public int DivisionID { get; set; }

            [Required(ErrorMessage = "Please Enter Code")]
            public string Code { get; set; }

            [DisplayName("DivisionName")]
            [Required(ErrorMessage = "Please Enter DivisionName")]
            public string DivisionName { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class BranchModels
    {
        public class BranchModel
        {
            public BranchModel()
            {
                this.LstBranch = new List<BranchModels.BranchModel>();
            }

            public List<BranchModels.BranchModel> LstBranch { get; set; }

            public int BranchID { get; set; }

            public string BranchName { get; set; }

            public string ShortName { get; set; }

            public string Description { get; set; }

            public string Code { get; set; }

            public string Phone { get; set; }

            public string Address { get; set; }

            public string Email { get; set; }

            public string Tax { get; set; }

            public string ServiceCharges { get; set; }
        }
    }
}
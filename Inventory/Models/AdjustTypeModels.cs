using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class AdjustTypeModels
    {
        public class AdjustTypeModel
        {
            public AdjustTypeModel()
            {
                this.LstAdjustType = new List<AdjustTypeModels.AdjustTypeModel>();
            }
            public List<AdjustTypeModels.AdjustTypeModel> LstAdjustType { get; set; }
            public int AdjustTypeID { get; set; }
            public string ShortName { get; set; }
            public string AdjustTypeName { get; set; }
            public bool IsIncrease { get; set; }
            public string IncreaseDecrease { get; set; }
            public string Sign { get; set; }
        }
    }
}
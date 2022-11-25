using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class AdjustTypeModels
    {
        public AdjustTypeModels()
        {
            this.LstAdjustType = new List<AdjustTypeModels>();
        }
        public List<AdjustTypeModels> LstAdjustType { get; set; }
        public int AdjustTypeID { get; set; }
        public string ShortName { get; set; }
        public string AdjustTypeName { get; set; }
        public bool IsIncrease { get; set; }
        public string IncreaseDecrease { get; set; }
        public string Sign { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class SetupModuleModels
    {
        public int SetupModuleID { get; set; }
        public string SetupModuleName { get; set; }
        public bool IsAllow { get; set; }
    }
}
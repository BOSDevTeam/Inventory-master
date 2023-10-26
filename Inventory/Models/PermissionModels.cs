using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class PermissionModels
    {
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public bool IsAllow { get; set; }
    }
}
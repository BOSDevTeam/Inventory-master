using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class RpAdjustmentItemController : MyController
    { 
        public ActionResult AdjustmentItemReportFilter()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class RpPurchaseAmountOnlyController : MyController
    {
        public ActionResult PurchaseAmountOnlyReportFilter()
        {
            return View();
        }
    }
}
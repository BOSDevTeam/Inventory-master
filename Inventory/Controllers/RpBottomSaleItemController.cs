using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class RpBottomSaleItemController : MyController
    {
        public ActionResult BottomSaleItemReport()
        {
            return View();
        }

        public ActionResult BottomSaleItemReportFilter()
        {
            return View();
        }
    }
}
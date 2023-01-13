using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class RpTopSaleItemController : MyController
    {
        public ActionResult TopSaleItemReport()
        {
            return View();
        }

        public ActionResult TopSaleItemReportFilter()
        {
            return View();
        }
    }
}
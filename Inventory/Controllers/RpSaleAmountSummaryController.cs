using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class RpSaleAmountSummaryController : MyController
    {
        public ActionResult SaleAmountSummaryReportFilter()
        {
            return View();
        }

        public ActionResult SaleAmountSummaryReport()
        {
            return View();
        }
    }
}
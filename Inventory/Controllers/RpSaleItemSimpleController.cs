using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class RpSaleItemSimpleController : MyController
    {       
        public ActionResult SaleItemSimpleReportFilter()
        {
            return View();
        }

        public ActionResult SaleItemSimpleReport()
        {
            return View();
        }
    }
}
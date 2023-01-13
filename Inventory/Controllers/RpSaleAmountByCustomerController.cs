using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class RpSaleAmountByCustomerController : MyController
    {
        public ActionResult SaleAmountByCustomerReportFilter()
        {
            return View();
        }

        public ActionResult SaleAmountByCustomerReport()
        {
            return View();
        }
    }
}
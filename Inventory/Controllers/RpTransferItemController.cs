using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class RpTransferItemController : MyController
    {
        public ActionResult TransferItemReportFilter()
        {
            return View();
        }
    }
}
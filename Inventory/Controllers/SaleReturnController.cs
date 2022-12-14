using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class SaleReturnController : MyController
    {
        public ActionResult SaleReturn()
        {
            return View();
        }

        public ActionResult ListSaleReturn()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class PurchaseReturnController : MyController
    {
        public ActionResult PurchaseReturn()
        {
            return View();
        }

        public ActionResult ListPurchaseReturn()
        {
            return View();
        }
    }
}
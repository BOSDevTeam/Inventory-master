using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class PurchaseOrderController : MyController
    {
        public ActionResult PurchaseOrder()
        {
            return View();
        }

        public ActionResult ListPurchaseOrder()
        {
            return View();
        }
    }
}
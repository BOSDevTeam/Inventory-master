using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class AdjustmentController : MyController
    {
        public ActionResult Adjustment()
        {
            return View();
        }

        public ActionResult ListAdjustment()
        {
            return View();
        }
    }
}
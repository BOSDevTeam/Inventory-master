using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class SaleOrderController : MyController
    {
        public ActionResult SaleOrder(int userId)
        {
            return View();
        }

        public ActionResult ListSaleOrder(int userId)
        {
            return View();
        }
    }
}
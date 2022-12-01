using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class CustomerConsignController : MyController
    {
        public ActionResult CustomerConsign()
        {
            return View();
        }

        public ActionResult ListCustomerConsign()
        {
            return View();
        }
    }
}
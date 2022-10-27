using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class PurchaseController : MyController
    {
       
        public ActionResult Purchase(int userId)
        {
            return View();
        }

        public ActionResult ListPurchase(int userId)
        {
            return View();
        }
    }
}
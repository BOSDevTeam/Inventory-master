using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class TransferController : MyController
    {
        public ActionResult Transfer()
        {
            return View();
        }

        public ActionResult ListTransfer()
        {
            return View();
        }
    }
}
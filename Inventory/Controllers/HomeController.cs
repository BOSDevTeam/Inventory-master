using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class HomeController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}
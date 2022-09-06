using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class PurchaseController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        Procedure procedure = new Procedure();
        MasterPurchaseModels model = new MasterPurchaseModels();

        public ActionResult Purchase()
        {
            getSupplier();
            return View(model);
        }

        public void getSupplier()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();

            SqlCommand cmd = new SqlCommand("Select SupplierID,SupplierName From S_Supplier", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.Suppliers.Add(new SelectListItem { Text = Convert.ToString(reader["SupplierName"]), Value = Convert.ToString(reader["SupplierID"]) });
            }
            reader.Close();
        }
    }
}
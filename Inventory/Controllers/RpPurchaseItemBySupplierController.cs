using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Common;
using Inventory.ViewModels;


namespace Inventory.Controllers
{
    public class RpPurchaseItemBySupplierController : MyController
    {
        // GET: RpPurchaseItemBySupplier
        RpPurchaseItemBySupplierViewModel purchaseViewModel = new RpPurchaseItemBySupplierViewModel();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PurchaseItemBySupplierReportFilter()
        {
            return View();
        }

        public ActionResult PurchaseItemBySupplierReport(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            purchaseViewModel.FromDate = fromDate;
            purchaseViewModel.ToDate = toDate;
            List<RpPurchaseItemBySupplierViewModel.TranPurchaseModels> lstTranPurchase = GetPurchaseItemBySupplier(fromDate, toDate, selectedLocationId);
            List<RpPurchaseItemBySupplierViewModel.SupplierModels> lstSupplier = GetSupplier(fromDate, toDate);
            purchaseViewModel.lstTranPurchase = lstTranPurchase;
            purchaseViewModel.lstSupplier = lstSupplier;
            return View(purchaseViewModel);
        }

        private List<RpPurchaseItemBySupplierViewModel.SupplierModels> GetSupplier(DateTime fromDate, DateTime toDate)
        {
            List<RpPurchaseItemBySupplierViewModel.SupplierModels> list = new List<RpPurchaseItemBySupplierViewModel.SupplierModels>();
            RpPurchaseItemBySupplierViewModel.SupplierModels item = new RpPurchaseItemBySupplierViewModel.SupplierModels();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSupplier, (SqlConnection)getConnction());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpPurchaseItemBySupplierViewModel.SupplierModels();
                item.SupplierID = Convert.ToInt32(reader["SupplierID"]);
                item.SupplierName = Convert.ToString(reader["SupplierName"]);
                list.Add(item);
            }
            reader.Close();
            return list;
        }

        private List<RpPurchaseItemBySupplierViewModel.TranPurchaseModels> GetPurchaseItemBySupplier(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            List<RpPurchaseItemBySupplierViewModel.TranPurchaseModels> list = new List<RpPurchaseItemBySupplierViewModel.TranPurchaseModels>();
            RpPurchaseItemBySupplierViewModel.TranPurchaseModels item = new RpPurchaseItemBySupplierViewModel.TranPurchaseModels();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptPurchaseBySupplier, (SqlConnection)getConnction());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpPurchaseItemBySupplierViewModel.TranPurchaseModels();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.SupplierID = Convert.ToInt32(reader["SupplierID"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);

                if (reader["Gold"] != DBNull.Value) item.Accessories += Resource.Gold + " " + reader["Gold"] + Resource.G + " ";
                if (reader["Pearl"] != DBNull.Value) item.Accessories += Resource.Pearl + " " + reader["Pearl"] + Resource.Rati + " ";
                if (reader["Diamond"] != DBNull.Value) item.Accessories += Resource.Diamond + " " + reader["Diamond"] + Resource.Carat + " ";
                if (reader["Stone"] != DBNull.Value) item.Accessories += Resource.Stone + " " + reader["Stone"] + Resource.Carat + " ";
                if (reader["Palatinum"] != DBNull.Value) item.Accessories += Resource.Palatinum + " " + reader["Palatinum"] + Resource.G + " ";

                list.Add(item);
            }
            reader.Close();
            return list;
        }

        private object getConnction()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null) Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();
            connection = Session[AppConstants.SQLConnection];
            return connection;
        }
    }
}
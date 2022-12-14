using Inventory.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using System.Data.SqlClient;
using Inventory.Common;
using System.Data;
using Inventory.Models;

namespace Inventory.Controllers
{
    public class StockStatusController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        StockStatusViewModel stockStatusViewModel = new StockStatusViewModel();
        AppData appData = new AppData();
        List<LocationModels.LocationModel> lstLocation = new List<LocationModels.LocationModel>();

        [SessionTimeoutAttribute]
        public ActionResult StockStatusLocation()
        {
            if (checkConnection())
            {
                getLocation();
                createHeader();
                return View(stockStatusViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        private List<StockStatusViewModel> selectStockStatus()
        {
            List<StockStatusViewModel> list = new List<StockStatusViewModel>();
            StockStatusViewModel item = new StockStatusViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetStockStatusByLocation, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new StockStatusViewModel();
                item.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.ShortName = Convert.ToString(reader["ShortName"]);
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductCode = Convert.ToString(reader["ProductCode"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Status = Convert.ToInt32(reader["Status"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        private void getStockStatus(List<StockStatusViewModel> list)
        {

        }

        private void createHeader()
        {
            StockStatusViewModel.HeaderViewModel item = new StockStatusViewModel.HeaderViewModel();
            item.ProductCode = "Product Code";
            item.ProductName = "Product Name";
            for (int i = 0; i < lstLocation.Count(); i++)
            {
                item.lstLocationName.Add(lstLocation[i].ShortName);
            }
        }

        private void getLocation()
        {
            lstLocation = appData.selectLocation(getConnection());
        }

        private bool checkConnection()
        {
            if (Session[AppConstants.SQLConnection] != null) return true;
            else return false;
        }

        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();

            connection = Session[AppConstants.SQLConnection];
            return connection;
        }
    }
}
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
        AppData appData = new AppData();
        List<LocationModels.LocationModel> lstLocation = new List<LocationModels.LocationModel>();

        [SessionTimeoutAttribute]
        public ActionResult StockStatusLocation()
        {
            if (checkConnection())
            {
                getLocation();
                ViewData["StockStatusHeader"] = createHeader(true);
                ViewData["StockStatusItem"] = selectStockStatus(true);
                return View();
            }
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public JsonResult PriceTypeChangeAction(bool isRequestValue)
        {
            getLocation();
            StockStatusViewModel.HeaderViewModel header = createHeader(isRequestValue);
            List<StockStatusViewModel.ItemViewModel> lstItem = selectStockStatus(isRequestValue);

            var jsonResult = new
            {
                Header = header,
                LstItem = lstItem
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        private List<StockStatusViewModel.ItemViewModel> selectStockStatus(bool isWithSalePrice)
        {
            int productId = 0;
            List<StockStatusViewModel.ItemViewModel> list = new List<StockStatusViewModel.ItemViewModel>();
            StockStatusViewModel.ItemViewModel item = new StockStatusViewModel.ItemViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetStockStatusByLocation, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("IsWithSalePrice", isWithSalePrice);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (productId != Convert.ToInt32(reader["ProductID"]))
                {
                    if (list.Count() != 0)
                    {
                        item = list.LastOrDefault();
                        for (int i = 0; i < item.lstLocationBalance.Count(); i++)
                        {
                            item.Balance += item.lstLocationBalance[i];
                        }
                        item.Amount = item.Balance * item.Price;
                        list.LastIndexOf(item);
                    }

                    item = new StockStatusViewModel.ItemViewModel();
                    item.ProductID = Convert.ToInt32(reader["ProductID"]);
                    item.ProductCode = Convert.ToString(reader["ProductCode"]);
                    item.ProductName = Convert.ToString(reader["ProductName"]);
                    item.Price = Convert.ToInt32(reader["Price"]);
                    item.lstLocationBalance.Add(Convert.ToInt32(reader["LocationBalance"]));
                    list.Add(item);
                }
                else
                {
                    item.lstLocationBalance.Add(Convert.ToInt32(reader["LocationBalance"]));
                    list.LastIndexOf(item);
                }
                productId = item.ProductID;
            }

            if (list.Count() != 0)
            {
                item = list.LastOrDefault();
                for (int i = 0; i < item.lstLocationBalance.Count(); i++)
                {
                    item.Balance += item.lstLocationBalance[i];
                }
                item.Amount = item.Balance * item.Price;
                list.LastIndexOf(item);
            }

            reader.Close();
            return list;
        }

        private StockStatusViewModel.HeaderViewModel createHeader(bool isWithSalePrice)
        {
            StockStatusViewModel.HeaderViewModel item = new StockStatusViewModel.HeaderViewModel();
            item.ProductCode = Resource.Code;
            item.ProductName = Resource.ProductName;
            item.Balance = Resource.Balance;
            for (int i = 0; i < lstLocation.Count(); i++)
            {
                item.lstLocationName.Add(lstLocation[i].ShortName);
            }
            item.Price = Resource.Price;
            item.Amount = Resource.Amount;
            return item;
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
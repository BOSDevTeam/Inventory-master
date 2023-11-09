using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using System.Data.SqlClient;
using Inventory.Common;
using System.Data;

namespace Inventory.Controllers
{
    public class RpStockStatusByModuleController : MyController
    {
        RpStockStatusByModuleViewModel stockStatusByModuleViewModel = new RpStockStatusByModuleViewModel();
        AppSetting setting = new AppSetting();

        public ActionResult StockStatusByModuleReport(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            stockStatusByModuleViewModel.lstDetail = selectStockStatusByModule(fromDate, toDate, selectedLocationId);
            stockStatusByModuleViewModel.FromDate = fromDate;
            stockStatusByModuleViewModel.ToDate = toDate;
            return View(stockStatusByModuleViewModel);
        }

        private List<RpStockStatusByModuleViewModel.DetailView> selectStockStatusByModule(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            List<RpStockStatusByModuleViewModel.DetailView> list = new List<RpStockStatusByModuleViewModel.DetailView>();
            RpStockStatusByModuleViewModel.DetailView item = new RpStockStatusByModuleViewModel.DetailView();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptStockStatusByModule, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpStockStatusByModuleViewModel.DetailView();
                item.Code = Convert.ToString(reader["ProductCode"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.OpeningStock = Convert.ToInt32(reader["OpeningStock"]);
                item.Sale = Convert.ToInt32(reader["Sale"]);
                item.Purchase = Convert.ToInt32(reader["Purchase"]);
                item.AdjustmentIncrease = Convert.ToInt32(reader["AdjustmentIncrease"]);
                item.AdjustmentDecrease = Convert.ToInt32(reader["AdjustmentDecrease"]);
                item.SaleReturn = Convert.ToInt32(reader["SaleReturn"]);
                item.PurchaseReturn = Convert.ToInt32(reader["PurchaseReturn"]);
                item.Balance = Convert.ToInt32(reader["Balance"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }
    }
}
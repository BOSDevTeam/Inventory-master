using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.ViewModels;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class RpSaleItemProfitController : MyController
    {
        AppSetting setting = new AppSetting();
        RpSaleItemProfitViewModel SaleItemProfit = new RpSaleItemProfitViewModel();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SaleItemProfitReport(DateTime fromDate,DateTime toDate)
        {
            try
            {
                SaleItemProfit.FromDate = fromDate;
                SaleItemProfit.ToDate = fromDate;
                SaleItemProfit.lstSaleItemProfit = GetSaleItemProfitReport(fromDate, toDate);
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(SaleItemProfit);
        }
        private List<RpSaleItemProfitViewModel> GetSaleItemProfitReport(DateTime fromDate,DateTime toDate)
        {
            List<RpSaleItemProfitViewModel> lstSaleItemProfit = new List<RpSaleItemProfitViewModel>();
            RpSaleItemProfitViewModel item = new RpSaleItemProfitViewModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleItemProfit, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = setting.conn;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpSaleItemProfitViewModel();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.Code = Convert.ToString(reader["Code"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                //item.SaleDiscount = Convert.ToInt32(reader["SaleDiscount"]);
                //item.PurchaseDiscount = Convert.ToInt32(reader["PurchaseDiscount"]);
                item.Balance = Convert.ToInt32(reader["Balance"]);
                //item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                lstSaleItemProfit.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return lstSaleItemProfit;
        }
    }
}
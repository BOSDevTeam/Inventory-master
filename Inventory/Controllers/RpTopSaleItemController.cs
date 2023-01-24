using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class RpTopSaleItemController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        RpTopSaleItemViewModel topSaleViewModel = new RpTopSaleItemViewModel();
        public ActionResult TopSaleItemReport(DateTime fromDate, DateTime toDate)
        {
            topSaleViewModel.FromDate = fromDate;
            topSaleViewModel.ToDate = toDate;
            List<RpTopSaleItemViewModel.TranSaleModels> lstTranSale = GetTranSaleReport(fromDate, toDate);
            topSaleViewModel.lstTranSaleRpt = lstTranSale;
            return View(topSaleViewModel);
        }

        public ActionResult TopSaleItemReportFilter()
        {
            return View();
        }

        private List<RpTopSaleItemViewModel.TranSaleModels> GetTranSaleReport(DateTime fromDate, DateTime toDate)
        {
            List<RpTopSaleItemViewModel.TranSaleModels> list = new List<RpTopSaleItemViewModel.TranSaleModels>();
            RpTopSaleItemViewModel.TranSaleModels data = new RpTopSaleItemViewModel.TranSaleModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptTopSaleItem, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data = new RpTopSaleItemViewModel.TranSaleModels();
                data.ProductCode = Convert.ToString(reader["Code"]);
                data.ProductID = Convert.ToInt32(reader["ProductID"]);
                data.ProductName = Convert.ToString(reader["ProductName"]);
                data.Quantity = Convert.ToInt32(reader["Quantity"]);
                data.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                data.Discount = Convert.ToInt32(reader["Discount"]);
                data.Amount = Convert.ToInt32(reader["Amount"]);
                list.Add(data);
                //int totalQuantity = 0, totalAmount = 0;
                //foreach (var sale in list)
                //{
                //    foreach (var item in list.Where(m => m.ProductName == sale.ProductName && m.SalePrice != sale.SalePrice))
                //    {

                //        totalQuantity = list.Where(m => m.ProductName == item.ProductName && item.SalePrice != sale.SalePrice).Sum(m => m.Quantity);
                //        totalAmount = list.Where(m => m.ProductName == item.ProductName && item.SalePrice != sale.SalePrice).Sum(m => m.Amount);
                //        topSaleViewModel.TotalQuantity = totalQuantity;
                //        topSaleViewModel.TotalAmount = totalAmount;
                //    }
                //}

            }
            reader.Close();
            return list;
        }

        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null) Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();
                connection = Session[AppConstants.SQLConnection];    
            return connection;
        }
    }
}
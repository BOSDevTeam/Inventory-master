using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class RpBottomSaleItemController : MyController
    {
        RpBottomSaleItemViewModel bottomSale = new RpBottomSaleItemViewModel();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        public ActionResult BottomSaleItemReport(DateTime fromDate, DateTime toDate)
        {
            bottomSale = new RpBottomSaleItemViewModel();
            List<RpBottomSaleItemViewModel.TranSaleModels> lstTranSale = GetSaleReport(fromDate,toDate);
            bottomSale.lstTranSale = lstTranSale;
            bottomSale.FromDate = fromDate;
            bottomSale.ToDate = toDate;
            return View(bottomSale);
        }

        public ActionResult BottomSaleItemReportFilter()
        {
            return View();
        }


        public List<RpBottomSaleItemViewModel.TranSaleModels> GetSaleReport(DateTime fromDate, DateTime toDate)
        {
            List<RpBottomSaleItemViewModel.TranSaleModels> list = new List<RpBottomSaleItemViewModel.TranSaleModels>();
            RpBottomSaleItemViewModel.TranSaleModels item = new RpBottomSaleItemViewModel.TranSaleModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptBottomSale, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpBottomSaleItemViewModel.TranSaleModels();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                list.Add(item);
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
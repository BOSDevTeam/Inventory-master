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
    public class RpTransferItemController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        RpTransferItemViewModel transferItemViewModel = new RpTransferItemViewModel();
        public ActionResult TransferItemReportFilter()
        {
            return View();
        }

        public ActionResult TransferItemReport(DateTime fromDate, DateTime toDate)
        {
            transferItemViewModel.FromDate = fromDate;
            transferItemViewModel.ToDate = toDate;
            List<RpTransferItemViewModel.MasterTransferModels> lstMasterTransfer = GetRptMasterTransfer(fromDate, toDate);
            transferItemViewModel.lstMasterTransfer = lstMasterTransfer;
            List<RpTransferItemViewModel.TranTransferModels> lstTranTransfer = GetRptTranTransfer(fromDate, toDate);
            transferItemViewModel.lstTranTransfer = lstTranTransfer;
            return View(transferItemViewModel);
        }

        private List<RpTransferItemViewModel.MasterTransferModels> GetRptMasterTransfer(DateTime fromDate, DateTime toDate)
        {
            List<RpTransferItemViewModel.MasterTransferModels> list = new List<RpTransferItemViewModel.MasterTransferModels>();
            RpTransferItemViewModel.MasterTransferModels item = new RpTransferItemViewModel.MasterTransferModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptMasterTransferItem, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpTransferItemViewModel.MasterTransferModels();
                item.FromLocationID = Convert.ToInt32(reader["FromLocationID"]);
                item.FromLocationName = Convert.ToString(reader["FromLocation"]);
                item.ToLocationID = Convert.ToInt32(reader["ToLocationID"]);
                item.ToLocationName = Convert.ToString(reader["ToLocation"]);
                list.Add(item);
            }

            reader.Close();

            return list;
        }

        private List<RpTransferItemViewModel.TranTransferModels> GetRptTranTransfer(DateTime fromDate, DateTime toDate)
        {
            List<RpTransferItemViewModel.TranTransferModels> list = new List<RpTransferItemViewModel.TranTransferModels>();
            RpTransferItemViewModel.TranTransferModels item = new RpTransferItemViewModel.TranTransferModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptTranTransferItem, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpTransferItemViewModel.TranTransferModels();
                item.FromLocationID = Convert.ToInt32(reader["FromLocationID"]);
                item.ToLocationID = Convert.ToInt32(reader["ToLocationID"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.PurPrice = Convert.ToInt32(reader["PurPrice"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.UnitKeyword = Convert.ToString(reader["Keyword"]);
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
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
    public class RpTransferVoucherController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        RpTransferVoucherViewModel transferVoucherViewModel = new RpTransferVoucherViewModel();
        public ActionResult TransferVoucherReportFilter()
        {
            return View();
        }
        public ActionResult TransferVoucherReport(DateTime fromDate, DateTime toDate)
        {
            transferVoucherViewModel.FromDate = fromDate;
            transferVoucherViewModel.ToDate = toDate;
            List<RpTransferVoucherViewModel.MasterTransferModels> lstMasterTransfer = GetRptMasterTransfer(fromDate, toDate);
            transferVoucherViewModel.lstMasterTransfer = lstMasterTransfer;
            List<RpTransferVoucherViewModel.TranTransferModels> lstTranTransfer = GetRptTranTransfer(fromDate, toDate);
            transferVoucherViewModel.lstTranTransfer = lstTranTransfer;
            return View(transferVoucherViewModel);
        }

        public List<RpTransferVoucherViewModel.MasterTransferModels> GetRptMasterTransfer(DateTime fromDate, DateTime toDate)
        {
            List<RpTransferVoucherViewModel.MasterTransferModels> list = new List<RpTransferVoucherViewModel.MasterTransferModels>();
            RpTransferVoucherViewModel.MasterTransferModels item = new RpTransferVoucherViewModel.MasterTransferModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptMasterTransferList, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpTransferVoucherViewModel.MasterTransferModels();
                item.TransferID = Convert.ToInt32(reader["TransferID"]);
                item.TransferDateTime = Convert.ToString(reader["TransferDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                item.FromLocationID = Convert.ToInt32(reader["FromLocationID"]);
                item.FromLocationName = Convert.ToString(reader["FromLocation"]);
                item.ToLocationID = Convert.ToInt32(reader["ToLocationID"]);
                item.ToLocationName = Convert.ToString(reader["ToLocation"]);
                item.Remark = Convert.ToString(reader["Remark"]);
                list.Add(item);
            }

            reader.Close();

            return list;
        }

        public List<RpTransferVoucherViewModel.TranTransferModels> GetRptTranTransfer(DateTime fromDate, DateTime toDate)
        {
            List<RpTransferVoucherViewModel.TranTransferModels> list = new List<RpTransferVoucherViewModel.TranTransferModels>();
            RpTransferVoucherViewModel.TranTransferModels item = new RpTransferVoucherViewModel.TranTransferModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptTranTransferList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpTransferVoucherViewModel.TranTransferModels();
                item.TransferID = Convert.ToInt32(reader["TransferID"]);
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.UnitKeyword = Convert.ToString(reader["Keyword"]);
                list.Add(item);
            }

            reader.Close();

            return list;
        }


        public object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null) Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();
            connection = Session[AppConstants.SQLConnection];
            return connection;
        }
    }
}
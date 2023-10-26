using Inventory.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class RpSaleAmountByMultiPayController : MyController
    {
        AppSetting setting = new AppSetting();
        AppData appData = new AppData();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();

        public ActionResult SaleAmountByMultiPayReport(DateTime fromDate, DateTime toDate)
        {
            RpSaleAmountByMultiPayViewModel viewModel = new RpSaleAmountByMultiPayViewModel();
            viewModel.lstMultiPayAmount = selectMultiPayAmount(fromDate, toDate);
            viewModel.lstBankPayment = appData.selectBankPayment(getConnection());
            viewModel.FromDate = fromDate;
            viewModel.ToDate = toDate;
            return View(viewModel);
        }

        private List<RpSaleAmountByMultiPayViewModel.MultiPayAmountView> selectMultiPayAmount(DateTime fromDate, DateTime toDate)
        {
            List<RpSaleAmountByMultiPayViewModel.MultiPayAmountView> lstMultiPayAmount = new List<RpSaleAmountByMultiPayViewModel.MultiPayAmountView>();
            RpSaleAmountByMultiPayViewModel.MultiPayAmountView data;
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleAmountByMultiPay, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data = new RpSaleAmountByMultiPayViewModel.MultiPayAmountView();
                data.SaleID = Convert.ToInt32(reader["SaleID"]);
                data.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                data.SlipID = Convert.ToInt32(reader["SlipID"]);
                data.PayMethodID = Convert.ToInt32(reader["PayMethodID"]);
                data.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
                data.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                data.MultiPayMethodID = Convert.ToInt32(reader["MultiPayMethodID"]);
                data.MultiBankPaymentID = Convert.ToInt32(reader["MultiBankPaymentID"]);
                data.MultiGrandtotal = Convert.ToInt32(reader["MultiGrandtotal"]);               
                lstMultiPayAmount.Add(data);
            }
            reader.Close();
            setting.conn.Close();
            return lstMultiPayAmount;
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
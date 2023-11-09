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
    public class RpPurchaseAmountOnlyController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        RpPurchaseAmountOnlyViewModel purchaseAmountViewModel = new RpPurchaseAmountOnlyViewModel();
        public ActionResult PurchaseAmountOnlyReportFilter()
        {
            return View();
        }
        public ActionResult PurchaseAmountOnlyReport(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            purchaseAmountViewModel.FromDate = fromDate;
            purchaseAmountViewModel.ToDate = toDate;
            List<RpPurchaseAmountOnlyViewModel.PayMethod> lstPayMethod = GetRptPayMethod(fromDate, toDate);
            purchaseAmountViewModel.lstPayMethod = lstPayMethod;
            List<RpPurchaseAmountOnlyViewModel.BankPayment> lstBankPayment = GetBankPayment();
            purchaseAmountViewModel.lstBankPayment = lstBankPayment;
            List<RpPurchaseAmountOnlyViewModel.MasterPurchaseModels> lstMasterPurchase = GetPurchaseReport(fromDate, toDate, selectedLocationId);
            purchaseAmountViewModel.lstMasterPurchase = lstMasterPurchase;
            return View(purchaseAmountViewModel);
        }

        private List<RpPurchaseAmountOnlyViewModel.PayMethod> GetRptPayMethod(DateTime fromDate, DateTime toDate)
        {
            List<RpPurchaseAmountOnlyViewModel.PayMethod> payMethodList = new List<RpPurchaseAmountOnlyViewModel.PayMethod>();
            RpPurchaseAmountOnlyViewModel.PayMethod item = new RpPurchaseAmountOnlyViewModel.PayMethod();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptPayMethod, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpPurchaseAmountOnlyViewModel.PayMethod();
                item.PayMethodID = Convert.ToInt32(reader["PayMethodID"]);
                item.PayMethodName = Convert.ToString(reader["PayMethodName"]);
                payMethodList.Add(item);
            }

            reader.Close();
            return payMethodList;
        }

        private List<RpPurchaseAmountOnlyViewModel.BankPayment> GetBankPayment()
        {
            List<RpPurchaseAmountOnlyViewModel.BankPayment> list = new List<RpPurchaseAmountOnlyViewModel.BankPayment>();
            RpPurchaseAmountOnlyViewModel.BankPayment item = new RpPurchaseAmountOnlyViewModel.BankPayment();
            SqlCommand cmd = new SqlCommand(TextQuery.bankPaymentQuery, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpPurchaseAmountOnlyViewModel.BankPayment();
                item.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
                item.BankPaymentName = Convert.ToString(reader["BankPaymentName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        private List<RpPurchaseAmountOnlyViewModel.MasterPurchaseModels> GetPurchaseReport(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            List<RpPurchaseAmountOnlyViewModel.MasterPurchaseModels> list = new List<RpPurchaseAmountOnlyViewModel.MasterPurchaseModels>();
            RpPurchaseAmountOnlyViewModel.MasterPurchaseModels item = new RpPurchaseAmountOnlyViewModel.MasterPurchaseModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptPurchaseAmountOnly, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpPurchaseAmountOnlyViewModel.MasterPurchaseModels();
                item.PurchaseID = Convert.ToInt32(reader["PurchaseID"]);
                item.PurchaseDateTime = Convert.ToString(reader["PurDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                item.PaymentName = Convert.ToString(reader["PaymentName"]);
                item.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.VouDisAmount = Convert.ToInt32(reader["VouDisAmount"]);
                item.VoucherFOC = Convert.ToInt32(reader["VoucherFOC"]);
                item.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                item.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                item.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
                if (item.IsVouFOC == true)
                {
                    item.Grandtotal = 0;
                }
                else
                {
                    item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                }

                item.PayMethodID = Convert.ToInt32(reader["PayMethodID"]);
                item.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
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
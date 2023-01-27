using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using Inventory.Filters;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class RpSaleAmountOnlyController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        RpSaleAmountOnlyViewModel saleAmountViewModel = new RpSaleAmountOnlyViewModel();   
        public ActionResult SaleAmountOnlyReportFilter()
        {
            return View();
        }

        public ActionResult SaleAmountOnlyReport(DateTime fromDate, DateTime toDate)
        {
            saleAmountViewModel = new RpSaleAmountOnlyViewModel();
            List<RpSaleAmountOnlyViewModel.MasterSaleModels> lstSaleReport = GetSaleReport(fromDate, toDate);
            List<RpSaleAmountOnlyViewModel.BankPayment> lstBankPayment = GetBankPayment(fromDate, toDate);
            saleAmountViewModel.FromDate = fromDate;
            saleAmountViewModel.ToDate = toDate;
            saleAmountViewModel.lstMasterSaleRpt = lstSaleReport;
            saleAmountViewModel.lstBankPayment = lstBankPayment;
            return View(saleAmountViewModel);
        }

        private List<RpSaleAmountOnlyViewModel.BankPayment> GetBankPayment(DateTime fromDate, DateTime toDate)
        {
            List<RpSaleAmountOnlyViewModel.BankPayment> list = new List<RpSaleAmountOnlyViewModel.BankPayment>();
            RpSaleAmountOnlyViewModel.BankPayment item = new RpSaleAmountOnlyViewModel.BankPayment();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptBankPayment, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpSaleAmountOnlyViewModel.BankPayment();
                item.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
                item.BankPaymentName = Convert.ToString(reader["BankPaymentName"]);
                list.Add(item);
            }
            reader.Close();
            
            return list;
        }

        private List<RpSaleAmountOnlyViewModel.MasterSaleModels> GetSaleReport(DateTime fromDate, DateTime toDate)
        {
            List<RpSaleAmountOnlyViewModel.MasterSaleModels> list = new List<RpSaleAmountOnlyViewModel.MasterSaleModels>();
            RpSaleAmountOnlyViewModel.MasterSaleModels item = new RpSaleAmountOnlyViewModel.MasterSaleModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleAmountOnly, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpSaleAmountOnlyViewModel.MasterSaleModels();
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                item.SaleDateTime = Convert.ToString(reader["SaleDateTime"]);
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
                if (item.PayMethodID == 1)
                {
                    saleAmountViewModel.PayMethodName = Convert.ToString(reader["PayMethodName"]);
                }
                else if (item.PayMethodID == 2)
                {
                    saleAmountViewModel.PayMethodName2 = Convert.ToString(reader["PayMethodName"]);
                }
                item.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        private object getConnection()
        {
            object conncetion;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();
            conncetion = Session[AppConstants.SQLConnection];
            return conncetion;
        }

        private bool checkConnection()
        {
            if (Session[AppConstants.SQLConnection] != null) return true;
            else return false;
        }
    }
}
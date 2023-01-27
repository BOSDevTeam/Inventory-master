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
    public class RpSaleAmountSummaryController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        RpSaleAmountSummaryViewModel saleViewModel = new RpSaleAmountSummaryViewModel();
        public ActionResult SaleAmountSummaryReportFilter()
        {
            return View();
        }

        public ActionResult SaleAmountSummaryReport(DateTime fromDate, DateTime toDate)
        {
            saleViewModel = new RpSaleAmountSummaryViewModel();
            List<RpSaleAmountSummaryViewModel.BankItem> lstBankItem = GetBankPayment(fromDate, toDate);
            List<RpSaleAmountSummaryViewModel.SaleItem> lstSaleItem = GetRptSale(fromDate, toDate);
            saleViewModel.FromDate = fromDate;
            saleViewModel.ToDate = toDate;
            saleViewModel.lstBankItem = lstBankItem;
            saleViewModel.lstSaleItem = lstSaleItem;
            return View(saleViewModel);
        }

        private List<RpSaleAmountSummaryViewModel.BankItem> GetBankPayment(DateTime fromDate, DateTime toDate)
        {
            List<RpSaleAmountSummaryViewModel.BankItem> list = new List<RpSaleAmountSummaryViewModel.BankItem>();
            RpSaleAmountSummaryViewModel.BankItem item = new RpSaleAmountSummaryViewModel.BankItem();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptBankPayment, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpSaleAmountSummaryViewModel.BankItem();
                item.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
                item.BankPaymentName = Convert.ToString(reader["BankPaymentName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        private List<RpSaleAmountSummaryViewModel.SaleItem> GetRptSale(DateTime fromDate, DateTime toDate)
        {
            List<RpSaleAmountSummaryViewModel.SaleItem> list = new List<RpSaleAmountSummaryViewModel.SaleItem>();
            RpSaleAmountSummaryViewModel.SaleItem item = new RpSaleAmountSummaryViewModel.SaleItem();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleAmountSummary, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpSaleAmountSummaryViewModel.SaleItem();
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                item.SlipID = Convert.ToInt32(reader["SlipID"]);
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
                    saleViewModel.PayMethodName = Convert.ToString(reader["PayMethodName"]);
                }
                else if (item.PayMethodID == 2)
                {
                    saleViewModel.PayMethodName2 = Convert.ToString(reader["PayMethodName"]);
                }
                item.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
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
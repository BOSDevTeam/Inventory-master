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
    public class RpSaleAuditController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        RpSaleAuditViewModel saleAuditViewModel = new RpSaleAuditViewModel();

        public ActionResult SaleAuditReportFilter()
        {
            return View();
        }

        public ActionResult SaleAuditReport(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            saleAuditViewModel.lstMasterSale = selectMasterSale(fromDate, toDate,selectedLocationId);
            saleAuditViewModel.lstTranSale = selectTranSale(fromDate, toDate, selectedLocationId);
            saleAuditViewModel.lstMultiPay = selectMultiPaySale(fromDate, toDate, selectedLocationId);
            saleAuditViewModel.FromDate = fromDate;
            saleAuditViewModel.ToDate = toDate;
            return View(saleAuditViewModel);
        }

        private List<RpSaleAuditViewModel.MasterSaleView> selectMasterSale(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            List<RpSaleAuditViewModel.MasterSaleView> lstMasterSale = new List<RpSaleAuditViewModel.MasterSaleView>();
            RpSaleAuditViewModel.MasterSaleView item = new RpSaleAuditViewModel.MasterSaleView();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptMasterSaleList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;                     
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpSaleAuditViewModel.MasterSaleView();
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                item.SaleDateTime = Convert.ToString(reader["SaleDateTime"]);
                item.User = Convert.ToString(reader["User"]);
                item.Client = Convert.ToString(reader["Client"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.Location = Convert.ToString(reader["Location"]);
                item.PaymentKeyword = Convert.ToString(reader["PaymentKeyword"]);
                item.PayMethodName = Convert.ToString(reader["PayMethodName"]);
                item.BankPayment = Convert.ToString(reader["BankPayment"]);
                item.SlipID = Convert.ToInt32(reader["SlipID"]);
                item.LimitedDay = Convert.ToString(reader["LimitedDay"]);
                item.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.Total = Convert.ToInt32(reader["Total"]);
                item.VouDisPercent = Convert.ToInt32(reader["VouDisPercent"]);
                item.VouDisAmount = Convert.ToInt32(reader["VouDisAmount"]);
                item.VoucherDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                item.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                item.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);
                item.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);              
                item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                item.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
                item.VoucherFOC = Convert.ToInt32(reader["VoucherFOC"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                if (Convert.ToInt32(reader["PayMethodID"]) == 3)
                {
                    item.PayMethodName = AppConstants.Message.MultiPay;
                }
                lstMasterSale.Add(item);
            }
            reader.Close();
            return lstMasterSale;
        }

        private List<RpSaleAuditViewModel.TranSaleView> selectTranSale(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            List<RpSaleAuditViewModel.TranSaleView> lstTranSale = new List<RpSaleAuditViewModel.TranSaleView>();
            RpSaleAuditViewModel.TranSaleView item = new RpSaleAuditViewModel.TranSaleView();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptTranSaleList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpSaleAuditViewModel.TranSaleView();
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                item.Code = Convert.ToString(reader["Code"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                item.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                lstTranSale.Add(item);
            }
            reader.Close();
            return lstTranSale;
        }

        private List<RpSaleAuditViewModel.MultiPayView> selectMultiPaySale(DateTime fromDate, DateTime toDate, int? selectedLocationId)
        {
            List<RpSaleAuditViewModel.MultiPayView> lstMultiPay = new List<RpSaleAuditViewModel.MultiPayView>();
            RpSaleAuditViewModel.MultiPayView item = new RpSaleAuditViewModel.MultiPayView();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptMultiPaySaleList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpSaleAuditViewModel.MultiPayView();
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                item.PayMethodID = Convert.ToInt32(reader["PayMethodID"]);
                item.PayMethodName = Convert.ToString(reader["PayMethodName"]);
                item.BankPaymentName = Convert.ToString(reader["BankPaymentName"]);
                item.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                lstMultiPay.Add(item);
            }
            reader.Close();
            return lstMultiPay;
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
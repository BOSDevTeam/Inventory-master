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
        RpSaleAmountViewModel saleAmountViewModel = new RpSaleAmountViewModel();   
        public ActionResult SaleAmountOnlyReportFilter()
        {
            return View();
        }

        public ActionResult SaleAmountOnlyReport(DateTime fromDate, DateTime toDate)
        {
            saleAmountViewModel = new RpSaleAmountViewModel();
            List<RpSaleAmountViewModel.MasterSaleView> lstSaleReport = GetSaleReport(fromDate, toDate);           
            saleAmountViewModel.FromDate = fromDate;
            saleAmountViewModel.ToDate = toDate;
            saleAmountViewModel.lstMasterSaleRpt = lstSaleReport;       
            return View(saleAmountViewModel);
        }     

        private List<RpSaleAmountViewModel.MasterSaleView> GetSaleReport(DateTime fromDate, DateTime toDate)
        {
            List<RpSaleAmountViewModel.MasterSaleView> list = new List<RpSaleAmountViewModel.MasterSaleView>();
            RpSaleAmountViewModel.MasterSaleView item = new RpSaleAmountViewModel.MasterSaleView();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleAmountOnlyAndSummary, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new RpSaleAmountViewModel.MasterSaleView();
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                item.SaleDateTime = Convert.ToString(reader["SaleDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.CashInHand = Convert.ToInt32(reader["CashInHand"]);
                item.Banking = Convert.ToInt32(reader["Banking"]);
                item.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.VoucherDiscount = Convert.ToInt32(reader["VoucherDiscount"]);               
                item.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                item.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                item.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
                item.VoucherFOC = Convert.ToInt32(reader["VoucherFOC"]);
                if (item.IsVouFOC == true) item.Grandtotal = 0;                
                else item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);                             
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
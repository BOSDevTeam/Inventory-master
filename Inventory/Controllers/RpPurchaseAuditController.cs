using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Filters;
using Inventory.Common;
using Inventory.ViewModels;

namespace Inventory.Controllers
{
    public class RpPurchaseAuditController : MyController
    {
        // GET: RpPurchaseAudit
        AppSetting setting = new AppSetting();
        RpPurchaseAuditViewModel purchaseAuditViewModel = new RpPurchaseAuditViewModel();
        [SessionTimeoutAttribute]
        public ActionResult PurchaseAuditReportFilter()
        {
            return View();
        }

        [SessionTimeoutAttribute]
        public ActionResult PurchaseAuditReport(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            try
            {
                List<RpPurchaseAuditViewModel.MasterPurchaseView> lstMasterPurchase = GetRptMasterPurchase(fromDate,toDate, selectedLocationId);
                List<RpPurchaseAuditViewModel.TranPurchaseView> lstTranPurchase = GetRptTranPurchase(fromDate,toDate, selectedLocationId);
                purchaseAuditViewModel.lstMasterPurchase = lstMasterPurchase;
                purchaseAuditViewModel.lstTranPurchase = lstTranPurchase;
                purchaseAuditViewModel.FromDate = fromDate;
                purchaseAuditViewModel.ToDate = toDate;
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }            
            return View(purchaseAuditViewModel);
        }

        public List<RpPurchaseAuditViewModel.MasterPurchaseView> GetRptMasterPurchase(DateTime FromDate,DateTime ToDate, int? selectedLocationId)
        {
            List<RpPurchaseAuditViewModel.MasterPurchaseView> lstRptMasterPurchase = new List<RpPurchaseAuditViewModel.MasterPurchaseView>();
            RpPurchaseAuditViewModel.MasterPurchaseView masterPurchase = new RpPurchaseAuditViewModel.MasterPurchaseView();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptMasterPurchaseList, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                masterPurchase = new RpPurchaseAuditViewModel.MasterPurchaseView();
                masterPurchase.PurchaseID = Convert.ToInt32(reader["PurchaseID"]);
                masterPurchase.PurDateTime = Convert.ToDateTime(reader["PurDateTime"]);
                masterPurchase.User = Convert.ToString(reader["UserName"]);
                masterPurchase.SupplierName = Convert.ToString(reader["SupplierName"]);
                masterPurchase.LocationName = Convert.ToString(reader["LocationName"]);
                masterPurchase.CurrencyName = Convert.ToString(reader["CurrencyName"]);
                masterPurchase.PaymentKeyword = Convert.ToString(reader["PaymentKeyword"]);
                masterPurchase.PayMethodName = Convert.ToString(reader["PayMethodName"]);
                masterPurchase.BankPayment = Convert.ToString(reader["BankPaymentName"]);
                masterPurchase.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                masterPurchase.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                masterPurchase.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                masterPurchase.Total = Convert.ToInt32(reader["Total"]);
                masterPurchase.VouDisPercent = Convert.ToInt32(reader["VouDisPercent"]);
                masterPurchase.VouDisAmount = Convert.ToInt32(reader["VouDisAmount"]);
                masterPurchase.VoucherDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                masterPurchase.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                masterPurchase.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);
                masterPurchase.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                masterPurchase.Grandtotal = Convert.ToInt32(reader["GrandTotal"]);
                masterPurchase.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
                masterPurchase.VoucherFOC = Convert.ToInt32(reader["VoucherFOC"]);
                lstRptMasterPurchase.Add(masterPurchase);
            }
            reader.Close();
            setting.conn.Close();
            return lstRptMasterPurchase;
        }
        public List<RpPurchaseAuditViewModel.TranPurchaseView> GetRptTranPurchase(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            List<RpPurchaseAuditViewModel.TranPurchaseView> lstRptTranPurchase = new List<RpPurchaseAuditViewModel.TranPurchaseView>();
            RpPurchaseAuditViewModel.TranPurchaseView tranPurchase = new RpPurchaseAuditViewModel.TranPurchaseView();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptTranPurchaseList, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tranPurchase = new RpPurchaseAuditViewModel.TranPurchaseView();
                tranPurchase.PurchaseID = Convert.ToInt32(reader["PurchaseID"]);
                tranPurchase.Quantity = Convert.ToInt32(reader["Quantity"]);
                tranPurchase.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                tranPurchase.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                tranPurchase.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                tranPurchase.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                tranPurchase.Discount = Convert.ToInt32(reader["Discount"]);
                tranPurchase.Amount = Convert.ToInt32(reader["Amount"]);
                tranPurchase.IsFOC = Convert.ToBoolean(reader["IsFOC"]);

                if (reader["Gold"] != DBNull.Value) tranPurchase.Accessories += Resource.Gold + " " + reader["Gold"] + Resource.G + " ";
                if (reader["Pearl"] != DBNull.Value) tranPurchase.Accessories += Resource.Pearl + " " + reader["Pearl"] + Resource.Rati + " ";
                if (reader["Diamond"] != DBNull.Value) tranPurchase.Accessories += Resource.Diamond + " " + reader["Diamond"] + Resource.Carat + " ";
                if (reader["Stone"] != DBNull.Value) tranPurchase.Accessories += Resource.Stone + " " + reader["Stone"] + Resource.Carat + " ";
                if (reader["Palatinum"] != DBNull.Value) tranPurchase.Accessories += Resource.Palatinum + " " + reader["Palatinum"] + Resource.G + " ";

                if (tranPurchase.Accessories != null) tranPurchase.ProductName = Convert.ToString(reader["ProductName"]) + Resource.OpenParenthesis + tranPurchase.Accessories.Trim() + Resource.CloseParenthesis;
                else tranPurchase.ProductName = Convert.ToString(reader["ProductName"]);

                lstRptTranPurchase.Add(tranPurchase);
            }
            reader.Close();
            setting.conn.Close();
            return lstRptTranPurchase;
        }
    }
}
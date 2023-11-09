using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.ViewModels;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class RpPurchaseLogController : MyController
    {
        AppSetting setting = new AppSetting();
        RpPurchaseLogViewModel purchaseLog = new RpPurchaseLogViewModel();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PurchaseLogReport(DateTime fromDate,DateTime toDate,int logType, int? selectedLocationId)
        {
            try
            {
                purchaseLog.FromDate = fromDate;
                purchaseLog.ToDate = toDate;
                purchaseLog.LogType = logType;
                purchaseLog.lstMasterPurchaseLog = GetRptPurchaseLog(fromDate, toDate, logType,selectedLocationId);
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(purchaseLog);
        }
        private List<RpPurchaseLogViewModel.MasterPurchaseLogViewModel> GetRptPurchaseLog(DateTime fromDate,DateTime toDate,int logType, int? selectedLocationId)
        {
            List<RpPurchaseLogViewModel.MasterPurchaseLogViewModel> lstMasterPurchaseLog = new List<RpPurchaseLogViewModel.MasterPurchaseLogViewModel>();
            List<RpPurchaseLogViewModel.PurchaseLogViewModel> lstPurchaseLog = new List<RpPurchaseLogViewModel.PurchaseLogViewModel>();
            RpPurchaseLogViewModel.MasterPurchaseLogViewModel master = new RpPurchaseLogViewModel.MasterPurchaseLogViewModel();
            RpPurchaseLogViewModel.PurchaseLogViewModel item = new RpPurchaseLogViewModel.PurchaseLogViewModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptPurchaseLog, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = setting.conn;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@LogType", logType);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                master = new RpPurchaseLogViewModel.MasterPurchaseLogViewModel();
                master.PurchaseLogID = Convert.ToInt32(reader["LogID"]);
                if (lstMasterPurchaseLog.Where(m => m.PurchaseLogID == master.PurchaseLogID).Count() > 0)
                {
                    item = new RpPurchaseLogViewModel.PurchaseLogViewModel();
                    item.Code = Convert.ToString(reader["Code"]);
                    item.ProductName = Convert.ToString(reader["ProductName"]);
                    if (logType == 2) item.OrginalQuantity = Convert.ToInt32(reader["OriginalQuantity"]);
                    item.Quantity = Convert.ToInt32(reader["Quantity"]);
                    item.UnitKeyword = Convert.ToString(reader["Keyword"]);
                    item.Amount = Convert.ToInt32(reader["Amount"]);
                    item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                    foreach(var PurchaseLog in lstMasterPurchaseLog.Where(m => m.PurchaseLogID == master.PurchaseLogID))
                    {
                        PurchaseLog.lstPurchaseLog.Add(item);
                    }
                }
                else
                {
                    master.PurchaseDateTime = Convert.ToDateTime(reader["UpdatedDateTime"]);
                    master.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                    master.EntryUserName = Convert.ToString(reader["EntryUserName"]);
                    master.UpdateUserName = Convert.ToString(reader["UpdatedUserName"]);
                    lstPurchaseLog = new List<RpPurchaseLogViewModel.PurchaseLogViewModel>();
                    item = new RpPurchaseLogViewModel.PurchaseLogViewModel();
                    item.Code = Convert.ToString(reader["Code"]);
                    item.ProductName = Convert.ToString(reader["ProductName"]);
                    if (logType == 2) item.OrginalQuantity = Convert.ToInt32(reader["OriginalQuantity"]);
                    item.Quantity = Convert.ToInt32(reader["Quantity"]);
                    item.UnitKeyword = Convert.ToString(reader["Keyword"]);
                    item.Amount = Convert.ToInt32(reader["Amount"]);
                    item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                    lstPurchaseLog.Add(item);
                    master.lstPurchaseLog = lstPurchaseLog;
                    lstMasterPurchaseLog.Add(master);
                }
            }
            reader.Close();
            setting.conn.Close();
            return lstMasterPurchaseLog;
        }
    }
}
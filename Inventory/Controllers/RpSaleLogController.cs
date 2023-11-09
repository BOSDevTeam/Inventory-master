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
    public class RpSaleLogController : MyController
    {
        RpSaleLogViewModel SaleLog = new RpSaleLogViewModel();
        AppSetting setting = new AppSetting();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SaleLogReport(DateTime fromDate,DateTime toDate,int logType, int? selectedLocationId)
        {
            SaleLog.lstMasterSaleLog = GetSaleLogData(fromDate, toDate, logType,selectedLocationId);
            try
            {
                SaleLog.FromDate = fromDate;
                SaleLog.ToDate = toDate;
                SaleLog.LogType = logType;
                
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(SaleLog);
        }
        private List<RpSaleLogViewModel.MasterSaleLogViewModel> GetSaleLogData(DateTime fromDate,DateTime toDate,int logType, int? selectedLocationId)
        {
            List<RpSaleLogViewModel.MasterSaleLogViewModel> lstMasterSaleLog = new List<RpSaleLogViewModel.MasterSaleLogViewModel>();
            List<RpSaleLogViewModel.SaleLogViewModel> lstSaleLog = new List<RpSaleLogViewModel.SaleLogViewModel>();
            RpSaleLogViewModel.MasterSaleLogViewModel master = new RpSaleLogViewModel.MasterSaleLogViewModel();
            RpSaleLogViewModel.SaleLogViewModel item = new RpSaleLogViewModel.SaleLogViewModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleLog, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = setting.conn;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@LogType", logType);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                master = new RpSaleLogViewModel.MasterSaleLogViewModel();
                master.LogID = Convert.ToInt32(reader["LogID"]);
                if (lstMasterSaleLog.Where(m => m.LogID == master.LogID).Count() > 0)
                {
                    item = new RpSaleLogViewModel.SaleLogViewModel();
                    item.Code = Convert.ToString(reader["Code"]);
                    item.ProductName = Convert.ToString(reader["ProductName"]);
                    item.Quantity = Convert.ToInt32(reader["Quantity"]);
                    if (logType == 2) item.OrginalQuantity = Convert.ToInt32(reader["OriginalQuantity"]);
                    item.UnitID = Convert.ToInt32(reader["UnitID"]);
                    item.UnitKeyword = Convert.ToString(reader["Keyword"]);
                    item.Amount = Convert.ToInt32(reader["Amount"]);
                    item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                    foreach (var SaleLog in lstMasterSaleLog.Where(m => m.LogID == master.LogID))
                    {
                        SaleLog.lstSaleLog.Add(item);
                    }
                }
                else
                {
                    master.SaleDate = Convert.ToDateTime(reader["UpdatedDateTime"]);
                    master.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                    master.EntryUserName = Convert.ToString(reader["EntryUserName"]);
                    master.UpdatedUserName = Convert.ToString(reader["UpdatedUserName"]);
                    item = new RpSaleLogViewModel.SaleLogViewModel();
                    item.Code = Convert.ToString(reader["Code"]);
                    item.ProductName = Convert.ToString(reader["ProductName"]);
                    item.Quantity = Convert.ToInt32(reader["Quantity"]);
                    if (logType == 2) item.OrginalQuantity = Convert.ToInt32(reader["OriginalQuantity"]);
                    item.UnitID = Convert.ToInt32(reader["UnitID"]);
                    item.UnitKeyword = Convert.ToString(reader["Keyword"]);
                    item.Amount = Convert.ToInt32(reader["Amount"]);
                    item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                    lstSaleLog = new List<RpSaleLogViewModel.SaleLogViewModel>();
                    lstSaleLog.Add(item);
                    master.lstSaleLog = lstSaleLog;
                    lstMasterSaleLog.Add(master);                    
                }
                
            }
            reader.Close();
            setting.conn.Close();
            return lstMasterSaleLog;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using Inventory.ViewModels;
using System.Data;
using System.Data.SqlClient;
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class RpAdjustmentVoucherController : MyController
    {
        AppSetting setting = new AppSetting();
        RpAdjustmentVoucherViewModel adjustmentVoucherViewModel = new RpAdjustmentVoucherViewModel();
        [SessionTimeoutAttribute]
        public ActionResult AdjustmentVoucherReportFilter()
        {
            return View();
        }

        [SessionTimeoutAttribute]
        public ActionResult AdjustmentVoucherReport(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            try
            {
                adjustmentVoucherViewModel.FromDate = fromDate;
                adjustmentVoucherViewModel.ToDate = toDate;
                adjustmentVoucherViewModel.lstMasterAdjustment = GetRptMasterAdjustment(fromDate, toDate, selectedLocationId);
                adjustmentVoucherViewModel.lstTranAdjustment = GetRptTranAdjustment(fromDate, toDate, selectedLocationId);
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }           
            return View(adjustmentVoucherViewModel);
        }
        public List<RpAdjustmentVoucherViewModel.MasterAdjustmentView> GetRptMasterAdjustment(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            List<RpAdjustmentVoucherViewModel.MasterAdjustmentView> lstMasterAdjustment = new List<RpAdjustmentVoucherViewModel.MasterAdjustmentView>();
            RpAdjustmentVoucherViewModel.MasterAdjustmentView masterAdjustment = new RpAdjustmentVoucherViewModel.MasterAdjustmentView();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptMasterAdjustmentList, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                masterAdjustment = new RpAdjustmentVoucherViewModel.MasterAdjustmentView();
                masterAdjustment.AdjustmentID = Convert.ToInt32(reader["AdjustmentID"]);
                masterAdjustment.VoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                masterAdjustment.VoucherID = Convert.ToString(reader["VoucherID"]);
                masterAdjustment.UserName = Convert.ToString(reader["UserName"]);
                masterAdjustment.LocationName = Convert.ToString(reader["LocationName"]);
                masterAdjustment.Remark = Convert.ToString(reader["Remark"]);              
                lstMasterAdjustment.Add(masterAdjustment);
            }
            reader.Close();
            setting.conn.Close();
            return lstMasterAdjustment;
        }
        public List<RpAdjustmentVoucherViewModel.TranAdjustmentView>GetRptTranAdjustment(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            List<RpAdjustmentVoucherViewModel.TranAdjustmentView> lstTranAdjustment = new List<RpAdjustmentVoucherViewModel.TranAdjustmentView>();
            RpAdjustmentVoucherViewModel.TranAdjustmentView tranAdjustment = new RpAdjustmentVoucherViewModel.TranAdjustmentView();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptTranAdjustmentList, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tranAdjustment = new RpAdjustmentVoucherViewModel.TranAdjustmentView();
                tranAdjustment.AdjustmentID = Convert.ToInt32(reader["AdjustmentID"]);
                tranAdjustment.ProductName = Convert.ToString(reader["ProductName"]);
                tranAdjustment.Quantity = Convert.ToInt32(reader["Quantity"]);
                tranAdjustment.UnitName = Convert.ToString(reader["UnitKeyword"]);
                tranAdjustment.AdjustTypeName = Convert.ToString(reader["AdjustTypeName"]);
                lstTranAdjustment.Add(tranAdjustment);
            }
            reader.Close();
            setting.conn.Close();
            return lstTranAdjustment;
        }
    }
}
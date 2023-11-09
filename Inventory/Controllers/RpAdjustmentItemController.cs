using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.ViewModels;
using Inventory.Common;
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class RpAdjustmentItemController : MyController
    {
        AppSetting setting = new AppSetting();
        RpAdjustmentItemViewModel adjustmentItemViewModel = new RpAdjustmentItemViewModel();
        [SessionTimeoutAttribute]
        public ActionResult AdjustmentItemReportFilter()
        {
            return View();
        }
        [SessionTimeoutAttribute]
        public ActionResult AdjustmentItemReport(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            try
            {
                adjustmentItemViewModel.FromDate = fromDate;
                adjustmentItemViewModel.ToDate = toDate;
                adjustmentItemViewModel.lstAdjustment = GetAdjustmentItemReport(fromDate,toDate, selectedLocationId);
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(adjustmentItemViewModel);
        }
        public List<RpAdjustmentItemViewModel.AdjustmentTypeViewModel> GetAdjustmentItemReport(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            List<RpAdjustmentItemViewModel.AdjustmentTypeViewModel> lstAdjustmentType = new List<RpAdjustmentItemViewModel.AdjustmentTypeViewModel>();
            RpAdjustmentItemViewModel.AdjustmentTypeViewModel adjustType = new RpAdjustmentItemViewModel.AdjustmentTypeViewModel();
            List<RpAdjustmentItemViewModel.AdjustmentItemViewModel> lstAdjustmentItem = new List<RpAdjustmentItemViewModel.AdjustmentItemViewModel>();
            RpAdjustmentItemViewModel.AdjustmentItemViewModel adjustItem = new RpAdjustmentItemViewModel.AdjustmentItemViewModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptAdjustmentItem, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                adjustType = new RpAdjustmentItemViewModel.AdjustmentTypeViewModel();
                adjustType.AdjustTypeID = Convert.ToInt32(reader["AdjustTypeID"]);
                if (lstAdjustmentType.Where(m => m.AdjustTypeID == adjustType.AdjustTypeID).Count() > 0)
                {
                    adjustItem = new RpAdjustmentItemViewModel.AdjustmentItemViewModel();
                    adjustItem.Code = Convert.ToString(reader["Code"]);
                    adjustItem.ProductName = Convert.ToString(reader["ProductName"]);
                    adjustItem.UnitID = Convert.ToInt32(reader["UnitID"]);
                    adjustItem.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                    adjustItem.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                    adjustItem.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                    adjustItem.Quantity = Convert.ToInt32(reader["Quantity"]);
                    foreach(var type in lstAdjustmentType.Where(m => m.AdjustTypeID == adjustType.AdjustTypeID))
                    {
                        type.lstAdjustmentItem.Add(adjustItem);
                    }
                }
                else
                {
                    adjustType.AdjustTypeName = Convert.ToString(reader["AdjustTypeName"]);
                    adjustType.Sign = Convert.ToString(reader["Sign"]);
                    
                    adjustItem = new RpAdjustmentItemViewModel.AdjustmentItemViewModel();
                    adjustItem.Code = Convert.ToString(reader["Code"]);
                    adjustItem.ProductName = Convert.ToString(reader["ProductName"]);
                    adjustItem.UnitID = Convert.ToInt32(reader["UnitID"]);
                    adjustItem.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                    adjustItem.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                    adjustItem.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                    adjustItem.Quantity = Convert.ToInt32(reader["Quantity"]);
                    List<RpAdjustmentItemViewModel.AdjustmentItemViewModel> lst = new List<RpAdjustmentItemViewModel.AdjustmentItemViewModel>();
                    lst.Add(adjustItem);
                    adjustType.lstAdjustmentItem = lst;
                    lstAdjustmentType.Add(adjustType);                   
                }
            }
            reader.Close();
            setting.conn.Close();
            return lstAdjustmentType;
        }
    }
}
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
    public class RpSaleItemByStaffController : MyController
    {
        AppSetting setting = new AppSetting();
        RpSaleItemByStaffViewModel SaleItemByStaffViewModel = new RpSaleItemByStaffViewModel();

        public ActionResult SaleItemByStaffReport(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                SaleItemByStaffViewModel.lstRptSaleItemByStaff = GetSaleItemByStaffReport(FromDate, ToDate);
                SaleItemByStaffViewModel.FromDate = FromDate;
                SaleItemByStaffViewModel.ToDate = ToDate;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(SaleItemByStaffViewModel);
        }

        public List<RpSaleItemByStaffViewModel.StaffViewModel> GetSaleItemByStaffReport(DateTime fromDate, DateTime toDate)
        {
            List<RpSaleItemByStaffViewModel.StaffViewModel> lstSaleItemByStaff = new List<RpSaleItemByStaffViewModel.StaffViewModel>();
            RpSaleItemByStaffViewModel.StaffViewModel staffModel = new RpSaleItemByStaffViewModel.StaffViewModel();
            RpSaleItemByStaffViewModel.SaleItemViewModel itemModel = new RpSaleItemByStaffViewModel.SaleItemViewModel();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleItemByStaff, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                staffModel = new RpSaleItemByStaffViewModel.StaffViewModel();
                staffModel.StaffID = Convert.ToInt32(reader["StaffID"]);
                if (lstSaleItemByStaff.Where(m => m.StaffID == staffModel.StaffID).Count() > 0)
                {
                    itemModel = new RpSaleItemByStaffViewModel.SaleItemViewModel();
                    itemModel.Code = Convert.ToString(reader["Code"]);
                    itemModel.ProductName = Convert.ToString(reader["ProductName"]);
                    itemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                    itemModel.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                    itemModel.Discount = Convert.ToInt32(reader["Discount"]);
                    itemModel.Amount = Convert.ToInt32(reader["Amount"]);
                    itemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                    itemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                    foreach (var staff in lstSaleItemByStaff.Where(m => m.StaffID == staffModel.StaffID))
                    {
                        staff.lstSaleItem.Add(itemModel);
                    }
                }
                else
                {
                    staffModel.StaffName = Convert.ToString(reader["StaffName"]);
                    lstSaleItemByStaff.Add(staffModel);
                    itemModel = new RpSaleItemByStaffViewModel.SaleItemViewModel();
                    itemModel.Code = Convert.ToString(reader["Code"]);
                    itemModel.ProductName = Convert.ToString(reader["ProductName"]);
                    itemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                    itemModel.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                    itemModel.Discount = Convert.ToInt32(reader["Discount"]);
                    itemModel.Amount = Convert.ToInt32(reader["Amount"]);
                    itemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                    itemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                    foreach (var staff in lstSaleItemByStaff.Where(m => m.StaffID == staffModel.StaffID))
                    {
                        List<RpSaleItemByStaffViewModel.SaleItemViewModel> lstSaleItem = new List<RpSaleItemByStaffViewModel.SaleItemViewModel>();
                        lstSaleItem.Add(itemModel);
                        staff.lstSaleItem = lstSaleItem;
                    }
                }
            }
            reader.Close();
            setting.conn.Close();

            int totalQuantity = 0, totalAmount = 0, totalDis = 0;
            foreach (var staff in lstSaleItemByStaff)
            {
                totalQuantity += staff.lstSaleItem.Sum(m => m.Quantity);
                totalAmount += staff.lstSaleItem.Sum(m => m.Amount);
                totalDis += staff.lstSaleItem.Sum(m => m.Discount);
            }
            SaleItemByStaffViewModel.TotalQuantity = totalQuantity;
            SaleItemByStaffViewModel.TotalAmount = totalAmount;
            SaleItemByStaffViewModel.TotalDis = totalDis;
            return lstSaleItemByStaff;
        }
    }
}
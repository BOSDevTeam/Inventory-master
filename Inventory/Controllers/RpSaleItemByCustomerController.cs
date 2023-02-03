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
    public class RpSaleItemByCustomerController : MyController
    {
        AppSetting setting = new AppSetting();
        RpSaleItemByCustomerViewModel SaleItemByCustomerViewModel = new RpSaleItemByCustomerViewModel();

        [SessionTimeoutAttribute]
        public ActionResult SaleItemByCustomerReportFilter()
        {
            return View();
        }

        [SessionTimeoutAttribute]
        public ActionResult SaleItemByCustomerReport(DateTime FromDate,DateTime ToDate)
        {
            try
            {
                SaleItemByCustomerViewModel.lstRptSaleItemByCustomer = GetSaleItemByCustomerReport(FromDate, ToDate);
                SaleItemByCustomerViewModel.FromDate = FromDate;
                SaleItemByCustomerViewModel.ToDate = ToDate;
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }           
            return View(SaleItemByCustomerViewModel);
        }

        public List<RpSaleItemByCustomerViewModel.CustomerViewModel> GetSaleItemByCustomerReport(DateTime fromDate,DateTime toDate)
        {
            List<RpSaleItemByCustomerViewModel.CustomerViewModel> lstSaleItemByCustomer = new List<RpSaleItemByCustomerViewModel.CustomerViewModel>();
            RpSaleItemByCustomerViewModel.CustomerViewModel customerModel = new RpSaleItemByCustomerViewModel.CustomerViewModel();
            RpSaleItemByCustomerViewModel.SaleItemViewModel itemModel = new RpSaleItemByCustomerViewModel.SaleItemViewModel();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleItemByCustomer, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                customerModel = new RpSaleItemByCustomerViewModel.CustomerViewModel();
                customerModel.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                if (lstSaleItemByCustomer.Where(m => m.CustomerID == customerModel.CustomerID).Count() > 0)
                {
                    itemModel = new RpSaleItemByCustomerViewModel.SaleItemViewModel();
                    itemModel.Code = Convert.ToString(reader["Code"]);
                    itemModel.ProductName = Convert.ToString(reader["ProductName"]);
                    itemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                    itemModel.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                    itemModel.Discount = Convert.ToInt32(reader["Discount"]);
                    itemModel.Amount = Convert.ToInt32(reader["Amount"]);
                    itemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                    itemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                    foreach (var customer in lstSaleItemByCustomer.Where(m => m.CustomerID == customerModel.CustomerID))
                    {
                        customer.lstSaleItem.Add(itemModel);
                    }
                }
                else
                {
                    customerModel.CustomerName = Convert.ToString(reader["CustomerName"]);
                    lstSaleItemByCustomer.Add(customerModel);
                    itemModel = new RpSaleItemByCustomerViewModel.SaleItemViewModel();
                    itemModel.Code = Convert.ToString(reader["Code"]);
                    itemModel.ProductName = Convert.ToString(reader["ProductName"]);
                    itemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                    itemModel.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                    itemModel.Discount = Convert.ToInt32(reader["Discount"]);
                    itemModel.Amount = Convert.ToInt32(reader["Amount"]);
                    itemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                    itemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                    foreach (var customer in lstSaleItemByCustomer.Where(m => m.CustomerID == customerModel.CustomerID))
                    {
                        List<RpSaleItemByCustomerViewModel.SaleItemViewModel> lstSaleItem = new List<RpSaleItemByCustomerViewModel.SaleItemViewModel>();
                        lstSaleItem.Add(itemModel);
                        customer.lstSaleItem = lstSaleItem;
                    }
                }
            }
            reader.Close();
            setting.conn.Close();
                       
            int totalQuantity = 0,totalAmount=0,totalDis=0;
            foreach(var customer in lstSaleItemByCustomer)
            {
                totalQuantity += customer.lstSaleItem.Sum(m => m.Quantity);
                totalAmount += customer.lstSaleItem.Sum(m => m.Amount);
                totalDis += customer.lstSaleItem.Sum(m => m.Discount);
            }
            SaleItemByCustomerViewModel.TotalQuantity = totalQuantity;
            SaleItemByCustomerViewModel.TotalAmount = totalAmount;
            SaleItemByCustomerViewModel.TotalDis = totalDis;
            return lstSaleItemByCustomer;
        }
    }
}
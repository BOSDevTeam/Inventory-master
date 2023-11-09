using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Common;
using Inventory.ViewModels;
using Inventory.Filters;

namespace Inventory.Controllers
{   
    public class RpSaleAmountByCustomerController : MyController
    {
        AppSetting setting = new AppSetting();
        RpSaleAmountByCustomerViewModel saleAmountByCustomerViewModel = new RpSaleAmountByCustomerViewModel();

        [SessionTimeoutAttribute]
        public ActionResult SaleAmountByCustomerReportFilter()
        {
            return View();
        }

        [SessionTimeoutAttribute]
        public ActionResult SaleAmountByCustomerReport(DateTime FromDate,DateTime ToDate, int? selectedLocationId)
        {
            try
            {
                saleAmountByCustomerViewModel.lstRptSaleAmountByCustomer = GetSaleAmountByCustomerReport(FromDate, ToDate,selectedLocationId);
                saleAmountByCustomerViewModel.FromDate = FromDate;
                saleAmountByCustomerViewModel.ToDate = ToDate;
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }           
            return View(saleAmountByCustomerViewModel);
        }
        public List<RpSaleAmountByCustomerViewModel.CustomerViewModel> GetSaleAmountByCustomerReport(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            List<RpSaleAmountByCustomerViewModel.CustomerViewModel> lstSaleAmountByCustomer = new List<RpSaleAmountByCustomerViewModel.CustomerViewModel>();
            RpSaleAmountByCustomerViewModel.CustomerViewModel customerModel = new RpSaleAmountByCustomerViewModel.CustomerViewModel();
            RpSaleAmountByCustomerViewModel.SaleItemViewModel saleModel = new RpSaleAmountByCustomerViewModel.SaleItemViewModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleAmountByCustomer, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                customerModel = new RpSaleAmountByCustomerViewModel.CustomerViewModel();
                customerModel.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                if (lstSaleAmountByCustomer.Where(m => m.CustomerID == customerModel.CustomerID).Count() > 0)
                {
                    saleModel = new RpSaleAmountByCustomerViewModel.SaleItemViewModel();
                    saleModel.SaleDate = Convert.ToDateTime(reader["SaleDateTime"]);
                    saleModel.SaleVoucherNO = Convert.ToString(reader["UserVoucherNo"]);
                    saleModel.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                    //saleModel.PaymentName = Convert.ToString(reader["PaymentName"]);
                    saleModel.SubTotal = Convert.ToInt32(reader["Subtotal"]);
                    saleModel.TaxAmount = Convert.ToInt32(reader["TaxAmt"]);
                    saleModel.ChargesAmount = Convert.ToInt32(reader["ChargesAmt"]);
                    saleModel.VouDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                    saleModel.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);                   
                    if (Convert.ToInt32(reader["PayMethodID"]) == 3) saleModel.PayPercentAmount = Convert.ToInt32(reader["MultiPayPercentAmt"]);                    
                    else saleModel.PayPercentAmount = Convert.ToInt32(reader["PayPercentAmt"]);                   
                    saleModel.VouFOC = Convert.ToInt32(reader["VoucherFOC"]);
                    if (saleModel.VouFOC != 0) saleModel.GrandTotal = 0;
                    else saleModel.GrandTotal = Convert.ToInt32(reader["Grandtotal"]);                       
                    foreach (var customer in lstSaleAmountByCustomer.Where(m => m.CustomerID == customerModel.CustomerID))
                    {
                        customer.lstSaleItem.Add(saleModel);
                    }
                }
                else
                {
                    customerModel.CustomerName = Convert.ToString(reader["CustomerName"]);
                    lstSaleAmountByCustomer.Add(customerModel);
                    saleModel = new RpSaleAmountByCustomerViewModel.SaleItemViewModel();
                    saleModel.SaleDate = Convert.ToDateTime(reader["SaleDateTime"]);
                    saleModel.SaleVoucherNO = Convert.ToString(reader["UserVoucherNo"]);
                    saleModel.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                    //saleModel.PaymentName = Convert.ToString(reader["PaymentName"]);
                    saleModel.SubTotal = Convert.ToInt32(reader["Subtotal"]);
                    saleModel.TaxAmount = Convert.ToInt32(reader["TaxAmt"]);
                    saleModel.ChargesAmount = Convert.ToInt32(reader["ChargesAmt"]);
                    saleModel.VouDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                    saleModel.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                    if (Convert.ToInt32(reader["PayMethodID"]) == 3) saleModel.PayPercentAmount = Convert.ToInt32(reader["MultiPayPercentAmt"]);
                    else saleModel.PayPercentAmount = Convert.ToInt32(reader["PayPercentAmt"]);
                    saleModel.VouFOC = Convert.ToInt32(reader["VoucherFOC"]);
                    if (saleModel.VouFOC != 0) saleModel.GrandTotal = 0;
                    else saleModel.GrandTotal = Convert.ToInt32(reader["Grandtotal"]);                       
                    foreach (var customer in lstSaleAmountByCustomer.Where(m => m.CustomerID == customerModel.CustomerID))
                    {
                        List<RpSaleAmountByCustomerViewModel.SaleItemViewModel> lst = new List<RpSaleAmountByCustomerViewModel.SaleItemViewModel>();
                        lst.Add(saleModel);
                        customer.lstSaleItem = lst;
                    }
                }
            }
            reader.Close();
            setting.conn.Close();

            int totalCash = 0, totalCredit = 0, totalTax = 0, totalCharges = 0, totalVouDis = 0, totalAdvPay = 0, totalBank = 0, totalVouFoc = 0, totalAmount = 0;
            foreach (var customer in lstSaleAmountByCustomer)
            {
                totalCash += customer.lstSaleItem.Where(m => m.PaymentID == 1).Sum(m => m.SubTotal);
                totalCredit += customer.lstSaleItem.Where(m => m.PaymentID == 2).Sum(m => m.SubTotal);
                totalTax += customer.lstSaleItem.Sum(m => m.TaxAmount);
                totalCharges += customer.lstSaleItem.Sum(m => m.ChargesAmount);
                totalVouDis += customer.lstSaleItem.Sum(m => m.VouDiscount);
                totalAdvPay += customer.lstSaleItem.Sum(m => m.AdvancedPay);
                totalBank += customer.lstSaleItem.Sum(m => m.PayPercentAmount);
                totalVouFoc += customer.lstSaleItem.Sum(m => m.VouFOC);
                totalAmount += customer.lstSaleItem.Sum(m => m.GrandTotal);
            }
            saleAmountByCustomerViewModel.TotalCash = totalCash;
            saleAmountByCustomerViewModel.TotalCredit = totalCredit;
            saleAmountByCustomerViewModel.TotalTax = totalTax;
            saleAmountByCustomerViewModel.TotalCharges = totalCharges;
            saleAmountByCustomerViewModel.TotalVouDis = totalVouDis;
            saleAmountByCustomerViewModel.TotalAdvancedPay = totalAdvPay;
            saleAmountByCustomerViewModel.TotalPayPercent = totalBank;
            saleAmountByCustomerViewModel.TotalVouFOC = totalVouFoc;
            saleAmountByCustomerViewModel.TotalAmount = totalAmount;           
            return lstSaleAmountByCustomer;
        }
    }
}
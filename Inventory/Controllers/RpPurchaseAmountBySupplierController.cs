using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using Inventory.ViewModels;
using Inventory.Filters;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class RpPurchaseAmountBySupplierController : MyController
    {
        AppSetting setting = new AppSetting();
        RpPurchaseAmountBySupplierViewModel purchaseAmountBySupplierViewModel = new RpPurchaseAmountBySupplierViewModel();

        // GET: RpPurchaseAmountBySupplier
        [SessionTimeoutAttribute]
        public ActionResult PurchaseAmountBySupplierReportFilter()
        {
            return View();
        }

        [SessionTimeoutAttribute]
        public ActionResult PurchaseAmountBySupplierReport(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            try
            {
                purchaseAmountBySupplierViewModel.lstRptPurchaseAmountBySupplier = GetPurchaseAmountBySupplierReport(fromDate, toDate,selectedLocationId);
                purchaseAmountBySupplierViewModel.FromDate = fromDate;
                purchaseAmountBySupplierViewModel.ToDate = toDate;
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(purchaseAmountBySupplierViewModel);
        }

        public List<RpPurchaseAmountBySupplierViewModel.SupplierViewModel> GetPurchaseAmountBySupplierReport(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            List<RpPurchaseAmountBySupplierViewModel.SupplierViewModel> lstPurchaseAmountBySupplier = new List<RpPurchaseAmountBySupplierViewModel.SupplierViewModel>();
            RpPurchaseAmountBySupplierViewModel.SupplierViewModel supplierModel = new RpPurchaseAmountBySupplierViewModel.SupplierViewModel();
            RpPurchaseAmountBySupplierViewModel.PurchaseItemViewModel purchaseModel = new RpPurchaseAmountBySupplierViewModel.PurchaseItemViewModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptPurchaseAmountBySupplier, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                supplierModel = new RpPurchaseAmountBySupplierViewModel.SupplierViewModel();
                supplierModel.SupplierID = Convert.ToInt32(reader["SupplierID"]);
                if (lstPurchaseAmountBySupplier.Where(m => m.SupplierID == supplierModel.SupplierID).Count() > 0)
                {
                    purchaseModel = new RpPurchaseAmountBySupplierViewModel.PurchaseItemViewModel();
                    purchaseModel.PurDate = Convert.ToDateTime(reader["PurDateTime"]);
                    purchaseModel.PurVoucherNO = Convert.ToString(reader["UserVoucherNo"]);
                    purchaseModel.PaymentID = Convert.ToInt32(reader["PaymentID"]);                   
                    purchaseModel.SubTotal = Convert.ToInt32(reader["Subtotal"]);
                    purchaseModel.TaxAmount = Convert.ToInt32(reader["TaxAmt"]);
                    purchaseModel.ChargesAmount = Convert.ToInt32(reader["ChargesAmt"]);
                    purchaseModel.VouDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                    purchaseModel.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                    purchaseModel.PayPercentAmount = Convert.ToInt32(reader["PayPercentAmt"]);
                    purchaseModel.VouFOC = Convert.ToInt32(reader["VoucherFOC"]);
                    if (purchaseModel.VouFOC > 0) purchaseModel.GrandTotal = 0;
                    else purchaseModel.GrandTotal = Convert.ToInt32(reader["Grandtotal"]);
                    foreach (var supplier in lstPurchaseAmountBySupplier.Where(m => m.SupplierID == supplierModel.SupplierID))
                    {
                        supplier.lstPurchaseItem.Add(purchaseModel);
                    }
                }
                else
                {
                    supplierModel.SupplierName = Convert.ToString(reader["SupplierName"]);
                    lstPurchaseAmountBySupplier.Add(supplierModel);
                    purchaseModel = new RpPurchaseAmountBySupplierViewModel.PurchaseItemViewModel();
                    purchaseModel.PurDate = Convert.ToDateTime(reader["PurDateTime"]);
                    purchaseModel.PurVoucherNO = Convert.ToString(reader["UserVoucherNo"]);
                    purchaseModel.PaymentID = Convert.ToInt32(reader["PaymentID"]);                 
                    purchaseModel.SubTotal = Convert.ToInt32(reader["Subtotal"]);
                    purchaseModel.TaxAmount = Convert.ToInt32(reader["TaxAmt"]);
                    purchaseModel.ChargesAmount = Convert.ToInt32(reader["ChargesAmt"]);
                    purchaseModel.VouDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                    purchaseModel.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                    purchaseModel.PayPercentAmount = Convert.ToInt32(reader["PayPercentAmt"]);
                    purchaseModel.VouFOC = Convert.ToInt32(reader["VoucherFOC"]);
                    if (purchaseModel.VouFOC > 0) purchaseModel.GrandTotal = 0;
                    else purchaseModel.GrandTotal = Convert.ToInt32(reader["Grandtotal"]);
                    foreach (var supplier in lstPurchaseAmountBySupplier.Where(m => m.SupplierID == supplierModel.SupplierID))
                    {
                        List<RpPurchaseAmountBySupplierViewModel.PurchaseItemViewModel> lst = new List<RpPurchaseAmountBySupplierViewModel.PurchaseItemViewModel>();
                        lst.Add(purchaseModel);
                        supplierModel.lstPurchaseItem = lst;
                    }
                }
            }
            reader.Close();
            setting.conn.Close();
            int totalCash = 0, totalCredit = 0, totalTax = 0, totalCharges = 0, totalVouDis = 0, totalAdvPay = 0, totalBank = 0, totalVouFoc = 0, totalAmount = 0;
            foreach (var supplier in lstPurchaseAmountBySupplier)
            {
                totalCash += supplier.lstPurchaseItem.Where(m => m.PaymentID == 1).Sum(m => m.SubTotal);
                totalCredit += supplier.lstPurchaseItem.Where(m => m.PaymentID == 2).Sum(m => m.SubTotal);
                totalTax += supplier.lstPurchaseItem.Sum(m => m.TaxAmount);
                totalCharges += supplier.lstPurchaseItem.Sum(m => m.ChargesAmount);
                totalVouDis += supplier.lstPurchaseItem.Sum(m => m.VouDiscount);
                totalAdvPay += supplier.lstPurchaseItem.Sum(m => m.AdvancedPay);
                totalBank += supplier.lstPurchaseItem.Sum(m => m.PayPercentAmount);
                totalVouFoc += supplier.lstPurchaseItem.Sum(m => m.VouFOC);
                totalAmount += supplier.lstPurchaseItem.Sum(m => m.GrandTotal);
            }
            purchaseAmountBySupplierViewModel.TotalCash = totalCash;
            purchaseAmountBySupplierViewModel.TotalCredit = totalCredit;
            purchaseAmountBySupplierViewModel.TotalTax = totalTax;
            purchaseAmountBySupplierViewModel.TotalCharges = totalCharges;
            purchaseAmountBySupplierViewModel.TotalVouDis = totalVouDis;
            purchaseAmountBySupplierViewModel.TotalAdvancedPay = totalAdvPay;
            purchaseAmountBySupplierViewModel.TotalPayPercent = totalBank;
            purchaseAmountBySupplierViewModel.TotalVouFOC = totalVouFoc;
            purchaseAmountBySupplierViewModel.TotalAmount = totalAmount;
            return lstPurchaseAmountBySupplier;
        }
    }
}
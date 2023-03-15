using Inventory.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;

namespace Inventory.Controllers
{
    public class HomeController : MyController
    {
        AppSetting setting = new AppSetting();
        HomeViewModel homeViewModel = new HomeViewModel();

        public ActionResult Dashboard()
        {
            selectDashboardData(setting.getLocalDate());
            ViewBag.Month = setting.getCurrentMonthName();
            homeViewModel.lstTopSaleProduct = selectTopSaleProduct(setting.getCurrentMonth());
            homeViewModel.lstTodaySaleQuantity = selectTodaySaleQuantity(setting.getLocalDate());
            selectTodayAmount(setting.getLocalDate());
            if(ViewBag.Banking != 0)
            {
                homeViewModel.lstBankingAmount = selectTodayBankingAmount(setting.getLocalDate());
            }
            homeViewModel.lstCustomerOutstandingOverDue = selectCustomerOutstandingOverDue(setting.getLocalDate());
            ViewData["LstCustomerOutstandingPayment"] = selectCustomerOutstandingPayment(setting.getLocalDate());
            return View(homeViewModel);
        }

        [HttpGet]
        public JsonResult CustomerOutstandingPaymentDateAction(string date)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<HomeViewModel.CustomerOutstandingPayment> list = new List<HomeViewModel.CustomerOutstandingPayment>();

            try
            {
                list = selectCustomerOutstandingPayment(Convert.ToDateTime(date));
                resultDefaultData.IsRequestSuccess = true;
            }
            catch(Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }
            
            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                List = list
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CustomerOutstandingOverDueDetailAction(int customerId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<HomeViewModel.CustomerOutstandingOverDueDetail> list = new List<HomeViewModel.CustomerOutstandingOverDueDetail>();

            try
            {
                list = selectCustomerOutstandingOverDueDetail(customerId,setting.getLocalDate());
                resultDefaultData.IsRequestSuccess = true;
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                List = list
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public void selectDashboardData(DateTime todayDate)
        {
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetDashboardData, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TodayDate", todayDate);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                ViewBag.TodaySale = Convert.ToInt32(reader["TodaySale"]);
                ViewBag.TodayClientOrder = Convert.ToInt32(reader["TodayClientOrder"]);
                ViewBag.TodaySaleProduct = Convert.ToInt32(reader["TodaySaleProduct"]);
                ViewBag.TodayCreditSale = Convert.ToInt32(reader["TodayCreditSale"]);
                ViewBag.Customer = Convert.ToInt32(reader["Customer"]);
                ViewBag.Supplier = Convert.ToInt32(reader["Supplier"]);
                ViewBag.ClientSalePerson = Convert.ToInt32(reader["ClientSalePerson"]);
                ViewBag.ClientEndUser = Convert.ToInt32(reader["ClientEndUser"]);
            }
            reader.Close();
            setting.conn.Close();
        }

        public void selectTodayAmount(DateTime todayDate)
        {
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTodayAmount, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TodayDate", todayDate);
            cmd.Parameters.AddWithValue("@ARAccountCode", AppConstants.ARAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                ViewBag.Summary = Convert.ToInt32(reader["Summary"]);
                ViewBag.Cash = Convert.ToInt32(reader["Cash"]);
                ViewBag.Sale = Convert.ToInt32(reader["Sale"]);
                ViewBag.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                double amount = Convert.ToDouble(reader["CustomerOutstandingPayment"]);
                if (amount.ToString().StartsWith("-"))
                    ViewBag.CustomerOutstandingPayment = Convert.ToDouble(amount.ToString().Substring(1, amount.ToString().Length - 1));
                else ViewBag.CustomerOutstandingPayment = amount;

                ViewBag.Credit = Convert.ToInt32(reader["Credit"]);
                ViewBag.CashInHand = Convert.ToInt32(reader["CashInHand"]);
                ViewBag.Banking = Convert.ToInt32(reader["Banking"]);
            }
            reader.Close();
            setting.conn.Close();
        }

        public List<HomeViewModel.TodaySaleQuantityView> selectTodaySaleQuantity(DateTime todayDate)
        {
            List<HomeViewModel.TodaySaleQuantityView> list = new List<HomeViewModel.TodaySaleQuantityView>();
            HomeViewModel.TodaySaleQuantityView item;
            string discount = "";

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTodaySaleQuantity, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TodayDate", todayDate);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new HomeViewModel.TodaySaleQuantityView();
                if (Convert.ToBoolean(reader["IsDiscount"]))
                {
                    discount = Convert.ToString(reader["Name"]);
                    item.Name= "Discount ("+ discount +"%)";
                }else item.Name = Convert.ToString(reader["Name"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
               
                list.Add(item);
            }
            list[0].IsSummary = true;
            reader.Close();
            setting.conn.Close();
            return list;
        }

        public List<HomeViewModel.TopSaleProductView> selectTopSaleProduct(int month)
        {
            List<HomeViewModel.TopSaleProductView> list = new List<HomeViewModel.TopSaleProductView>();
            HomeViewModel.TopSaleProductView item;
            int number = 0;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTopSaleProductByMonth, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Month", month);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new HomeViewModel.TopSaleProductView();
                number++;
                item.Number = number;
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }

        public List<HomeViewModel.BankingAmountView> selectTodayBankingAmount(DateTime todayDate)
        {
            List<HomeViewModel.BankingAmountView> list = new List<HomeViewModel.BankingAmountView>();
            HomeViewModel.BankingAmountView item;         

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTodayAmountByBanking, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TodayDate", todayDate);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new HomeViewModel.BankingAmountView();                         
                item.BankPaymentName = Convert.ToString(reader["BankPaymentName"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }

        public List<HomeViewModel.CustomerOutstandingOverDue> selectCustomerOutstandingOverDue(DateTime todayDate)
        {
            List<HomeViewModel.CustomerOutstandingOverDue> list = new List<HomeViewModel.CustomerOutstandingOverDue>();
            HomeViewModel.CustomerOutstandingOverDue item;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetCustomerOutstandingOverDue, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TodayDate", todayDate);
            cmd.Parameters.AddWithValue("@SaleAccountCode", AppConstants.SaleAccountCode);
            cmd.Parameters.AddWithValue("@ARAccountCode", AppConstants.ARAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new HomeViewModel.CustomerOutstandingOverDue();
                item.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.TownshipName = Convert.ToString(reader["TownshipName"]);
                item.TotalDueVoucher = Convert.ToInt32(reader["TotalDueVoucher"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }

        public List<HomeViewModel.CustomerOutstandingPayment> selectCustomerOutstandingPayment(DateTime todayDate)
        {
            List<HomeViewModel.CustomerOutstandingPayment> list = new List<HomeViewModel.CustomerOutstandingPayment>();
            HomeViewModel.CustomerOutstandingPayment item;
            double amount = 0;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetCustomerOutstandingPayment, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TodayDate", todayDate);
            cmd.Parameters.AddWithValue("@ARAccountCode", AppConstants.ARAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new HomeViewModel.CustomerOutstandingPayment();
                item.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.TownshipName = Convert.ToString(reader["TownshipName"]);
                amount= Convert.ToDouble(reader["Amount"]);
                if (amount.ToString().StartsWith("-"))
                {
                    item.Amount = Convert.ToDouble(amount.ToString().Substring(1, amount.ToString().Length-1));
                }
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }

        public List<HomeViewModel.CustomerOutstandingOverDueDetail> selectCustomerOutstandingOverDueDetail(int customerId,DateTime todayDate)
        {
            List<HomeViewModel.CustomerOutstandingOverDueDetail> list = new List<HomeViewModel.CustomerOutstandingOverDueDetail>();
            HomeViewModel.CustomerOutstandingOverDueDetail item;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetCustomerOutstandingOverDueDetail, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            cmd.Parameters.AddWithValue("@TodayDate", todayDate);
            cmd.Parameters.AddWithValue("@SaleAccountCode", AppConstants.SaleAccountCode);
            cmd.Parameters.AddWithValue("@ARAccountCode", AppConstants.ARAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new HomeViewModel.CustomerOutstandingOverDueDetail();
                item.DateTime = Convert.ToString(reader["DateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.LimitedDayName = Convert.ToString(reader["LimitedDayName"]);
                item.OverDueDay = Convert.ToInt32(reader["OverDueDay"]);
                item.Amount = Convert.ToDouble(reader["Amount"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }
    }
}
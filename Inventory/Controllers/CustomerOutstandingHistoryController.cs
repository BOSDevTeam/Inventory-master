using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;
using Inventory.Filters;
using Inventory.Common;
using Inventory.Models;

namespace Inventory.Controllers
{
    public class CustomerOutstandingHistoryController : MyController
    {
        AppData appData = new AppData();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();
        
        #region OutstandingListAction

        [HttpGet]
        public JsonResult PaymentAction(int customerId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<CustomerOutstandingHistoryViewModel.DetailViewModel> lstCustomerOutstandingPayment = new List<CustomerOutstandingHistoryViewModel.DetailViewModel>();
            try
            {
                Session["HCustomerOutstandingPaymentList"] = null;
                lstCustomerOutstandingPayment = selectTranCustomerOutstanding(customerId);
                resultDefaultData.IsRequestSuccess = true;
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }
            var jsonResult = new
            {
                LstCustomerOutstandingPayment = lstCustomerOutstandingPayment,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }                    

        #endregion

        #region PaymentAction

        [HttpGet]
        public JsonResult AddPaymentAction(int ledgerId, int payment)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["HCustomerOutstandingTranList"] != null)
            {
                try
                {
                    List<CustomerOutstandingHistoryViewModel.DetailViewModel> lstTranCustomerOutstanding = Session["HCustomerOutstandingTranList"] as List<CustomerOutstandingHistoryViewModel.DetailViewModel>;
                    CustomerOutstandingHistoryViewModel.DetailViewModel data = lstTranCustomerOutstanding.Where(x => x.LedgerID == ledgerId).SingleOrDefault();
                    List<CustomerOutstandingHistoryViewModel.DetailViewModel> lstPayment = new List<CustomerOutstandingHistoryViewModel.DetailViewModel>();
                    if (Session["HCustomerOutstandingPaymentList"] != null)
                    {
                        lstPayment = Session["HCustomerOutstandingPaymentList"] as List<CustomerOutstandingHistoryViewModel.DetailViewModel>;
                    }
                    data.Payment = payment;

                    int index = lstPayment.FindIndex(x => x.LedgerID == ledgerId);
                    if (index != -1)
                        lstPayment[index] = data;
                    else lstPayment.Add(data);

                    Session["HCustomerOutstandingPaymentList"] = lstPayment;
                    resultDefaultData.IsRequestSuccess = true;                    
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangedPayDateAction(int ledgerId, string payDate)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["HCustomerOutstandingTranList"] != null)
            {
                try
                {
                    List<CustomerOutstandingHistoryViewModel.DetailViewModel> lstTranCustomerOutstanding = Session["HCustomerOutstandingTranList"] as List<CustomerOutstandingHistoryViewModel.DetailViewModel>;
                    CustomerOutstandingHistoryViewModel.DetailViewModel data = lstTranCustomerOutstanding.Where(x => x.LedgerID == ledgerId).SingleOrDefault();
                    List<CustomerOutstandingHistoryViewModel.DetailViewModel> lstPayment = new List<CustomerOutstandingHistoryViewModel.DetailViewModel>();
                    if (Session["HCustomerOutstandingPaymentList"] != null)
                    {
                        lstPayment = Session["HCustomerOutstandingPaymentList"] as List<CustomerOutstandingHistoryViewModel.DetailViewModel>;
                    }
                    data.PayDate = payDate;

                    int index = lstPayment.FindIndex(x => x.LedgerID == ledgerId);
                    if (index != -1)
                        lstPayment[index] = data;
                    else lstPayment.Add(data);

                    Session["HCustomerOutstandingPaymentList"] = lstPayment;
                    resultDefaultData.IsRequestSuccess = true;
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaveAction(int customerId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["HCustomerOutstandingPaymentList"] != null)
            {
                try
                {
                    List<CustomerOutstandingHistoryViewModel.DetailViewModel> lstPayment = Session["HCustomerOutstandingPaymentList"] as List<CustomerOutstandingHistoryViewModel.DetailViewModel>;

                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("LedgerID", typeof(int)));
                    dt.Columns.Add(new DataColumn("PayDate", typeof(DateTime)));
                    dt.Columns.Add(new DataColumn("Payment", typeof(int)));
                    for (int i = 0; i < lstPayment.Count; i++)
                    {
                        dt.Rows.Add(lstPayment[i].LedgerID, Convert.ToDateTime(lstPayment[i].PayDate), lstPayment[i].Payment);
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateCustomerOutstandingPayment, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;               
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Connection = setting.conn;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();

                    Session["HCustomerOutstandingPaymentList"] = null;
                    resultDefaultData.IsRequestSuccess = true;
                    resultDefaultData.Message = AppConstants.Message.UpdateSuccess;
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                resultDefaultData.Message = AppConstants.Message.FillOutstandingPayment;
            }

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        #endregion
        
        private List<CustomerOutstandingHistoryViewModel.DetailViewModel> selectTranCustomerOutstanding(int customerId)
        {
            List<CustomerOutstandingHistoryViewModel.DetailViewModel> list = new List<CustomerOutstandingHistoryViewModel.DetailViewModel>();
            CustomerOutstandingHistoryViewModel.DetailViewModel item = new CustomerOutstandingHistoryViewModel.DetailViewModel();
            int payment = 0;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranCustomerOutstandingHistory, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerID", customerId);          
            cmd.Parameters.AddWithValue("@ARAccountCode", AppConstants.ARAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new CustomerOutstandingHistoryViewModel.DetailViewModel();
                item.Date = Convert.ToString(reader["Date"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.PayDate = setting.convertDateToString(Convert.ToDateTime(reader["PayDate"]));
                item.PayType = Convert.ToInt16(reader["PayType"]);
                payment = Convert.ToInt32(reader["Payment"]);
                if (payment.ToString().StartsWith("-"))
                    payment = Convert.ToInt32(payment.ToString().Remove(0, 1));
                item.Payment = payment;
                item.LedgerID = Convert.ToInt32(reader["LedgerID"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            Session["HCustomerOutstandingTranList"] = list;

            return list;
        }
    }
}
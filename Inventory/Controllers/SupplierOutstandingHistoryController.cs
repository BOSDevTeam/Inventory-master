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
    public class SupplierOutstandingHistoryController : MyController
    {
        AppData appData = new AppData();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();

        #region OutstandingListAction

        [HttpGet]
        public JsonResult PaymentAction(int supplierId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SupplierOutstandingHistoryViewModel.DetailViewModel> lstSupplierOutstandingPayment = new List<SupplierOutstandingHistoryViewModel.DetailViewModel>();
            try
            {
                Session["HSupplierOutstandingPaymentList"] = null;
                lstSupplierOutstandingPayment = selectTranSupplierOutstanding(supplierId,false);
                resultDefaultData.IsRequestSuccess = true;
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }
            var jsonResult = new
            {
                LstSupplierOutstandingPayment = lstSupplierOutstandingPayment,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(int supplierId, DateTime fromDate, DateTime toDate)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SupplierOutstandingHistoryViewModel.DetailViewModel> lstSupplierOutstandingPayment = new List<SupplierOutstandingHistoryViewModel.DetailViewModel>();
            try
            {
                Session["HSupplierOutstandingPaymentList"] = null;
                lstSupplierOutstandingPayment = selectTranSupplierOutstanding(supplierId,true,fromDate,toDate);
                resultDefaultData.IsRequestSuccess = true;
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }
            var jsonResult = new
            {
                LstSupplierOutstandingPayment = lstSupplierOutstandingPayment,
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
            if (Session["HSupplierOutstandingTranList"] != null)
            {
                try
                {
                    List<SupplierOutstandingHistoryViewModel.DetailViewModel> lstTranSupplierOutstanding = Session["HSupplierOutstandingTranList"] as List<SupplierOutstandingHistoryViewModel.DetailViewModel>;
                    SupplierOutstandingHistoryViewModel.DetailViewModel data = lstTranSupplierOutstanding.Where(x => x.LedgerID == ledgerId).SingleOrDefault();
                    List<SupplierOutstandingHistoryViewModel.DetailViewModel> lstPayment = new List<SupplierOutstandingHistoryViewModel.DetailViewModel>();
                    if (Session["HSupplierOutstandingPaymentList"] != null)
                    {
                        lstPayment = Session["HSupplierOutstandingPaymentList"] as List<SupplierOutstandingHistoryViewModel.DetailViewModel>;
                    }
                    data.Payment = payment;

                    int index = lstPayment.FindIndex(x => x.LedgerID == ledgerId);
                    if (index != -1)
                        lstPayment[index] = data;
                    else lstPayment.Add(data);

                    Session["HSupplierOutstandingPaymentList"] = lstPayment;
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
            if (Session["HSupplierOutstandingTranList"] != null)
            {
                try
                {
                    List<SupplierOutstandingHistoryViewModel.DetailViewModel> lstTranSupplierOutstanding = Session["HSupplierOutstandingTranList"] as List<SupplierOutstandingHistoryViewModel.DetailViewModel>;
                    SupplierOutstandingHistoryViewModel.DetailViewModel data = lstTranSupplierOutstanding.Where(x => x.LedgerID == ledgerId).SingleOrDefault();
                    List<SupplierOutstandingHistoryViewModel.DetailViewModel> lstPayment = new List<SupplierOutstandingHistoryViewModel.DetailViewModel>();
                    if (Session["HSupplierOutstandingPaymentList"] != null)
                    {
                        lstPayment = Session["HSupplierOutstandingPaymentList"] as List<SupplierOutstandingHistoryViewModel.DetailViewModel>;
                    }
                    data.PayDate = payDate;

                    int index = lstPayment.FindIndex(x => x.LedgerID == ledgerId);
                    if (index != -1)
                        lstPayment[index] = data;
                    else lstPayment.Add(data);

                    Session["HSupplierOutstandingPaymentList"] = lstPayment;
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
        public JsonResult SaveAction(int supplierId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["HSupplierOutstandingPaymentList"] != null)
            {
                try
                {
                    List<SupplierOutstandingHistoryViewModel.DetailViewModel> lstPayment = Session["HSupplierOutstandingPaymentList"] as List<SupplierOutstandingHistoryViewModel.DetailViewModel>;

                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("LedgerID", typeof(int)));
                    dt.Columns.Add(new DataColumn("PayDate", typeof(DateTime)));
                    dt.Columns.Add(new DataColumn("Payment", typeof(int)));
                    for (int i = 0; i < lstPayment.Count; i++)
                    {
                        dt.Rows.Add(lstPayment[i].LedgerID, Convert.ToDateTime(lstPayment[i].PayDate), lstPayment[i].Payment);
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateSupplierOutstandingPayment, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Connection = setting.conn;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();

                    Session["HSupplierOutstandingPaymentList"] = null;
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

        private List<SupplierOutstandingHistoryViewModel.DetailViewModel> selectTranSupplierOutstanding(int supplierId, bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate)
        {
            List<SupplierOutstandingHistoryViewModel.DetailViewModel> list = new List<SupplierOutstandingHistoryViewModel.DetailViewModel>();
            SupplierOutstandingHistoryViewModel.DetailViewModel item = new SupplierOutstandingHistoryViewModel.DetailViewModel();
            int payment = 0;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranSupplierOutstandingHistory, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierID", supplierId);
            cmd.Parameters.AddWithValue("@APAccountCode", AppConstants.APAccountCode);
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", setting.getLocalDate());
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
            }
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new SupplierOutstandingHistoryViewModel.DetailViewModel();
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
            Session["HSupplierOutstandingTranList"] = list;

            return list;
        }
    }
}
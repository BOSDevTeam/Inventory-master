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
        CustomerOutstandingHistoryViewModel viewModel = new CustomerOutstandingHistoryViewModel();

        [SessionTimeoutAttribute]
        public ActionResult CustomerOutstandingPaymentHistory()
        {
            try
            {
                getCustomer(true);
                List<CustomerOutstandingHistoryViewModel.ListViewModel> tempList = selectCustomerOutstandingHistory(false);
                PagingViewModel pagingViewModel = calcCustomerOutstandingPaging(tempList);
                List<CustomerOutstandingHistoryViewModel.ListViewModel> lstCustomerOutstanding = getCustomerOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstCustomerOutstanding"] = lstCustomerOutstanding;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(viewModel);
        }

        #region OutstandingListAction

        [HttpGet]
        public JsonResult PaymentAction(int customerId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<CustomerOutstandingHistoryViewModel.DetailViewModel> lstCustomerOutstandingPayment = new List<CustomerOutstandingHistoryViewModel.DetailViewModel>();
            try
            {
                Session["CustomerOutstandingPaymentList"] = null;
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

        [HttpGet]
        public JsonResult SearchAction(DateTime fromDate, DateTime toDate, int customerId)
        {
            List<CustomerOutstandingHistoryViewModel.ListViewModel> tempList = selectCustomerOutstandingHistory(true, fromDate, toDate, customerId);
            PagingViewModel pagingViewModel = calcCustomerOutstandingPaging(tempList);
            List<CustomerOutstandingHistoryViewModel.ListViewModel> lstCustomerOutstanding = getCustomerOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstCustomerOutstanding = lstCustomerOutstanding
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RefreshAction()
        {
            List<CustomerOutstandingHistoryViewModel.ListViewModel> tempList = selectCustomerOutstandingHistory(false);
            PagingViewModel pagingViewModel = calcCustomerOutstandingPaging(tempList);
            List<CustomerOutstandingHistoryViewModel.ListViewModel> lstCustomerOutstanding = getCustomerOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstCustomerOutstanding = lstCustomerOutstanding
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OutstandingPagingAction(int currentPage)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<CustomerOutstandingHistoryViewModel.ListViewModel> lstCustomerOutstanding = new List<CustomerOutstandingHistoryViewModel.ListViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["CustomerOutstandingList"] != null)
            {
                try
                {
                    List<CustomerOutstandingHistoryViewModel.ListViewModel> tempList = Session["CustomerOutstandingList"] as List<CustomerOutstandingHistoryViewModel.ListViewModel>;
                    pagingViewModel = calcCustomerOutstandingPaging(tempList, currentPage);
                    lstCustomerOutstanding = getCustomerOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
                LstCustomerOutstanding = lstCustomerOutstanding,
                TotalPage = pagingViewModel.TotalPageNum,
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
            //if (Session["CustomerOutstandingTranList"] != null)
            //{
            //    try
            //    {
            //        List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> lstTranCustomerOutstanding = Session["CustomerOutstandingTranList"] as List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>;
            //        CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel data = lstTranCustomerOutstanding.Where(x => x.UserVoucherNo == userVoucherNo).SingleOrDefault();
            //        if (data.IsOpening)
            //        {
            //            if (payment > data.Opening)
            //            {
            //                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
            //                resultDefaultData.Message = AppConstants.Message.InvalidPayment;
            //            }
            //            else
            //            {
            //                List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> lstPayment = new List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>();
            //                if (Session["CustomerOutstandingPaymentList"] != null)
            //                {
            //                    lstPayment = Session["CustomerOutstandingPaymentList"] as List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>;
            //                }
            //                data.Payment = payment;

            //                int index = lstPayment.FindIndex(x => x.UserVoucherNo == userVoucherNo);
            //                if (index != -1)
            //                    lstPayment[index] = data;
            //                else lstPayment.Add(data);

            //                Session["CustomerOutstandingPaymentList"] = lstPayment;
            //                resultDefaultData.IsRequestSuccess = true;
            //            }
            //        }
            //        else
            //        {
            //            if (payment > data.Sale)
            //            {
            //                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
            //                resultDefaultData.Message = AppConstants.Message.InvalidPayment;
            //            }
            //            else
            //            {
            //                List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> lstPayment = new List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>();
            //                if (Session["CustomerOutstandingPaymentList"] != null)
            //                {
            //                    lstPayment = Session["CustomerOutstandingPaymentList"] as List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>;
            //                }
            //                data.Payment = payment;

            //                int index = lstPayment.FindIndex(x => x.UserVoucherNo == userVoucherNo);
            //                if (index != -1)
            //                    lstPayment[index] = data;
            //                else lstPayment.Add(data);

            //                Session["CustomerOutstandingPaymentList"] = lstPayment;
            //                resultDefaultData.IsRequestSuccess = true;
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
            //        resultDefaultData.Message = ex.Message;
            //    }
            //}
            //else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };

            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        private List<CustomerOutstandingHistoryViewModel.ListViewModel> selectCustomerOutstandingHistory(bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]int customerId)
        {
            List<CustomerOutstandingHistoryViewModel.ListViewModel> tempList = new List<CustomerOutstandingHistoryViewModel.ListViewModel>();
            CustomerOutstandingHistoryViewModel.ListViewModel item = new CustomerOutstandingHistoryViewModel.ListViewModel();
            int payment = 0;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetCustomerOutstandingHistory, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@CustomerID", 0);
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
            }
            cmd.Parameters.AddWithValue("@ARAccountCode", AppConstants.ARAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new CustomerOutstandingHistoryViewModel.ListViewModel();
                item.CustomerID = Convert.ToInt32(reader["VendorID"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                payment = Convert.ToInt32(reader["Payment"]);
                if (payment.ToString().StartsWith("-"))               
                    payment=Convert.ToInt32(payment.ToString().Remove(0, 1));               
                item.Payment = payment;
                tempList.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            Session["CustomerOutstandingList"] = tempList;  // for paging

            return tempList;
        }

        private PagingViewModel calcCustomerOutstandingPaging(List<CustomerOutstandingHistoryViewModel.ListViewModel> tempList, [Optional]int currentPage)
        {
            PagingViewModel item = new PagingViewModel();
            int totalPageNum = 0;

            if (currentPage == 0) currentPage = 1;
            if (tempList.Count > paging.eachItemCount)
            {
                totalPageNum = tempList.Count / paging.eachItemCount;
                paging.lastItemCount = tempList.Count % paging.eachItemCount;
                if (paging.lastItemCount != 0) totalPageNum += 1;

                int i = currentPage * paging.eachItemCount;
                int j = (i - paging.eachItemCount) + 1;
                int start = j;
                int end = i;
                paging.startItemIndex = start - 1;
                paging.endItemIndex = end - 1;
            }
            else
            {
                paging.startItemIndex = 0;
                paging.endItemIndex = tempList.Count - 1;
            }

            item.CurrentPage = currentPage;
            item.TotalPageNum = totalPageNum;
            item.StartItemIndex = paging.startItemIndex;
            item.EndItemIndex = paging.endItemIndex;

            return item;
        }

        private List<CustomerOutstandingHistoryViewModel.ListViewModel> getCustomerOutstandingByPaging(List<CustomerOutstandingHistoryViewModel.ListViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<CustomerOutstandingHistoryViewModel.ListViewModel> list = new List<CustomerOutstandingHistoryViewModel.ListViewModel>();
            CustomerOutstandingHistoryViewModel.ListViewModel item = new CustomerOutstandingHistoryViewModel.ListViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new CustomerOutstandingHistoryViewModel.ListViewModel();
                item.CustomerID = tempList[page].CustomerID;
                item.CustomerName = tempList[page].CustomerName;
                item.Payment = tempList[page].Payment;
                list.Add(item);
            }
            return list;
        }

        private void getCustomer(bool isIncludeDefault)
        {
            if (isIncludeDefault) viewModel.Customers.Add(new SelectListItem { Text = AppConstants.AllCustomer, Value = "0" });
            List<CustomerModels.CustomerModel> list = appData.selectCustomer();
            for (int i = 0; i < list.Count; i++)
            {
                viewModel.Customers.Add(new SelectListItem { Text = list[i].CustomerName, Value = Convert.ToString(list[i].CustomerID) });
            }
        }

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
            Session["CustomerOutstandingTranList"] = list;

            return list;
        }
    }
}
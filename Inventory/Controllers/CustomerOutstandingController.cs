using Inventory.Common;
using Inventory.Models;
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

namespace Inventory.Controllers
{
    public class CustomerOutstandingController : MyController
    {
        AppData appData = new AppData();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();
        CustomerOutstandingViewModel customerOutstandingViewModel = new CustomerOutstandingViewModel();
        int totalBalance;

        [SessionTimeoutAttribute]
        public ActionResult ListCustomerOutstanding()
        {
            try
            {
                getCustomer(true);
                List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> tempList = selectCustomerOutstanding(false);
                PagingViewModel pagingViewModel = calcCustomerOutstandingPaging(tempList);
                List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> lstCustomerOutstanding = getCustomerOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstCustomerOutstanding"] = lstCustomerOutstanding;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            
            return View(customerOutstandingViewModel);
        }

        #region OutstandingListAction

        [HttpGet]
        public JsonResult PaymentAction(int customerId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> lstCustomerOutstandingPayment = new List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>();
            try
            {
                Session["CustomerOutstandingPaymentList"] = null;
                lstCustomerOutstandingPayment = selectTranCustomerOutstanding(customerId, setting.getLocalDate());                
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
                TotalBalance=totalBalance,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(DateTime fromDate, DateTime toDate, int customerId)
        {
            List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> tempList = selectCustomerOutstanding(true, fromDate, toDate, customerId);
            PagingViewModel pagingViewModel = calcCustomerOutstandingPaging(tempList);
            List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> lstCustomerOutstanding = getCustomerOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
            List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> tempList = selectCustomerOutstanding(false);
            PagingViewModel pagingViewModel = calcCustomerOutstandingPaging(tempList);
            List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> lstCustomerOutstanding = getCustomerOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
            List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> lstCustomerOutstanding = new List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["CustomerOutstandingList"] != null)
            {
                try
                {
                    List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> tempList = Session["CustomerOutstandingList"] as List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel>;
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
        public JsonResult PayTypeChangeAction(int payType)
        {
            Session["CustomerOutstandingPaymentList"] = null;
            var jsonResult = new
            {
              
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangedPayDateAction(string userVoucherNo, string payDate)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["CustomerOutstandingTranList"] != null)
            {
                try
                {
                    List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> lstTranCustomerOutstanding = Session["CustomerOutstandingTranList"] as List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>;
                    CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel data = lstTranCustomerOutstanding.Where(x => x.UserVoucherNo == userVoucherNo).SingleOrDefault();
                    List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> lstPayment = new List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>();
                    if (Session["CustomerOutstandingPaymentList"] != null)
                    {
                        lstPayment = Session["CustomerOutstandingPaymentList"] as List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>;
                    }
                    data.PayDate = Convert.ToDateTime(payDate);

                    int index = lstPayment.FindIndex(x => x.UserVoucherNo == userVoucherNo);
                    if (index != -1)
                        lstPayment[index] = data;
                    else lstPayment.Add(data);

                    Session["CustomerOutstandingPaymentList"] = lstPayment;
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
        public JsonResult AddPaymentAction(string userVoucherNo, int payment)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["CustomerOutstandingTranList"] != null)
            {
                try
                {
                    List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> lstTranCustomerOutstanding = Session["CustomerOutstandingTranList"] as List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>;
                    CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel data = lstTranCustomerOutstanding.Where(x => x.UserVoucherNo == userVoucherNo).SingleOrDefault();
                    if (data.IsOpening)
                    {
                        if (payment > data.Opening)
                        {
                            resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                            resultDefaultData.Message = AppConstants.Message.InvalidPayment;
                        }
                        else
                        {
                            List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> lstPayment = new List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>();
                            if (Session["CustomerOutstandingPaymentList"] != null)
                            {
                                lstPayment = Session["CustomerOutstandingPaymentList"] as List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>;
                            }
                            data.Payment = payment;

                            int index = lstPayment.FindIndex(x => x.UserVoucherNo == userVoucherNo);
                            if (index != -1)
                                lstPayment[index] = data;
                            else lstPayment.Add(data);

                            Session["CustomerOutstandingPaymentList"] = lstPayment;
                            resultDefaultData.IsRequestSuccess = true;
                        }
                    }
                    else
                    {
                        if (payment > data.Sale)
                        {
                            resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                            resultDefaultData.Message = AppConstants.Message.InvalidPayment;
                        }
                        else
                        {
                            List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> lstPayment = new List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>();
                            if (Session["CustomerOutstandingPaymentList"] != null)
                            {
                                lstPayment = Session["CustomerOutstandingPaymentList"] as List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>;
                            }
                            data.Payment = payment;

                            int index = lstPayment.FindIndex(x => x.UserVoucherNo == userVoucherNo);
                            if (index != -1)
                                lstPayment[index] = data;
                            else lstPayment.Add(data);

                            Session["CustomerOutstandingPaymentList"] = lstPayment;
                            resultDefaultData.IsRequestSuccess = true;
                        }
                    }
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
        public JsonResult ResetAction()
        {
            Session["CustomerOutstandingPaymentList"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaveAction(int payType,int allPayment,string allPayDate, int customerId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["CustomerOutstandingPaymentList"] != null)
            {
                try
                {
                    List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>  lstPayment = Session["CustomerOutstandingPaymentList"] as List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>;
                
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("UserVoucherNo", typeof(string)));
                    dt.Columns.Add(new DataColumn("PayDate", typeof(DateTime)));
                    dt.Columns.Add(new DataColumn("Payment", typeof(int)));
                    for (int i = 0; i < lstPayment.Count; i++)
                    {
                        if(lstPayment[i].Payment!=0)
                            dt.Rows.Add(lstPayment[i].UserVoucherNo, lstPayment[i].PayDate, lstPayment[i].Payment);                    
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertCustomerOutstandingPayment, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@PayType", payType);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@AccountCode", AppConstants.ARAccountCode);
                    cmd.Connection = setting.conn;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();

                    Session["CustomerOutstandingPaymentList"] = null;
                    resultDefaultData.IsRequestSuccess = true;
                    resultDefaultData.Message = AppConstants.Message.PaymentSuccess;
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

        private void getCustomer(bool isIncludeDefault)
        {
            if (isIncludeDefault) customerOutstandingViewModel.Customers.Add(new SelectListItem { Text = AppConstants.AllCustomer, Value = "0" });
            List<CustomerModels.CustomerModel> list = appData.selectCustomer();
            for (int i = 0; i < list.Count; i++)
            {
                customerOutstandingViewModel.Customers.Add(new SelectListItem { Text = list[i].CustomerName, Value = Convert.ToString(list[i].CustomerID) });
            }
        }

        private List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> selectCustomerOutstanding(bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]int customerId)
        {
            List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> tempList = new List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel>();
            CustomerOutstandingViewModel.CustomerOutstandingListViewModel item = new CustomerOutstandingViewModel.CustomerOutstandingListViewModel();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetCustomerOutstanding, setting.conn);
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
            cmd.Parameters.AddWithValue("@SaleAccountCode", AppConstants.SaleAccountCode);
            cmd.Parameters.AddWithValue("@CustomerOpeningAccountCode", AppConstants.CustomerOpeningAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new CustomerOutstandingViewModel.CustomerOutstandingListViewModel();
                item.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.OutstandingOpening = Convert.ToInt32(reader["OutstandingOpening"]);
                item.AccountOpening = Convert.ToInt32(reader["AccountOpening"]);
                item.Sale = Convert.ToInt32(reader["Sale"]);
                item.Balance = Convert.ToInt32(reader["Balance"]);
                tempList.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            Session["CustomerOutstandingList"] = tempList;  // for paging

            return tempList;
        }

        private PagingViewModel calcCustomerOutstandingPaging(List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> tempList, [Optional]int currentPage)
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

        private List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> getCustomerOutstandingByPaging(List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel> list = new List<CustomerOutstandingViewModel.CustomerOutstandingListViewModel>();
            CustomerOutstandingViewModel.CustomerOutstandingListViewModel item = new CustomerOutstandingViewModel.CustomerOutstandingListViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new CustomerOutstandingViewModel.CustomerOutstandingListViewModel();
                item.CustomerID = tempList[page].CustomerID;
                item.CustomerName = tempList[page].CustomerName;
                item.OutstandingOpening = tempList[page].OutstandingOpening;
                item.AccountOpening = tempList[page].AccountOpening;
                item.Sale = tempList[page].Sale;
                item.Balance = tempList[page].Balance;
                list.Add(item);
            }
            return list;
        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.CustomerOutstandingModule, userId);
            return userVoucherNo;
        }

        private List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> selectTranCustomerOutstanding(int customerId, DateTime toDate)
        {
            List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel> list = new List<CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel>();
            CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel item = new CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel();
            totalBalance = 0;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranCustomerOutstanding, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@SaleAccountCode", AppConstants.SaleAccountCode);
            cmd.Parameters.AddWithValue("@CustomerOpeningAccountCode", AppConstants.CustomerOpeningAccountCode);
            cmd.Parameters.AddWithValue("@ARAccountCode", AppConstants.ARAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new CustomerOutstandingViewModel.CustomerOutstandingPaymentViewModel();
                item.Date = Convert.ToString(reader["Date"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.PayDate = setting.getLocalDate();
                item.IsOpening = Convert.ToBoolean(reader["IsOpening"]);
                item.Opening = Convert.ToInt32(reader["Opening"]);
                item.Sale = Convert.ToInt32(reader["Sale"]);
                list.Add(item);
                totalBalance += item.Opening + item.Sale;
            }
            reader.Close();
            setting.conn.Close();
            Session["CustomerOutstandingTranList"] = list;  // for paging         

            return list;
        }
    }
}
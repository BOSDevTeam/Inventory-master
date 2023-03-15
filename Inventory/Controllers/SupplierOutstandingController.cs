using Inventory.Common;
using Inventory.Models;
using Inventory.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;

namespace Inventory.Controllers
{
    public class SupplierOutstandingController : MyController
    {
        AppData appData = new AppData();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();
        SupplierOutstandingViewModel supplierOutstandingViewModel = new SupplierOutstandingViewModel();
        int totalBalance;

        [SessionTimeoutAttribute]
        public ActionResult SupplierOutstanding()
        {
            try
            {
                getSupplier(true);
                List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> tempList = selectSupplierOutstanding(false);
                PagingViewModel pagingViewModel = calcSupplierOutstandingPaging(tempList);
                List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> lstSupplierOutstanding = getSupplierOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstSupplierOutstanding"] = lstSupplierOutstanding;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(supplierOutstandingViewModel);
        }

        #region OutstandingListAction

        [HttpGet]
        public JsonResult PaymentAction(int supplierId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstSupplierOutstandingPayment = new List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>();
            try
            {
                Session["SupplierOutstandingPaymentList"] = null;
                lstSupplierOutstandingPayment = selectTranSupplierOutstanding(supplierId, setting.getLocalDate());
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
                TotalBalance = totalBalance,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(DateTime fromDate, DateTime toDate, int supplierId)
        {
            List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> tempList = selectSupplierOutstanding(true, fromDate, toDate, supplierId);
            PagingViewModel pagingViewModel = calcSupplierOutstandingPaging(tempList);
            List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> lstSupplierOutstanding = getSupplierOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstSupplierOutstanding = lstSupplierOutstanding
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RefreshAction()
        {
            List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> tempList = selectSupplierOutstanding(false);
            PagingViewModel pagingViewModel = calcSupplierOutstandingPaging(tempList);
            List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> lstSupplierOutstanding = getSupplierOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstSupplierOutstanding = lstSupplierOutstanding
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OutstandingPagingAction(int currentPage)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> lstSupplierOutstanding = new List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["SupplierOutstandingList"] != null)
            {
                try
                {
                    List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> tempList = Session["SupplierOutstandingList"] as List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel>;
                    pagingViewModel = calcSupplierOutstandingPaging(tempList, currentPage);
                    lstSupplierOutstanding = getSupplierOutstandingByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
                LstSupplierOutstanding = lstSupplierOutstanding,
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
            Session["SupplierOutstandingPaymentList"] = null;
            var jsonResult = new
            {

            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangedPayDateAction(string userVoucherNo, string payDate)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["SupplierOutstandingTranList"] != null)
            {
                try
                {
                    List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstTranSupplierOutstanding = Session["SupplierOutstandingTranList"] as List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>;
                    SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel data = lstTranSupplierOutstanding.Where(x => x.UserVoucherNo == userVoucherNo).SingleOrDefault();
                    List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstPayment = new List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>();
                    if (Session["SupplierOutstandingPaymentList"] != null)
                    {
                        lstPayment = Session["SupplierOutstandingPaymentList"] as List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>;
                    }
                    data.PayDate = Convert.ToDateTime(payDate);

                    int index = lstPayment.FindIndex(x => x.UserVoucherNo == userVoucherNo);
                    if (index != -1)
                        lstPayment[index] = data;
                    else lstPayment.Add(data);

                    Session["SupplierOutstandingPaymentList"] = lstPayment;
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
            if (Session["SupplierOutstandingTranList"] != null)
            {
                try
                {
                    List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstTranSupplierOutstanding = Session["SupplierOutstandingTranList"] as List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>;
                    SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel data = lstTranSupplierOutstanding.Where(x => x.UserVoucherNo == userVoucherNo).SingleOrDefault();
                    if (data.IsOpening)
                    {
                        if (payment > data.Opening)
                        {
                            resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                            resultDefaultData.Message = AppConstants.Message.InvalidPayment;
                        }
                        else
                        {
                            List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstPayment = new List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>();
                            if (Session["SupplierOutstandingPaymentList"] != null)
                            {
                                lstPayment = Session["SupplierOutstandingPaymentList"] as List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>;
                            }
                            data.Payment = payment;

                            int index = lstPayment.FindIndex(x => x.UserVoucherNo == userVoucherNo);
                            if (index != -1)
                                lstPayment[index] = data;
                            else lstPayment.Add(data);

                            Session["SupplierOutstandingPaymentList"] = lstPayment;
                            resultDefaultData.IsRequestSuccess = true;
                        }
                    }
                    else
                    {
                        if (payment > data.Purchase)
                        {
                            resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                            resultDefaultData.Message = AppConstants.Message.InvalidPayment;
                        }
                        else
                        {
                            List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstPayment = new List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>();
                            if (Session["SupplierOutstandingPaymentList"] != null)
                            {
                                lstPayment = Session["SupplierOutstandingPaymentList"] as List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>;
                            }
                            data.Payment = payment;

                            int index = lstPayment.FindIndex(x => x.UserVoucherNo == userVoucherNo);
                            if (index != -1)
                                lstPayment[index] = data;
                            else lstPayment.Add(data);

                            Session["SupplierOutstandingPaymentList"] = lstPayment;
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
        public JsonResult SaveAction(int payType, int allPayment, string allPayDate, int supplierId, int payMethodId, int bankPaymentId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (payType == 2)
            {
                if (Session["SupplierOutstandingTranList"] != null)
                {
                    List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstTran = Session["SupplierOutstandingTranList"] as List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>;
                    List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstPayment = new List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>();
                    SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel data = new SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel();
                    int amount = 0;

                    for (int i = 0; i < lstTran.Count(); i++)
                    {
                        data = lstTran[i];
                        data.PayDate = Convert.ToDateTime(allPayDate);
                        if (data.IsOpening) amount = data.Opening;
                        else amount = data.Purchase;

                        if (amount < allPayment)
                        {
                            data.Payment = amount;
                            allPayment -= amount;
                            lstPayment.Add(data);
                        }
                        else if (amount >= allPayment)
                        {
                            data.Payment = allPayment;
                            lstPayment.Add(data);
                            break;
                        }
                    }
                    Session["SupplierOutstandingPaymentList"] = lstPayment;
                }
                else
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
                    resultDefaultData.Message = AppConstants.Message.SessionExpired;
                }
            }

            if (Session["SupplierOutstandingPaymentList"] != null)
            {
                try
                {
                    List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstPayment = Session["SupplierOutstandingPaymentList"] as List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>;

                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("UserVoucherNo", typeof(string)));
                    dt.Columns.Add(new DataColumn("PayDate", typeof(DateTime)));
                    dt.Columns.Add(new DataColumn("Payment", typeof(int)));
                    for (int i = 0; i < lstPayment.Count; i++)
                    {
                        if (lstPayment[i].Payment != 0)
                            dt.Rows.Add(lstPayment[i].UserVoucherNo, lstPayment[i].PayDate, lstPayment[i].Payment);
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertSupplierOutstandingPayment, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                    cmd.Parameters.AddWithValue("@PayType", payType);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@AccountCode", AppConstants.APAccountCode);
                    cmd.Parameters.AddWithValue("@PayMethodID", payMethodId);
                    cmd.Parameters.AddWithValue("@BankPaymentID", bankPaymentId);
                    cmd.Connection = setting.conn;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();

                    Session["SupplierOutstandingPaymentList"] = null;
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

        [HttpGet]
        public JsonResult DateChangeAction(int supplierId, string date)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> lstSupplierOutstandingPayment = new List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>();
            try
            {
                Session["SupplierOutstandingPaymentList"] = null;
                lstSupplierOutstandingPayment = selectTranSupplierOutstanding(supplierId, Convert.ToDateTime(date));
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
                TotalBalance = totalBalance,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPayMethodAction()
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<PayMethodModels> lstPayMethod = new List<PayMethodModels>();
            List<BankPaymentModels> lstBankPayment = new List<BankPaymentModels>();
            try
            {
                lstPayMethod = appData.selectPayMethod();
                lstBankPayment = appData.selectBankPayment();
                resultDefaultData.IsRequestSuccess = true;
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }

            var jsonResult = new
            {
                LstPayMethod = lstPayMethod,
                LstBankPayment = lstBankPayment,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        private void getSupplier(bool isIncludeDefault)
        {
            if (isIncludeDefault) supplierOutstandingViewModel.Suppliers.Add(new SelectListItem { Text = AppConstants.AllSupplier, Value = "0" });
            List<SupplierModels.SupplierModel> list = appData.selectSupplier();
            for (int i = 0; i < list.Count; i++)
            {
                supplierOutstandingViewModel.Suppliers.Add(new SelectListItem { Text = list[i].SupplierName, Value = Convert.ToString(list[i].SupplierID) });
            }
        }

        private List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> selectSupplierOutstanding(bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]int supplierId)
        {
            List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> tempList = new List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel>();
            SupplierOutstandingViewModel.SupplierOutstandingListViewModel item = new SupplierOutstandingViewModel.SupplierOutstandingListViewModel();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetSupplierOutstanding, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@SupplierID", 0);
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@SupplierID", supplierId);
            }
            cmd.Parameters.AddWithValue("@PurchaseAccountCode", AppConstants.PurchaseAccountCode);
            cmd.Parameters.AddWithValue("@SupplierOpeningAccountCode", AppConstants.SupplierOpeningAccountCode);
            cmd.Parameters.AddWithValue("@APAccountCode", AppConstants.APAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new SupplierOutstandingViewModel.SupplierOutstandingListViewModel();
                item.SupplierID = Convert.ToInt32(reader["SupplierID"]);
                item.SupplierName = Convert.ToString(reader["SupplierName"]);
                int outOpening = Convert.ToInt32(reader["OutstandingOpening"]);
                int outOpeningPayment = Convert.ToInt32(reader["OutstandingOpeningPayment"]);
                int accountOpening = Convert.ToInt32(reader["AccountOpening"]);
                int accountOpeningPayment = Convert.ToInt32(reader["AccountOpeningPayment"]);
                item.OutstandingOpening = outOpening + (outOpeningPayment);
                item.AccountOpening = accountOpening + (accountOpeningPayment);
                int purchase = Convert.ToInt32(reader["Purchase"]);
                int purchasePayment = Convert.ToInt32(reader["PurchasePayment"]);
                item.Purchase = purchase + (purchasePayment);
                item.Balance = Convert.ToInt32(reader["Balance"]);
                tempList.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            Session["SupplierOutstandingList"] = tempList;  // for paging

            return tempList;
        }

        private PagingViewModel calcSupplierOutstandingPaging(List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> tempList, [Optional]int currentPage)
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

        private List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> getSupplierOutstandingByPaging(List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel> list = new List<SupplierOutstandingViewModel.SupplierOutstandingListViewModel>();
            SupplierOutstandingViewModel.SupplierOutstandingListViewModel item = new SupplierOutstandingViewModel.SupplierOutstandingListViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new SupplierOutstandingViewModel.SupplierOutstandingListViewModel();
                item.SupplierID = tempList[page].SupplierID;
                item.SupplierName = tempList[page].SupplierName;
                item.OutstandingOpening = tempList[page].OutstandingOpening;
                item.AccountOpening = tempList[page].AccountOpening;
                item.OpeningPayment = tempList[page].OpeningPayment;
                item.Purchase = tempList[page].Purchase;
                item.PurchasePayment = tempList[page].PurchasePayment;
                item.Balance = tempList[page].Balance;
                list.Add(item);
            }
            return list;
        }

        private List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> selectTranSupplierOutstanding(int supplierId, DateTime toDate)
        {
            List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel> list = new List<SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel>();
            SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel item = new SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel();
            totalBalance = 0;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranSupplierOutstanding, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierID", supplierId);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@PurchaseAccountCode", AppConstants.PurchaseAccountCode);
            cmd.Parameters.AddWithValue("@SupplierOpeningAccountCode", AppConstants.SupplierOpeningAccountCode);
            cmd.Parameters.AddWithValue("@APAccountCode", AppConstants.APAccountCode);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new SupplierOutstandingViewModel.SupplierOutstandingPaymentViewModel();
                item.Date = Convert.ToString(reader["OutstandingDate"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.PayDate = setting.getLocalDate();
                item.IsOpening = Convert.ToBoolean(reader["IsOpening"]);
                item.Opening = Convert.ToInt32(reader["Opening"]);
                item.Purchase = Convert.ToInt32(reader["Purchase"]);
                list.Add(item);
                totalBalance += item.Opening + item.Purchase;
            }
            reader.Close();
            setting.conn.Close();
            Session["SupplierOutstandingTranList"] = list;

            return list;
        }
    }
}
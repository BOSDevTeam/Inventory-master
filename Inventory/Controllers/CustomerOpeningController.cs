using Inventory.Common;
using Inventory.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using Inventory.Models;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace Inventory.Controllers
{
    public class CustomerOpeningController : MyController
    {       
        AppData appData = new AppData();
        CustomerOpeningViewModel customerOpeningViewModel = new CustomerOpeningViewModel();
        AppSetting.Paging paging = new AppSetting.Paging();

        [SessionTimeoutAttribute]
        public ActionResult CustomerOpening(int userId)
        {
            try
            {
                ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                getCustomer();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(customerOpeningViewModel);
        }

        [SessionTimeoutAttribute]
        public ActionResult ListCustomerOpening(int userId)
        {
            try
            {
                List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> tempList = selectMasterCustomerOpening(userId);
                PagingViewModel pagingViewModel = calcMasterCustomerOpeningPaging(tempList);
                List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> lstMasterCustomerOpening = getMasterCustomerOpeningByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstMasterCustomerOpening"] = lstMasterCustomerOpening;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(customerOpeningViewModel);
        }

        [HttpGet]
        public JsonResult CustomerClickAction(int customerId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            try
            {
                List<TranCustomerOpeningModels> list = new List<TranCustomerOpeningModels>();
                TranCustomerOpeningModels data = new TranCustomerOpeningModels();
                data.CustomerID = customerId;
                data.Balance = 0;
                if (Session["TranCustomerOpeningList"] != null)
                    list = Session["TranCustomerOpeningList"] as List<TranCustomerOpeningModels>;

                list.Add(data);
                Session["TranCustomerOpeningList"] = list;
                resultDefaultData.IsRequestSuccess = true;
            }
            catch(Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ResetAction()
        {
            Session["TranCustomerOpeningList"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AddBalanceAction(int customerId, int balance)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranCustomerOpeningList"] != null)
            {
                try
                {
                    List<TranCustomerOpeningModels> lstTranCustomerOpening = Session["TranCustomerOpeningList"] as List<TranCustomerOpeningModels>;
                    TranCustomerOpeningModels data = lstTranCustomerOpening.Where(x => x.CustomerID == customerId).SingleOrDefault();
                    int index = lstTranCustomerOpening.IndexOf(data);
                    data.Balance = balance;
                    lstTranCustomerOpening.RemoveAt(index);
                    lstTranCustomerOpening.Insert(index, data);
                    Session["TranCustomerOpeningList"] = lstTranCustomerOpening;
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

        [HttpPost]
        public JsonResult DeleteAction(int customerOpeningId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalPageNum = 0;

            if (Session["MasterCustomerOpeningList"] != null)
            {
                try
                {
                    //SqlCommand cmd = new SqlCommand(textQuery.deleteOpeningStockQuery(openingStockId), (SqlConnection)getConnection());
                    //cmd.CommandType = CommandType.Text;
                    //cmd.ExecuteNonQuery();

                    //List<OpeningStockViewModel.MasterOpeningStockViewModel> lstMasterOpeningStock = Session["MasterOpeningStockData"] as List<OpeningStockViewModel.MasterOpeningStockViewModel>;
                    //int index = lstMasterOpeningStock.FindIndex(x => x.OpeningStockID == openingStockId);
                    //lstMasterOpeningStock.RemoveAt(index);

                    //if (lstMasterOpeningStock.Count > paging.eachItemCount)
                    //{
                    //    totalPageNum = lstMasterOpeningStock.Count / paging.eachItemCount;
                    //    paging.lastItemCount = lstMasterOpeningStock.Count % paging.eachItemCount;
                    //    if (paging.lastItemCount != 0) totalPageNum += 1;
                    //}
                    resultDefaultData.IsRequestSuccess = true;
                    resultDefaultData.Message = AppConstants.Message.DeleteSuccess;
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
                TotalPage = totalPageNum,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CustomerOpeningPagingAction(int currentPage)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> lstMasterCustomerOpening = new List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["MasterCustomerOpeningList"] != null)
            {
                try
                {
                    List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> tempList = Session["MasterCustomerOpeningList"] as List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel>;
                    pagingViewModel = calcMasterCustomerOpeningPaging(tempList, currentPage);
                    lstMasterCustomerOpening = getMasterCustomerOpeningByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
                LstMasterCustomerOpening = lstMasterCustomerOpening,
                TotalPage = pagingViewModel.TotalPageNum,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.CustomerOpeningModule, userId);
            return userVoucherNo;
        }

        private void getCustomer()
        {
            List<CustomerModels.CustomerModel> list = appData.selectCustomer();
            for (int i = 0; i < list.Count; i++)
            {
                customerOpeningViewModel.Customers.Add(new SelectListItem { Text = list[i].CustomerName, Value = Convert.ToString(list[i].CustomerID) });
            }
        }

        private List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> selectMasterCustomerOpening(int userId)
        {
            List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> tempList = new List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel>();
            CustomerOpeningViewModel.MasterCustomerOpeningViewModel item = new CustomerOpeningViewModel.MasterCustomerOpeningViewModel();

            //SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterOpeningStockList, (SqlConnection)getConnection());
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@UserID", userId);

            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    item = new OpeningStockViewModel.MasterOpeningStockViewModel();
            //    item.OpeningStockID = Convert.ToInt32(reader["OpeningStockID"]);
            //    item.OpeningDateTime = Convert.ToString(reader["OpeningDateTime"]);
            //    item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
            //    item.VoucherID = Convert.ToString(reader["VoucherID"]);
            //    item.LocationName = Convert.ToString(reader["LocationName"]);
            //    item.UserName = Convert.ToString(reader["UserName"]);
            //    tempList.Add(item);
            //}
            //reader.Close();
            Session["MasterCustomerOpeningList"] = tempList;  // for paging

            return tempList;
        }

        private PagingViewModel calcMasterCustomerOpeningPaging(List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> tempList, [Optional]int currentPage)
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

        private List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> getMasterCustomerOpeningByPaging(List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> list = new List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel>();
            CustomerOpeningViewModel.MasterCustomerOpeningViewModel item = new CustomerOpeningViewModel.MasterCustomerOpeningViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new CustomerOpeningViewModel.MasterCustomerOpeningViewModel();
                item.CustomerOpeningID = tempList[page].CustomerOpeningID;
                item.OpeningDateTime = tempList[page].OpeningDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.VoucherID = tempList[page].VoucherID;
                item.UserName = tempList[page].UserName;
                list.Add(item);
            }
            return list;
        }
    }
}
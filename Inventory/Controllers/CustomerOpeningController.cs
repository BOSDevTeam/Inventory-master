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
using System.Data;

namespace Inventory.Controllers
{
    public class CustomerOpeningController : MyController
    {       
        AppData appData = new AppData();
        AppSetting setting = new AppSetting();
        TextQuery textQuery = new TextQuery();
        CustomerOpeningViewModel customerOpeningViewModel = new CustomerOpeningViewModel();
        AppSetting.Paging paging = new AppSetting.Paging();

        [SessionTimeoutAttribute]
        public ActionResult CustomerOpening(int userId, int? customerOpeningId)
        {
            try
            {
                getLocation();
                getCurrency();
                getCustomer();
                Session["TranCustomerOpeningList"] = null;
                if (customerOpeningId != null)
                {
                    ViewBag.IsEdit = true;
                    CustomerOpeningViewModel.MasterCustomerOpeningViewModel data = selectMasterCustomerOpeningByID((int)customerOpeningId);
                    List<TranCustomerOpeningModels> lstTranCustomerOpening = selectTranCustomerOpeningByID((int)customerOpeningId);
                    Session["TranCustomerOpeningList"] = lstTranCustomerOpening;
                    ViewBag.UserVoucherNo = data.UserVoucherNo;
                    DateTime date = setting.convertStringToDate(data.OpeningDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = data.VoucherID;
                    ViewBag.LocationID = data.LocationID;
                    ViewBag.CurrencyID = data.CurrencyID;
                    ViewBag.CustomerOpeningID = customerOpeningId;
                }
                else ViewBag.UserVoucherNo = getUserVoucherNo(userId);                
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(customerOpeningViewModel);
        }

        [SessionTimeoutAttribute]
        public ActionResult ListCustomerOpening()
        {
            try
            {
                List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> tempList = selectMasterCustomerOpening();
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
                if (!checkIsExistInTran(customerId))
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
                }else
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                    resultDefaultData.Message = AppConstants.Message.AlreadyAddCustomer;
                }                
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

        [HttpPost]
        public JsonResult SaveAction(string userVoucherNo, string date, string voucherId, int userId, int locationId, int currencyId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranCustomerOpeningList"] != null)
            {
                try
                {
                    List<TranCustomerOpeningModels> list = Session["TranCustomerOpeningList"] as List<TranCustomerOpeningModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("CustomerID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Balance", typeof(int)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].CustomerID, list[i].Balance);
                    }

                    DateTime openingDateTime = DateTime.Parse(date);
                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertCustomerOpening, setting.conn);
                    cmd.Parameters.AddWithValue("@OpeningDateTime", openingDateTime);                  
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.CustomerOpeningModule);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@CurrencyID", currencyId);
                    cmd.Parameters.AddWithValue("@AccountCode", AppConstants.CustomerOpeningAccountCode);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = setting.conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {                      
                        userVoucherNo = Convert.ToString(reader[0]);
                        Session["TranCustomerOpeningList"] = null;
                        resultDefaultData.IsRequestSuccess = true;
                        resultDefaultData.Message = AppConstants.Message.SaveSuccess;                     
                    }
                    reader.Close();
                    setting.conn.Close();
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
                UserVoucherNo = userVoucherNo,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditAction(int customerOpeningId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranCustomerOpeningList"] != null)
            {
                try
                {
                    List<TranCustomerOpeningModels> list = Session["TranCustomerOpeningList"] as List<TranCustomerOpeningModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("CustomerID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Balance", typeof(int)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].CustomerID, list[i].Balance);
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateCustomerOpening, setting.conn);
                    cmd.Parameters.AddWithValue("@CustomerOpeningID", customerOpeningId);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = setting.conn;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();
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
                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(textQuery.deleteCustomerOpeningQuery(customerOpeningId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = setting.conn;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();

                    List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> lstMasterCustomerOpening = Session["MasterCustomerOpeningList"] as List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel>;
                    int index = lstMasterCustomerOpening.FindIndex(x => x.CustomerOpeningID == customerOpeningId);
                    lstMasterCustomerOpening.RemoveAt(index);

                    if (lstMasterCustomerOpening.Count > paging.eachItemCount)
                    {
                        totalPageNum = lstMasterCustomerOpening.Count / paging.eachItemCount;
                        paging.lastItemCount = lstMasterCustomerOpening.Count % paging.eachItemCount;
                        if (paging.lastItemCount != 0) totalPageNum += 1;
                    }
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

        private bool checkIsExistInTran(int customerId)
        {
            bool result = false;          
            if (Session["TranCustomerOpeningList"] != null)
            {
                List<TranCustomerOpeningModels> lstTranCustomerOpening = Session["TranCustomerOpeningList"] as List<TranCustomerOpeningModels>;
                if (lstTranCustomerOpening.Count() != 0)
                {
                    TranCustomerOpeningModels data = lstTranCustomerOpening.Where(x => x.CustomerID == customerId).SingleOrDefault();
                    if (data != null) result = true;
                }
            }          
            return result;
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

        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation();
            for (int i = 0; i < list.Count; i++)
            {
                customerOpeningViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
        }

        private void getCurrency()
        {
            List<CurrencyModels> list = appData.selectCurrency();
            for (int i = 0; i < list.Count; i++)
            {
                customerOpeningViewModel.Currencies.Add(new SelectListItem { Text = list[i].Keyword, Value = Convert.ToString(list[i].CurrencyID) });
            }
        }

        private List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> selectMasterCustomerOpening()
        {
            List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel> tempList = new List<CustomerOpeningViewModel.MasterCustomerOpeningViewModel>();
            CustomerOpeningViewModel.MasterCustomerOpeningViewModel item = new CustomerOpeningViewModel.MasterCustomerOpeningViewModel();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterCustomerOpeningList, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;        
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new CustomerOpeningViewModel.MasterCustomerOpeningViewModel();
                item.CustomerOpeningID = Convert.ToInt32(reader["CustomerOpeningID"]);
                item.OpeningDateTime = Convert.ToString(reader["OpeningDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                tempList.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            Session["MasterCustomerOpeningList"] = tempList;  // for paging

            return tempList;
        }

        private CustomerOpeningViewModel.MasterCustomerOpeningViewModel selectMasterCustomerOpeningByID(int customerOpeningId)
        {
            CustomerOpeningViewModel.MasterCustomerOpeningViewModel item = new CustomerOpeningViewModel.MasterCustomerOpeningViewModel();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterCustomerOpeningByID, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerOpeningID", customerOpeningId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.OpeningDateTime = Convert.ToString(reader["Date"]);
                item.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
            }
            reader.Close();
            setting.conn.Close();
            return item;
        }

        private List<TranCustomerOpeningModels> selectTranCustomerOpeningByID(int customerOpeningId)
        {
            List<TranCustomerOpeningModels> list = new List<TranCustomerOpeningModels>();
            TranCustomerOpeningModels item = new TranCustomerOpeningModels();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranCustomerOpeningByID, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerOpeningID", customerOpeningId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranCustomerOpeningModels();
                item.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.Balance = Convert.ToInt32(reader["Balance"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
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
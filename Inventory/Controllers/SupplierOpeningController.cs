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
    public class SupplierOpeningController : MyController
    {
        AppData appData = new AppData();
        AppSetting setting = new AppSetting();
        TextQuery textQuery = new TextQuery();
        SupplierOpeningViewModel supplierOpeningViewModel = new SupplierOpeningViewModel();
        AppSetting.Paging paging = new AppSetting.Paging();

        [SessionTimeoutAttribute]
        public ActionResult SupplierOpening(int userId, int? supplierOpeningId)
        {
            try
            {
                getLocation();
                getCurrency();
                getSupplier();
                Session["TranSupplierOpeningList"] = null;
                if (supplierOpeningId != null)
                {
                    ViewBag.IsEdit = true;
                    SupplierOpeningViewModel.MasterSupplierOpeningViewModel data = selectMasterSupplierOpeningByID((int)supplierOpeningId);
                    List<TranSupplierOpeningModels> lstTranSupplierOpening = selectTranSupplierOpeningByID((int)supplierOpeningId);
                    Session["TranSupplierOpeningList"] = lstTranSupplierOpening;
                    ViewBag.UserVoucherNo = data.UserVoucherNo;
                    DateTime date = setting.convertStringToDate(data.OpeningDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = data.VoucherID;
                    ViewBag.LocationID = data.LocationID;
                    ViewBag.CurrencyID = data.CurrencyID;
                    ViewBag.SupplierOpeningID = supplierOpeningId;
                }
                else ViewBag.UserVoucherNo = getUserVoucherNo(userId);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(supplierOpeningViewModel);
        }

        [SessionTimeoutAttribute]
        public ActionResult ListSupplierOpening()
        {
            try
            {
                List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> tempList = selectMasterSupplierOpening();
                PagingViewModel pagingViewModel = calcMasterSupplierOpeningPaging(tempList);
                List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> lstMasterSupplierOpening = getMasterSupplierOpeningByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstMasterSupplierOpening"] = lstMasterSupplierOpening;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(supplierOpeningViewModel);
        }

        [HttpGet]
        public JsonResult SupplierClickAction(int supplierId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            try
            {
                if (!checkIsExistInTran(supplierId))
                {
                    List<TranSupplierOpeningModels> list = new List<TranSupplierOpeningModels>();
                    TranSupplierOpeningModels data = new TranSupplierOpeningModels();
                    data.SupplierID = supplierId;
                    data.Balance = 0;
                    if (Session["TranSupplierOpeningList"] != null)
                        list = Session["TranSupplierOpeningList"] as List<TranSupplierOpeningModels>;

                    list.Add(data);
                    Session["TranSupplierOpeningList"] = list;
                    resultDefaultData.IsRequestSuccess = true;
                }
                else
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                    resultDefaultData.Message = AppConstants.Message.AlreadyAddSupplier;
                }
            }
            catch (Exception ex)
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

            if (Session["TranSupplierOpeningList"] != null)
            {
                try
                {
                    List<TranSupplierOpeningModels> list = Session["TranSupplierOpeningList"] as List<TranSupplierOpeningModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("SupplierID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Balance", typeof(int)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].SupplierID, list[i].Balance);
                    }

                    DateTime openingDateTime = DateTime.Parse(date);
                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertSupplierOpening, setting.conn);
                    cmd.Parameters.AddWithValue("@OpeningDateTime", openingDateTime);
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.SupplierOpeningModule);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@CurrencyID", currencyId);
                    cmd.Parameters.AddWithValue("@AccountCode", AppConstants.SupplierOpeningAccountCode);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = setting.conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        userVoucherNo = Convert.ToString(reader[0]);
                        Session["TranSupplierOpeningList"] = null;
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
        public JsonResult EditAction(int supplierOpeningId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranSupplierOpeningList"] != null)
            {
                try
                {
                    List<TranSupplierOpeningModels> list = Session["TranSupplierOpeningList"] as List<TranSupplierOpeningModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("SupplierID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Balance", typeof(int)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].SupplierID, list[i].Balance);
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateSupplierOpening, setting.conn);
                    cmd.Parameters.AddWithValue("@SupplierOpeningID", supplierOpeningId);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@AccountCode", AppConstants.SupplierOpeningAccountCode);
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
            Session["TranSupplierOpeningList"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AddBalanceAction(int supplierId, int balance)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranSupplierOpeningList"] != null)
            {
                try
                {
                    List<TranSupplierOpeningModels> lstTranSupplierOpening = Session["TranSupplierOpeningList"] as List<TranSupplierOpeningModels>;
                    TranSupplierOpeningModels data = lstTranSupplierOpening.Where(x => x.SupplierID == supplierId).SingleOrDefault();
                    int index = lstTranSupplierOpening.IndexOf(data);
                    data.Balance = balance;
                    lstTranSupplierOpening.RemoveAt(index);
                    lstTranSupplierOpening.Insert(index, data);
                    Session["TranSupplierOpeningList"] = lstTranSupplierOpening;
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
        public JsonResult DeleteAction(int supplierOpeningId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalPageNum = 0;

            if (Session["MasterSupplierOpeningList"] != null)
            {
                try
                {
                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(textQuery.deleteSupplierOpeningQuery(supplierOpeningId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = setting.conn;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();

                    List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> lstMasterSupplierOpening = Session["MasterSupplierOpeningList"] as List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel>;
                    int index = lstMasterSupplierOpening.FindIndex(x => x.SupplierOpeningID == supplierOpeningId);
                    lstMasterSupplierOpening.RemoveAt(index);

                    if (lstMasterSupplierOpening.Count > paging.eachItemCount)
                    {
                        totalPageNum = lstMasterSupplierOpening.Count / paging.eachItemCount;
                        paging.lastItemCount = lstMasterSupplierOpening.Count % paging.eachItemCount;
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
        public JsonResult SupplierOpeningPagingAction(int currentPage)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> lstMasterSupplierOpening = new List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["MasterSupplierOpeningList"] != null)
            {
                try
                {
                    List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> tempList = Session["MasterSupplierOpeningList"] as List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel>;
                    pagingViewModel = calcMasterSupplierOpeningPaging(tempList, currentPage);
                    lstMasterSupplierOpening = getMasterSupplierOpeningByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
                LstMasterSupplierOpening = lstMasterSupplierOpening,
                TotalPage = pagingViewModel.TotalPageNum,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        private bool checkIsExistInTran(int supplierId)
        {
            bool result = false;
            if (Session["TranSupplierOpeningList"] != null)
            {
                List<TranSupplierOpeningModels> lstTranSupplierOpening = Session["TranSupplierOpeningList"] as List<TranSupplierOpeningModels>;
                if (lstTranSupplierOpening.Count() != 0)
                {
                    TranSupplierOpeningModels data = lstTranSupplierOpening.Where(x => x.SupplierID == supplierId).SingleOrDefault();
                    if (data != null) result = true;
                }
            }
            return result;
        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.SupplierOpeningModule, userId);
            return userVoucherNo;
        }

        private void getSupplier()
        {
            List<SupplierModels.SupplierModel> list = appData.selectSupplier();
            for (int i = 0; i < list.Count; i++)
            {
                supplierOpeningViewModel.Suppliers.Add(new SelectListItem { Text = list[i].SupplierName, Value = Convert.ToString(list[i].SupplierID) });
            }
        }

        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation();
            for (int i = 0; i < list.Count; i++)
            {
                supplierOpeningViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
        }

        private void getCurrency()
        {
            List<CurrencyModels> list = appData.selectCurrency();
            for (int i = 0; i < list.Count; i++)
            {
                supplierOpeningViewModel.Currencies.Add(new SelectListItem { Text = list[i].Keyword, Value = Convert.ToString(list[i].CurrencyID) });
            }
        }

        private List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> selectMasterSupplierOpening()
        {
            List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> tempList = new List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel>();
            SupplierOpeningViewModel.MasterSupplierOpeningViewModel item = new SupplierOpeningViewModel.MasterSupplierOpeningViewModel();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSupplierOpeningList, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new SupplierOpeningViewModel.MasterSupplierOpeningViewModel();
                item.SupplierOpeningID = Convert.ToInt32(reader["SupplierOpeningID"]);
                item.OpeningDateTime = Convert.ToString(reader["OpeningDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                tempList.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            Session["MasterSupplierOpeningList"] = tempList;  // for paging

            return tempList;
        }

        private SupplierOpeningViewModel.MasterSupplierOpeningViewModel selectMasterSupplierOpeningByID(int supplierOpeningId)
        {
            SupplierOpeningViewModel.MasterSupplierOpeningViewModel item = new SupplierOpeningViewModel.MasterSupplierOpeningViewModel();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSupplierOpeningByID, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierOpeningID", supplierOpeningId);
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

        private List<TranSupplierOpeningModels> selectTranSupplierOpeningByID(int supplierOpeningId)
        {
            List<TranSupplierOpeningModels> list = new List<TranSupplierOpeningModels>();
            TranSupplierOpeningModels item = new TranSupplierOpeningModels();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranSupplierOpeningByID, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierOpeningID", supplierOpeningId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranSupplierOpeningModels();
                item.SupplierID = Convert.ToInt32(reader["SupplierID"]);
                item.SupplierName = Convert.ToString(reader["SupplierName"]);
                item.Balance = Convert.ToInt32(reader["Balance"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }

        private PagingViewModel calcMasterSupplierOpeningPaging(List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> tempList, [Optional]int currentPage)
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

        private List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> getMasterSupplierOpeningByPaging(List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel> list = new List<SupplierOpeningViewModel.MasterSupplierOpeningViewModel>();
            SupplierOpeningViewModel.MasterSupplierOpeningViewModel item = new SupplierOpeningViewModel.MasterSupplierOpeningViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new SupplierOpeningViewModel.MasterSupplierOpeningViewModel();
                item.SupplierOpeningID = tempList[page].SupplierOpeningID;
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
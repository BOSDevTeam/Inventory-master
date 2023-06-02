using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;
using Inventory.ViewModels;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class TransferController : MyController
    {
        #region Page
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        TransferViewModel transferViewModel = new TransferViewModel();
        AppData appData = new AppData();
        AppSetting appSetting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();
        TextQuery textQuery = new TextQuery();

        [SessionTimeoutAttribute]
        public ActionResult Transfer(int userId, int? transferId)
        {
            if (checkConnection()) {
                getLocation();
                getUnit();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                clearTranTransfer();
                ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                int totalQuantity = 0;
                if (transferId != null)
                {
                    ViewBag.IsEdit = true;
                    MasterTransferViewModel data = selectMasterTransferByTransferID((int)transferId);
                    List<TranTransferModels> lstTranTransfer = selectTranTransferByTransferID((int)transferId);
                    for (int i = 0; i < lstTranTransfer.Count(); i++)
                    {
                        totalQuantity = lstTranTransfer[i].Quantity;
                    }
                    Session["TranTransferData"] = lstTranTransfer;
                    ViewBag.TotalItem = lstTranTransfer.Count();
                    ViewBag.UserVoucherNo = data.MasterTransferModel.UserVoucherNo;
                    DateTime date = appSetting.convertStringToDate(data.MasterTransferModel.TransferDateTime);
                    ViewBag.Date = appSetting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterTransferModel.VoucherID;
                    ViewBag.FromLocationID = data.FromLocationID;
                    ViewBag.ToLocationID = data.ToLocationID;
                    ViewBag.Total = data.MasterTransferModel.TotalQuantity;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.TransferID = transferId;
                    ViewBag.Remark = data.MasterTransferModel.Remark;
                }
                return View(transferViewModel);

            }
            return RedirectToAction("Login", "User");

        }

        [SessionTimeoutAttribute]
        public ActionResult ListTransfer()
        {
            List<TransferViewModel.MasterTransferModels> templist = selectMasterTransfer(false);
            PagingViewModel pagingViewModel = calcMasterTransferPaging(templist);
            List<TransferViewModel.MasterTransferModels> lstMaterTransfer = getMasterTransferByPaging(templist, paging.startItemIndex, paging.endItemIndex );
            ViewBag.TotalPage = pagingViewModel.TotalPageNum;
            ViewBag.CurrentPage = pagingViewModel.CurrentPage;
            ViewData["LstMasterTansfer"] = lstMaterTransfer;
            return View(transferViewModel);
        }

        #endregion

        #region Transfer Action

        public JsonResult SearchAction(int userId,DateTime fromDate, DateTime toDate, string userVoucherNo)
        {
            List<TransferViewModel.MasterTransferModels> templist = selectMasterTransfer(true, fromDate, toDate, userVoucherNo);
            PagingViewModel pagingViewModel = calcMasterTransferPaging(templist);
            List<TransferViewModel.MasterTransferModels> lstMaterTransfer = getMasterTransferByPaging(templist, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            
            var jsonResult = new
            {
                LstMasterTransfer = lstMaterTransfer,
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshAction(int userId)
        {
            List<TransferViewModel.MasterTransferModels> templist = selectMasterTransfer(false);
            PagingViewModel pagingViewModel = calcMasterTransferPaging(templist);
            List<TransferViewModel.MasterTransferModels> lstMaterTransfer = getMasterTransferByPaging(templist, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                LstMasterTransfer = lstMaterTransfer,
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TransferPagingAction(int currentPage)
        {
            List<TransferViewModel.MasterTransferModels> lstMaterTransfer = new List<TransferViewModel.MasterTransferModels>();
            PagingViewModel pagingViewModel = new PagingViewModel();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["MasterTransferData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<TransferViewModel.MasterTransferModels> templist = Session["MasterTransferData"] as List<TransferViewModel.MasterTransferModels>;
                    pagingViewModel = calcMasterTransferPaging(templist, currentPage);
                    lstMaterTransfer = getMasterTransferByPaging(templist, paging.startItemIndex, paging.endItemIndex);
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
                LstMasterTransfer = lstMaterTransfer,
                TotalPage = pagingViewModel.TotalPageNum,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ViewAction(int transferId)
        {
            MasterTransferViewModel item = selectMasterTransferByTransferID(transferId);
            List<TranTransferModels> lstTranTransfer = selectTranTransferByTransferID(transferId);
            var jsonResult = new
            {
                LstTranTransfer = lstTranTransfer,
                UserVoucherNo = item.MasterTransferModel.UserVoucherNo,
                VoucherID = item.MasterTransferModel.VoucherID,
                FromLocationName = item.FromLocationName,
                ToLocationName = item.ToLocationName,
                TransferDateTime = item.MasterTransferModel.TransferDateTime,
                User = item.UserName,
                TotalQty = item.MasterTransferModel.TotalQuantity,
                Remark = item.MasterTransferModel.Remark
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }

        public JsonResult DeleteAction(int transferId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["MasterTransferData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    SqlCommand cmd = new SqlCommand(textQuery.deleteTransferQuery(transferId), (SqlConnection)getConnection());
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    List<TransferViewModel.MasterTransferModels> lstMaterTransfer = Session["MasterTransferData"] as List<TransferViewModel.MasterTransferModels>;
                    int index = lstMaterTransfer.FindIndex(p => p.TransferID == transferId);
                    lstMaterTransfer.RemoveAt(index);
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
                ResultDefaultData = resultDefaultData,
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrepareToEditTranTransferAction(int number, bool isMultiUnit)
        {
            string productCode = "", productName = "";
            int productId = 0, quantity = 0;
            int? unitId = 0;

            List<TranTransferModels> list = new List<TranTransferModels>();
            TranTransferModels data = new TranTransferModels();
            List<UnitModels> lstUnit = new List<UnitModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranTransferData"] != null)
            {
                try
                {
                    list = Session["TranTransferData"] as List<TranTransferModels>;
                    if (list.Count != 0)
                    {
                        data = list[number - 1];
                        productId = data.ProductID;
                        productName = data.ProductName;
                        productCode = data.ProductCode;
                        quantity = data.Quantity;
                        unitId = data.UnitID;
                        if (isMultiUnit) lstUnit = getUnit();
                        resultDefaultData.IsRequestSuccess = true;
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
                ProductID = productId,
                ProductCode = productCode,
                ProductName = productName,
                Quantity = quantity,
                UnitID = unitId,
                LstUnit = lstUnit,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranTransferAddEditAction(int productId, string productCode, string productName, int quantity, int? unitId, string unitKeyword, bool isEdit, int? number)
        {
            List<TranTransferModels> list = new List<TranTransferModels>();
            TranTransferModels data = new TranTransferModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalQuantity = 0;
            data.ProductID = productId;
            data.ProductCode = productCode;
            data.ProductName = productName;
            data.Quantity = quantity;
            data.UnitID = unitId;
            if (unitKeyword != null) data.UnitKeyword = unitKeyword;
            if (!isEdit)
            {
                if (Session["TranTransferData"] != null)
                {
                    try
                    {
                        list = Session["TranTransferData"] as List<TranTransferModels>;
                        list.Add(data);
                    }
                    catch (Exception ex)
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                        resultDefaultData.Message = ex.Message;
                    }

                }
                else list.Add(data);
                resultDefaultData.IsRequestSuccess = true;
            }
            else
            {
                if (Session["TranTransferData"] != null)
                {
                    try
                    {
                        resultDefaultData.IsRequestSuccess = true;
                        list = Session["TranTransferData"] as List<TranTransferModels>;
                        int index = (int)number - 1;
                        list[index] = data;
                    }
                    catch (Exception ex)
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                        resultDefaultData.Message = ex.Message;
                    }

                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }

            for (int i=0; i<list.Count; i++)
            {
                totalQuantity += list[i].Quantity;
            }

            Session["TranTransferData"] = list;
            var jsonResult = new
            {
                TotalQuantity = totalQuantity,
                LstTranTransfer = list,
                ResultDefaultData = resultDefaultData

            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TransferDeleteAction(int number)
        {
            List<TranTransferModels> list = new List<TranTransferModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalQuantity = 0;
            if (Session["TranTransferData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    list = Session["TranTransferData"] as List<TranTransferModels>;
                    list.RemoveAt(number - 1);
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }

            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            for (int i = 0; i < list.Count; i++)
            {
                totalQuantity += list[i].Quantity;
            }

            Session["TranTransferData"] = list;

            var JsonResult = new
            {
                TotalQuantity = totalQuantity,
                ResultDefaultData = resultDefaultData,
                LstTranTransfer = list

            };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAction(int formLocation, int toLocation, string userVoucherNo, string date, string voucherId, 
            int userId, int totalQty, string remark)
        {
            bool isRequestSuccess = true;
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (formLocation != toLocation)
            {

                if (Session["TranTransferData"] != null)
                {
                    try
                    {
                        resultDefaultData.IsRequestSuccess = true;
                        List<TranTransferModels> list = Session["TranTransferData"] as List<TranTransferModels>;
                        DataTable dt = new DataTable();
                        dt.Columns.Add("TransferID", typeof(int));
                        dt.Columns.Add("ProductID", typeof(int));
                        dt.Columns.Add("Quantity", typeof(int));
                        dt.Columns.Add("UnitID", typeof(int));
                        for (int i = 0; i < list.Count; i++)
                        {
                            dt.Rows.Add(list[i].TransferID, list[i].ProductID, list[i].Quantity, list[i].UnitID);
                        }

                        DateTime transferDateTime = DateTime.Parse(date);
                        SqlCommand cmd = new SqlCommand(Procedure.PrcInsertTransfer, (SqlConnection)dataConnectorSQL.Connect());
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromLocationID", formLocation);
                        cmd.Parameters.AddWithValue("@ToLocationID", toLocation);
                        cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                        cmd.Parameters.AddWithValue("@TransferDateTime", date);
                        cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@TotalQuantity", totalQty);
                        cmd.Parameters.AddWithValue("@Remark", remark);
                        cmd.Parameters.AddWithValue("@temptbl", dt);
                        cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.TransferModule);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            userVoucherNo = Convert.ToString(reader[0]);
                            reader.Close();
                        }
                        dataConnectorSQL.Close();
                        clearTranTransfer();
                    }
                    catch (Exception ex)
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                        resultDefaultData.Message = ex.Message;
                    }
                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            }
            else
            {
                isRequestSuccess = false;

            }
            var jsonResult = new
            {
                UserVoucherNo = userVoucherNo,
                ResultDefaultData = resultDefaultData,
                IsRequestSuccess = isRequestSuccess
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditAction(int transferId, string date, string voucherId, int formLocation, int toLocation, int totalQty, string remark)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            bool isRequestSuccess = true;
            if (formLocation != toLocation)
            {
                if (Session["TranTransferData"] != null)
                {
                    try
                    {
                        resultDefaultData.IsRequestSuccess = true;
                        List<TranTransferModels> list = Session["TranTransferData"] as List<TranTransferModels>;
                        DataTable dt = new DataTable();
                        dt.Columns.Add("TransferID", typeof(int));
                        dt.Columns.Add("ProductID", typeof(int));
                        dt.Columns.Add("Quantity", typeof(int));
                        dt.Columns.Add("UnitID", typeof(int));
                        for (int i = 0; i < list.Count; i++)
                        {
                            dt.Rows.Add(list[i].TransferID, list[i].ProductID, list[i].Quantity, list[i].UnitID);
                        }

                        DateTime transferDateTime = DateTime.Parse(date);
                        SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateTransfer, (SqlConnection)dataConnectorSQL.Connect());
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransferID", transferId);
                        cmd.Parameters.AddWithValue("@FromLocationID", formLocation);
                        cmd.Parameters.AddWithValue("@ToLocationID", toLocation);
                        cmd.Parameters.AddWithValue("@TransferDateTime", date);
                        cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                        cmd.Parameters.AddWithValue("@TotalQuantity", totalQty);
                        cmd.Parameters.AddWithValue("@Remark", remark);
                        cmd.Parameters.AddWithValue("@temptbl", dt);
                        cmd.ExecuteNonQuery();
                        dataConnectorSQL.Close();
                        clearTranTransfer();
                    }
                    catch (Exception ex)
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                        resultDefaultData.Message = ex.Message;
                    }

                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetProductByCodeAction(string code, bool isMultiUnit)
        {
            string productName = "", productCode = "";
            int productId = 0;
           List<UnitModels> lstUnit = new List<UnitModels>();
            bool isExistProduct = true;
            ProductModels.ProductModel data = new ProductModels.ProductModel();
            data = appData.selectProductByCode(getConnection(), code);
            if (data.ProductID != 0)
            {
                productName = data.ProductName;
                productId = data.ProductID;
                productCode = data.Code;
                if (isMultiUnit) lstUnit = getUnit();
            }
            else isExistProduct = false;

            var jsonResult = new
            {
                ProductID = productId,
                ProductName = productName,
                Code = productCode,
                LstUnit = lstUnit,
                IsExistProduct = isExistProduct
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductByKeywordAction(string keyword)
        {
            List<ProductModels.ProductModel> list = appData.selectProductByKeyword(getConnection(), keyword);
            Session["SearchProductData"] = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchProductClickAction(int productId, bool isMultiUnit)
        {
            string productName = "", code = "";
            List<UnitModels> lstUnit = new List<UnitModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["SearchProductData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<ProductModels.ProductModel> list = Session["SearchProductData"] as List<ProductModels.ProductModel>;
                    var result = list.Where(p => p.ProductID == productId).SingleOrDefault();
                    productName = result.ProductName;
                    code = result.Code;
                    if (isMultiUnit) lstUnit = getUnit();

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
                ResultDefaultData = resultDefaultData,
                ProductName = productName,
                Code = code,
                LstUnit = lstUnit

            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelAction()
        {
            Session["TranTransferData"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Method

        private MasterTransferViewModel selectMasterTransferByTransferID(int transferId)
        {
            MasterTransferViewModel item = new MasterTransferViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterTransferByTransferID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TransferID", transferId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.UserName = Convert.ToString(reader["UserName"]);
                item.FromLocationID = Convert.ToInt32(reader["FromLocationID"]);
                item.FromLocationName = Convert.ToString(reader["FromLocationName"]);
                item.ToLocationID = Convert.ToInt32(reader["ToLocationID"]);
                item.ToLocationName = Convert.ToString(reader["ToLocationName"]);
                item.MasterTransferModel.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.MasterTransferModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.MasterTransferModel.TransferDateTime = Convert.ToString(reader["Date"]);
                item.MasterTransferModel.TotalQuantity = Convert.ToInt32(reader["TotalQuantity"]);
                item.MasterTransferModel.Remark = Convert.ToString(reader["Remark"]);
            }
            reader.Close();
            return item;
        }

        private List<TranTransferModels> selectTranTransferByTransferID(int transferId)
        {
            List<TranTransferModels> list = new List<TranTransferModels>();
            TranTransferModels item = new TranTransferModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranTransferByTransferID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TransferID", transferId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranTransferModels();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                list.Add(item);
            }
            reader.Close();
            return list;
        }

        private List<TransferViewModel.MasterTransferModels> getMasterTransferByPaging(List<TransferViewModel.MasterTransferModels> tempList, int startRowIndex, int endRowIndex)
        {
            List<TransferViewModel.MasterTransferModels> list = new List<TransferViewModel.MasterTransferModels>();
            TransferViewModel.MasterTransferModels item = new TransferViewModel.MasterTransferModels();
            for (int page = startRowIndex; page < tempList.Count(); page++)
            {
                if (page > endRowIndex) break;
                item = new TransferViewModel.MasterTransferModels();
                item.TransferID = tempList[page].TransferID;
                item.TransferDateTime = tempList[page].TransferDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.FromLocationName = tempList[page].FromLocationName;
                item.ToLocationName = tempList[page].ToLocationName;
                item.TotalQuantity = tempList[page].TotalQuantity;
                list.Add(item);
            }
            return list;
        }

        private PagingViewModel calcMasterTransferPaging(List<TransferViewModel.MasterTransferModels> tempList, [Optional] int currentPage)
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

        private List<TransferViewModel.MasterTransferModels> selectMasterTransfer(bool isSearch, [Optional]DateTime fromDate, [Optional] DateTime toDate, [Optional] string userVoucherNo)
        {
            List<TransferViewModel.MasterTransferModels> tempList = new List<TransferViewModel.MasterTransferModels>();
            TransferViewModel.MasterTransferModels item = new TransferViewModel.MasterTransferModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterTransferList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IsSearch", isSearch);
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", appSetting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", appSetting.getLocalDate());
                cmd.Parameters.AddWithValue("@UserVoucherNo", "");
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
            }
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TransferViewModel.MasterTransferModels();
                item.TransferID = Convert.ToInt32(reader["TransferID"]);
                item.TransferDateTime = Convert.ToString(reader["TransferDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.FromLocationName = Convert.ToString(reader["FromLocationName"]);
                item.ToLocationName = Convert.ToString(reader["ToLocationName"]);
                item.TotalQuantity = Convert.ToInt32(reader["TotalQuantity"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterTransferData"] = tempList;
            return tempList;
        }

        private void getProduct(int subMenuId)
        {
            transferViewModel.ProductMenus.Products = appData.selectProduct(getConnection(), subMenuId);
            Session["ProductData"] = transferViewModel.ProductMenus.Products;
        }

        private int getFirstSubMenuID()
        {
            int subMenuId = 0;
            if (transferViewModel.ProductMenus.SubMenus.Count() !=0)
            {
                SubMenuModels.SubMenuModel firstSubMenu = transferViewModel.ProductMenus.SubMenus.First();
                subMenuId = firstSubMenu.SubMenuID;
            }
            return subMenuId;
        }

        private void getSubMenu(int mainMenuId)
        {
            transferViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(), mainMenuId);
        }

        private int getFirstMainMenuID()
        {
            int mainMenuId = 0;
            if (transferViewModel.ProductMenus.MainMenus.Count() != 0)
            {
                MainMenuModels.MainMenuModel firstMainMenu = transferViewModel.ProductMenus.MainMenus.First();
                mainMenuId = firstMainMenu.MainMenuID;
            }
            return mainMenuId;
        }

        private void getMainMenu()
        {
            transferViewModel.ProductMenus.MainMenus = appData.selectMainMenu(getConnection());
        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.TransferModule, userId, getConnection());
            return userVoucherNo;
        }

        private List<UnitModels> getUnit()
        {
            List<UnitModels> list = new List<UnitModels>();
            if (Session["UnitData"] == null)
            {
                list = appData.selectUnit(getConnection());
                Session["UnitData"] = list;
            }
            else
            {
                list = Session["UnitData"] as List<UnitModels>;
            }
            return list;
        }

        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                transferViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
        }

        private void clearTranTransfer()
        {
            Session["TranTransferData"] = null;
        }

        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();
                connection = Session[AppConstants.SQLConnection];
            return connection;
        }
        private bool checkConnection() {
            if (Session[AppConstants.SQLConnection] != null) return true;
            else return false;
        }
        #endregion
    }
}
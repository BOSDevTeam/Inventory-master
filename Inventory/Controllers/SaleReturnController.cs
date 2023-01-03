using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace Inventory.Controllers
{
    public class SaleReturnController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        MasterSaleReturnViewModel saleReturnViewModel = new MasterSaleReturnViewModel();
        AppData appData = new AppData();
        AppSetting appSetting = new AppSetting();
        TextQuery textQuery = new TextQuery();
        AppSetting.Paging paging = new AppSetting.Paging();
        #region Page
        public ActionResult SaleReturn(int userId, int? saleReturnId)
        {
            if (checkConnection()) {
                ViewBag.PaymentID = 
                ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                clearTranSaleReturn();
                int totalQuantity = 0;
                int saleId = 0;
                if (saleReturnId != null) // saleReturn edit
                {
                    ViewBag.IsEdit = true;
                    MasterSaleReturnViewModel data = selectMasterSaleReturnBySaleReturnID((int)saleReturnId);
                    List<TranSaleReturnModels> lstTranSaleReturn = selectTranSaleReturnBySaleReturnID((int)saleReturnId);
                    List<TranSaleModels> lstTranSale = selectSale(1,data.ReturnVoucherNo);
                    for (int i = 0; i < lstTranSale.Count(); i++)
                    {
                        totalQuantity += lstTranSale[i].Quantity;
                        saleId = lstTranSale[i].SaleID;
                    }
                    Session["TranSaleReturnData"] = lstTranSaleReturn;
                    ViewBag.UserVoucherNo = data.UserVoucherNo;
                    DateTime date = appSetting.convertStringToDate(data.ReturnDateTime);
                    ViewBag.Date = appSetting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterSaleReturnModel.VoucherID;
                    ViewBag.LocationID = data.LocationID;
                    ViewBag.Total = data.Total;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.SaleReturnID = saleReturnId;                   
                    ViewBag.ReturnVoucherNo = data.ReturnVoucherNo;
                    ViewBag.SaleID = saleId;
                    ViewBag.Remark = data.Remark;
                    Session["TranSaleData"] = lstTranSale;
                }

            }
            return View();
        }

        public ActionResult ListSaleReturn(int userId)
        {
            List<MasterSaleReturnViewModel> tempList = selectMasterSaleReturn(userId, false);
            PagingViewModel pagingViewModel = calcMasterSaleReturnPaging(tempList);
            List<MasterSaleReturnViewModel> lstMasterSaleReturn = getMasterSaleReturnByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            ViewData["LstMasterSaleReturn"] = lstMasterSaleReturn;
            ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
            ViewBag.CurrentPage = pagingViewModel.CurrentPage;
            return View(saleReturnViewModel);
        }

        public JsonResult SaleReturnPagingAction(int currentPage)
        {
            List<MasterSaleReturnViewModel> lstMasterSaleReturn = new List<MasterSaleReturnViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["MasterSaleReturnData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<MasterSaleReturnViewModel> tempList = Session["MasterSaleReturnData"] as List<MasterSaleReturnViewModel>;
                    pagingViewModel = calcMasterSaleReturnPaging(tempList, currentPage);
                    lstMasterSaleReturn = getMasterSaleReturnByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
                LstMasterSaleReturn = lstMasterSaleReturn,
                TotalPage = pagingViewModel.TotalPageNum,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        #endregion
    
        #region Action
        [HttpPost]
        public JsonResult SaveAction(int saleId, string returnVoucherNo, string userVoucherNo, string date, string voucherId, int userId, int locationId, int totalAmount, string remark)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranSaleReturnData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<TranSaleReturnModels> list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
                    //List<TranSaleModels> saleList = Session["TranSaleData"] as List<TranSaleModels>;
                    //List<TranSaleModels> saleRemainList = new List<TranSaleModels>();
                    //for (int i = 0; i < list.Count; i++)
                    //{
                    //    foreach (var sale in saleList.Where(s => s.ID == list[i].ID))
                    //    {
                    //        sale.Quantity -= list[i].Quantity;
                    //        sale.Discount -= list[i].Discount;
                    //        sale.Amount -= list[i].Amount;
                    //    }
                    //}
                    //// For transale table
                    //saleRemainList = saleList.Where(s => s.Quantity !=0).ToList();
                    //DataTable remainDt = new DataTable();
                    //remainDt.Columns.Add("ProductID", typeof(int));
                    //remainDt.Columns.Add("Quantity", typeof(int));
                    //remainDt.Columns.Add("UnitID", typeof(int));
                    //remainDt.Columns.Add("SalePrice", typeof(int));
                    //remainDt.Columns.Add("CurrencyID", typeof(int));
                    //remainDt.Columns.Add("DiscountPercent", typeof(int));
                    //remainDt.Columns.Add("Discount", typeof(int));
                    //remainDt.Columns.Add("Amount", typeof(int));
                    //remainDt.Columns.Add("IsFOC", typeof(bool));
                    //for (int i = 0; i < saleRemainList.Count; i++)
                    //{
                    //    remainDt.Rows.Add(saleRemainList[i].ProductID, saleRemainList[i].Quantity, saleRemainList[i].UnitID, saleRemainList[i].SalePrice,
                    //        saleRemainList[i].CurrencyID, saleRemainList[i].DiscountPercent, saleRemainList[i].Discount, saleRemainList[i].Amount, saleRemainList[i].IsFOC);
                    //}

                    // For transalereturn talbe
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("UnitID", typeof(int));
                    dt.Columns.Add("SalePrice", typeof(int));
                    dt.Columns.Add("CurrencyID", typeof(int));
                    dt.Columns.Add("DiscountPercent", typeof(int));
                    dt.Columns.Add("Discount", typeof(int));
                    dt.Columns.Add("Amount", typeof(int));
                    dt.Columns.Add("IsFOC", typeof(bool));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].SalePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                    }

                    DateTime saleReturnDateTime = DateTime.Parse(date);
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertSaleReturn, dataConnectorSQL.Connect());

                    cmd.Parameters.AddWithValue("@SaleID", saleId);
                    cmd.Parameters.AddWithValue("@ReturnVoucherNo", returnVoucherNo);
                    cmd.Parameters.AddWithValue("@ReturnDateTime", saleReturnDateTime);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@Remark", remark);
                    cmd.Parameters.AddWithValue("@Total", totalAmount);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    //cmd.Parameters.AddWithValue("@temptblSale", remainDt);
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.SaleReturnModule);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        userVoucherNo = Convert.ToString(reader[0]);
                    }

                    reader.Close();
                    dataConnectorSQL.Close();
                    clearTranSaleReturn();
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
                UserVoucherNo = userVoucherNo
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditAction(int saleRetunrId, string date, string voucherId,int totalAmount, string remark)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranSaleReturnData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<TranSaleReturnModels> list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;

                    // For transalereturn talbe
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("UnitID", typeof(int));
                    dt.Columns.Add("SalePrice", typeof(int));
                    dt.Columns.Add("CurrencyID", typeof(int));
                    dt.Columns.Add("DiscountPercent", typeof(int));
                    dt.Columns.Add("Discount", typeof(int));
                    dt.Columns.Add("Amount", typeof(int));
                    dt.Columns.Add("IsFOC", typeof(bool));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].SalePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                    }

                    DateTime saleReturnDateTime = DateTime.Parse(date);
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateSaleReturn, dataConnectorSQL.Connect());
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SaleReturnID", saleRetunrId);
                    cmd.Parameters.AddWithValue("@ReturnDateTime", saleReturnDateTime);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@Remark", remark);
                    cmd.Parameters.AddWithValue("@Total", totalAmount);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.ExecuteNonQuery();
                    dataConnectorSQL.Close();
                    clearTranSaleReturn();
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

        [HttpGet]
        public JsonResult SearchActionByPaymentID(int paymentId, string keyword)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<TranSaleModels> list = new List<TranSaleModels>();
            TranSaleModels item = new TranSaleModels();
            MasterSaleModels data = new MasterSaleModels();
            int totalQuantity = 0;
            bool isExistPurchase = true, isVoucherFOC = false;
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterANDTranSaleByPaymentID, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", keyword);
            cmd.Parameters.AddWithValue("@PaymentID", paymentId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranSaleModels();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                data.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                data.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
                list.Add(item);
            }
            reader.Close();

            if (item.SaleID > 0)
            {
                if (data.IsVouFOC == false)
                {
                }
                else
                {
                    resultDefaultData.Message = AppConstants.Message.NoReturnFOCVoucher;
                    isVoucherFOC = true;
                }
            }
            else isExistPurchase = false;

            for (int i = 0; i < list.Count; i++)
            {
                totalQuantity += list[i].Quantity;
            }

            Session["TranSaleData"] = list;
            Session["MasterSaleData"] = data;
            var jsonResult = new
            {
                List = list,
                TotalQuantity = totalQuantity,
                IsExistPurchase = isExistPurchase,
                IsVoucherFOC = isVoucherFOC,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaleClickAction(int number)
        {
            List<TranSaleModels> list = new List<TranSaleModels>();
            MasterSaleModels item = new MasterSaleModels();
            TranSaleModels data = new TranSaleModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            string productName = "", unitName = "", currencyKeyword ="" ;
            int id = 0, productId = 0, quantity = 0,inputQuantity = 0, price = 0, disPercent = 0,locationId = 0,
                unitId = 0, currencyId = 0, saleId = 0;
            if (Session["TranSaleData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    item = Session["MasterSaleData"] as MasterSaleModels;
                    list = Session["TranSaleData"] as List<TranSaleModels>;
                    //data = list.Where(s => s.ID == number).FirstOrDefault();
                    data = list[number - 1];
                    id = data.ID;
                    productId = data.ProductID;
                    productName = data.ProductName;
                    unitId = Convert.ToInt32(data.UnitID);
                    unitName = data.UnitKeyword;
                    quantity = data.Quantity;
                    inputQuantity = data.Quantity;
                    price = data.SalePrice;
                    currencyId = Convert.ToInt32(data.CurrencyID);
                    currencyKeyword = data.CurrencyKeyword;
                    disPercent = data.DiscountPercent;
                    locationId = Convert.ToInt32(item.LocationID);
                    saleId = data.SaleID;
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
                ID = id,
                ProductID = productId,
                ProductName = productName,
                Quantity = quantity,
                InputQuantity = inputQuantity,
                UnitID = unitId,
                UnitName = unitName,
                SalePrice = price,
                CurrencyID = currencyId,
                CurrencyKeyword = currencyKeyword,
                DisPercent = disPercent,
                LocationID = locationId,
                SaleID = saleId,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranSaleReturnAddEditAction(int saleId, int productId, int locationId, string productName, int inputQuantity,int oquantity, int price,int? disPercent, int unitId, string unitKeyword, int currencyId, string currencyKeyword, bool isEdit, int? number)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels data = new TranSaleReturnModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalAmount = 0, discount = 0;
            data.SaleID = saleId;
            data.ProductID = productId;
            data.LocationID = locationId;
            data.ProductName = productName;
            data.Quantity = inputQuantity;
            data.InputQuantity = oquantity;
            data.UnitID = unitId;
            data.UnitKeyword = unitKeyword;
            data.SalePrice = price;
            data.CurrencyID = currencyId;
            data.CurrencyKeyword = currencyKeyword;
            data.DiscountPercent = Convert.ToInt32(disPercent);
            discount = ((inputQuantity * price) * Convert.ToInt32(disPercent)) / 100;
            data.Discount = discount;
            data.Amount = (inputQuantity * price) - discount;
            if (!isEdit)
            {
                if (Session["TranSaleReturnData"] != null)
                {
                    list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
                    list.Add(data);
                }
                else
                {
                    list.Add(data);
                }
                resultDefaultData.IsRequestSuccess = true;

            }
            else {
                if (Session["TranSaleReturnData"] != null)
                {
                    try
                    {
                        resultDefaultData.IsRequestSuccess = true;
                        list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
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

            for (int i = 0; i < list.Count(); i++)
            {
                totalAmount += list[i].Amount;
            }

            Session["TranSaleReturnData"] = list;

            var jsonResult = new
            {
                List = list,
                TotalAmount = totalAmount,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrepareToEditTranSaleReturnAction(int number)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels data = new TranSaleReturnModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            string productName = "", unitName = "", currencyKeyword = "";
            int productId = 0, locationId = 0,quantity = 0, inputQuantity=0, price = 0, disPercent = 0;
            if (Session["TranSaleReturnData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
                    data = list[number - 1];
                    productId = data.ProductID;
                    locationId = data.LocationID;
                    productName = data.ProductName;
                    inputQuantity = data.InputQuantity;
                    quantity = data.Quantity;
                    unitName = data.UnitKeyword;
                    price = data.SalePrice;
                    currencyKeyword = data.CurrencyKeyword;
                    disPercent = data.DiscountPercent;

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
                LocationID = locationId,
                ProductName = productName,
                InputQuantity = inputQuantity,
                Quantity = quantity,
                UnitKeyword = unitName,
                SalePrice = price,
                CurrencyKeyword = currencyKeyword,
                DisPercent = disPercent
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranSaleReturnDeleteAction(int number)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels data = new TranSaleReturnModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalAmount = 0;
            if (Session["TranSaleReturnData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
                    list.RemoveAt(number - 1);
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            for (int i = 0; i < list.Count(); i++)
            {
                totalAmount += list[i].Amount;
            }

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                List = list,
                TotalAmount = totalAmount
            };


            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelAction()
        {
            Session["TranSaleReturnData"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchAction(int userId, bool isSearch, DateTime fromDate, DateTime toDate, string userVoucherNo)
        {
            List<MasterSaleReturnViewModel> tempList = selectMasterSaleReturn(userId, isSearch, fromDate, toDate, userVoucherNo);
            PagingViewModel pagingViewModel = calcMasterSaleReturnPaging(tempList);
            List<MasterSaleReturnViewModel> lstMasterSaleReturn = getMasterSaleReturnByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                List = lstMasterSaleReturn,
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshAction(int userId, bool isSearch)
        {
            List<MasterSaleReturnViewModel> tempList = selectMasterSaleReturn(userId, isSearch);
            PagingViewModel pagingViewModel = calcMasterSaleReturnPaging(tempList);
            List<MasterSaleReturnViewModel> lstMasterSaleOrder = getMasterSaleReturnByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                List = lstMasterSaleOrder,
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ViewAction(int saleReturnId)
        {
            MasterSaleReturnViewModel item = selectMasterSaleReturnBySaleReturnID(saleReturnId);
            List<TranSaleReturnModels> lstTranSaleReturn = selectTranSaleReturnBySaleReturnID(saleReturnId);
            var jsonResult = new
            {
                LstTranSaleReturn = lstTranSaleReturn,
                List = item,
                UserVoucherNo = item.UserVoucherNo,
                VoucherID = item.MasterSaleReturnModel.VoucherID,
                LocationName = item.ShortName,
                ReturnDateTime = item.ReturnDateTime,
                User = item.UserName,
                Total = item.Total,
                Remark = item.Remark,
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAction(int saleReturnId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            int totalPageNum = 0;
            if (Session["MasterSaleReturnData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    //SqlCommand cmd = new SqlCommand(textQuery.deleteSaleReturnQuery(saleReturnId), (SqlConnection)getConnection());
                    //cmd.CommandType = CommandType.Text;
                    //cmd.ExecuteNonQuery();
                    List<MasterSaleReturnViewModel> lstMasterSaleReturn = Session["MasterSaleReturnData"] as List<MasterSaleReturnViewModel>;
                    int index = lstMasterSaleReturn.FindIndex(x => x.SaleReturnID == saleReturnId);
                    lstMasterSaleReturn.RemoveAt(index);
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
        #endregion

        #region Method
        private List<TranSaleModels> selectSale(int paymentId, string keyword)
        {
            MasterSaleModels data = new MasterSaleModels();
            List<TranSaleModels> list = new List<TranSaleModels>();
            TranSaleModels item = new TranSaleModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterANDTranSaleByPaymentID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", keyword);
            cmd.Parameters.AddWithValue("@PaymentID", paymentId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranSaleModels();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                data.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                data.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
                list.Add(item);

            }
            reader.Close();
            return list;
        }

        private List<MasterSaleReturnViewModel> getMasterSaleReturnByPaging(List<MasterSaleReturnViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<MasterSaleReturnViewModel> list = new List<MasterSaleReturnViewModel>();
            MasterSaleReturnViewModel item = new MasterSaleReturnViewModel();
            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;
                item = new MasterSaleReturnViewModel();
                item.SaleReturnID = tempList[page].SaleReturnID;
                item.ReturnDateTime = tempList[page].ReturnDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.ReturnVoucherNo = tempList[page].ReturnVoucherNo;
                item.Total = tempList[page].Total;
                list.Add(item);
            }
            return list;
        }

        private PagingViewModel calcMasterSaleReturnPaging(List<MasterSaleReturnViewModel> tempList, [Optional] int currentPage)
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

        private MasterSaleReturnViewModel selectMasterSaleReturnBySaleReturnID(int saleReturnId)
        {
            List<MasterSaleReturnViewModel> list = new List<MasterSaleReturnViewModel>();
            MasterSaleReturnViewModel data = new MasterSaleReturnViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleReturnBySaleReturnID, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleReturnID", saleReturnId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                data.SaleReturnID = Convert.ToInt32(reader["SaleReturnID"]);
                data.ReturnDateTime = Convert.ToString(reader["Date"]);
                data.ReturnVoucherNo = Convert.ToString(reader["ReturnVoucherNo"]);
                data.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                data.MasterSaleReturnModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                data.UserName = Convert.ToString(reader["UserName"]);
                data.ShortName = Convert.ToString(reader["LocationName"]);
                data.Total = Convert.ToInt32(reader["Total"]);
                data.Remark = Convert.ToString(reader["Remark"]);
            }

            reader.Close();
            
            return data;
        }

        private List<TranSaleReturnModels> selectTranSaleReturnBySaleReturnID(int saleReturnId)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels data = new TranSaleReturnModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranSaleReturnBySaleReturnID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleReturnID", saleReturnId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data = new TranSaleReturnModels();
                data.ProductID = Convert.ToInt32(reader["ProductID"]);
                data.ProductName = Convert.ToString(reader["ProductName"]);
                data.Quantity = Convert.ToInt32(reader["Quantity"]);
                data.InputQuantity = Convert.ToInt32(reader["Quantity"]);
                data.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                data.Amount = Convert.ToInt32(reader["Amount"]);
                data.UnitID = Convert.ToInt32(reader["UnitID"]);
                data.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                data.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                data.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                data.Discount = Convert.ToInt32(reader["Discount"]);
                data.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                list.Add(data);
            }

            reader.Close();

            return list;
        }

        private List<MasterSaleReturnViewModel> selectMasterSaleReturn(int userId, bool isSearch, [Optional]DateTime fromDate,[Optional]DateTime toDate,[Optional]string userVoucherNo)
        {
            List<MasterSaleReturnViewModel> list = new List<MasterSaleReturnViewModel>();
            MasterSaleReturnViewModel data = new MasterSaleReturnViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleReturnList, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
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
                data = new MasterSaleReturnViewModel();
                data.SaleReturnID = Convert.ToInt32(reader["SaleReturnID"]);
                data.MasterSaleReturnModel.UserID = Convert.ToInt32(reader["UserID"]);
                data.MasterSaleReturnModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                data.MasterSaleReturnModel.Remark = Convert.ToString(reader["Remark"]);
                data.ReturnDateTime = Convert.ToString(reader["ReturnDateTime"]);
                data.ReturnVoucherNo = Convert.ToString(reader["ReturnVoucherNo"]);
                data.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                data.Total = Convert.ToInt32(reader["Total"]);
                data.LocationID = Convert.ToInt32(reader["LocationID"]);
                data.ShortName = Convert.ToString(reader["ShortName"]);
                list.Add(data);
            }
            reader.Close();
            Session["MasterSaleReturnData"] = list;
            return list;
        }

        private void clearTranSaleReturn()
        {
            Session["TranSaleReturnData"] = null;

        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.SaleReturnModule, userId, getConnection());
            return userVoucherNo;
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
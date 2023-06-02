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
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class PurchaseOrderController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
        AppData appData = new AppData();
        AppSetting appSetting = new AppSetting();
        TextQuery textQuery = new TextQuery();
        Procedure procedure = new Procedure();
        AppSetting.Paging paging = new AppSetting.Paging();

        #region page
        [SessionTimeoutAttribute]
        public ActionResult PurchaseOrder(int userId, int? purOrderId)
        {
            if (checkConnection())
            {
                getSupplier(false);
                getLocation();
                getUnit();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                clearTranPurchaseOrder();
                ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                int totalQuantity = 0;
                if (purOrderId != null)
                {
                    ViewBag.IsEdit = true;
                    MasterPurOrderViewModel data = selectMasterPurOrderByPurOrdeID((int)purOrderId);
                    List<TranPurchaseOrderModels> lstTranPurchaseOrder = selectTranPurOrderByPurOrderID((int)purOrderId);
                    for (int i = 0; i < lstTranPurchaseOrder.Count(); i++)
                    {
                        totalQuantity = lstTranPurchaseOrder[i].Quantity;
                    }
                    Session["TranPurchaseOrderData"] = lstTranPurchaseOrder;
                    ViewBag.TotalItem = lstTranPurchaseOrder.Count();
                    ViewBag.UserVoucherNo = data.MasterPurchaseOrderModel.UserVoucherNo;
                    DateTime date = appSetting.convertStringToDate(data.MasterPurchaseOrderModel.OrderDateTime);
                    ViewBag.Date = appSetting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterPurchaseOrderModel.VoucherID;
                    ViewBag.SupplierID = data.MasterPurchaseOrderModel.SupplierID;
                    ViewBag.LocationID = data.MasterPurchaseOrderModel.LocationID;
                    ViewBag.Subtotal = data.MasterPurchaseOrderModel.Subtotal;
                    ViewBag.TaxAmt = data.MasterPurchaseOrderModel.TaxAmt;
                    ViewBag.ChargesAmt = data.MasterPurchaseOrderModel.ChargesAmt;
                    ViewBag.Total = data.MasterPurchaseOrderModel.Total;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.PurchaseOrderID = purOrderId;
                }
                return View(purchaseOrderViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        [SessionTimeoutAttribute]
        public ActionResult ListPurchaseOrder()
        {
            getSupplier(true);
            List<PurchaseOrderViewModel.MasterPurchaseOrderModels> tempList = selectMasterPurOrder(false);
            PagingViewModel pagingViewModel = calcMasterPurOrderPaging(tempList);
            List<PurchaseOrderViewModel.MasterPurchaseOrderModels> lstMasterPurOrder = getMasterPurOrderByPaging(tempList, paging.startItemIndex, paging.endItemIndex);
            ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
            ViewBag.CurrentPage = pagingViewModel.CurrentPage;
            ViewData["LstMasterPurOrder"] = lstMasterPurOrder;
            return View(purchaseOrderViewModel);
        }

        #endregion

        #region purchaseAction

        [HttpPost]
        public JsonResult SaveAction(string userVoucherNo, string date, string voucherId, int userId, int supplierId, int locationId,
            int subtotal, int tax, int taxAmt, int charges, int chargesAmt, int total)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranPurchaseOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<TranPurchaseOrderModels> list = Session["TranPurchaseOrderData"] as List<TranPurchaseOrderModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("UnitID", typeof(int));
                    dt.Columns.Add("PurPrice", typeof(int));
                    dt.Columns.Add("CurrencyID", typeof(int));
                    dt.Columns.Add("Amount", typeof(int));
                    dt.Columns.Add("IsFOC", typeof(bool));
                    for (int i = 0; i < list.Count(); i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].PurPrice, list[i].CurrencyID, list[i].Amount, list[i].IsFOC);
                    }

                    DateTime purOrderDateTime = DateTime.Parse(date);
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertPurchaseOrder, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@OrderDateTime", purOrderDateTime);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Tax", tax);
                    cmd.Parameters.AddWithValue("@TaxAmt", taxAmt);
                    cmd.Parameters.AddWithValue("@Charges", charges);
                    cmd.Parameters.AddWithValue("@ChargesAmt", chargesAmt);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.PurchaseOrderModule);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) userVoucherNo = Convert.ToString(reader[0]);
                    reader.Close();
                    dataConnectorSQL.Close();
                    clearTranPurchaseOrder();
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

        public JsonResult EditAction(int purOrderId, string date, string voucherId, int supplierId, int locationId,
            int subtotal, int taxAmt, int chargesAmt, int total)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranPurchaseOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<TranPurchaseOrderModels> list = Session["TranPurchaseOrderData"] as List<TranPurchaseOrderModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("UnitID", typeof(int));
                    dt.Columns.Add("PurPrice", typeof(int));
                    dt.Columns.Add("CurrencyID", typeof(int));
                    dt.Columns.Add("Amount", typeof(int));
                    dt.Columns.Add("IsFOC", typeof(int));
                    for (int i = 0; i < list.Count(); i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].PurPrice, list[i].CurrencyID, list[i].Amount, list[i].IsFOC);
                    }

                    DateTime purOrderDateTime = DateTime.Parse(date);
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdatePurchaseOrder, (SqlConnection)dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@PurchaseOrderID", purOrderId);
                    cmd.Parameters.AddWithValue("@OrderDateTime", purOrderDateTime);
                    cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@TaxAmt", taxAmt);
                    cmd.Parameters.AddWithValue("@ChargesAmt", chargesAmt);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    dataConnectorSQL.Close();
                    clearTranPurchaseOrder();
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

        public JsonResult TranPurchaseOrderAddEditAction(int productId, string productCode, string productName, int quantity, int price, int? unitId, string unitKeyword, int? currencyId, string currencyKeyword, bool isEdit, int? number, bool isItemFOC)
        {
            List<TranPurchaseOrderModels> lstTranPurchaseOrder = new List<TranPurchaseOrderModels>();
            TranPurchaseOrderModels data = new TranPurchaseOrderModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int subtotal = 0, totalQuantity = 0;
            data.ProductID = productId;
            data.ProductName = productName;
            data.ProductCode = productCode;
            data.Quantity = quantity;
            data.PurPrice = price;
            data.UnitID = unitId;
            if (unitKeyword != null) data.UnitKeyword = unitKeyword;
            else data.UnitKeyword = "";
            data.CurrencyID = currencyId;
            if (currencyKeyword != null) data.CurrencyKeyword = currencyKeyword;
            else data.CurrencyKeyword = "";
            data.Amount = (quantity * price);
            data.IsFOC = isItemFOC;
            if (!isEdit)
            {
                if (Session["TranPurchaseOrderData"] != null)
                {

                    lstTranPurchaseOrder = Session["TranPurchaseOrderData"] as List<TranPurchaseOrderModels>;
                    lstTranPurchaseOrder.Add(data);
                }
                else lstTranPurchaseOrder.Add(data);
                resultDefaultData.IsRequestSuccess = true;

            }
            else {
                if (Session["TranPurchaseOrderData"] != null)
                {
                    try
                    {
                        resultDefaultData.IsRequestSuccess = true;
                        lstTranPurchaseOrder = Session["TranPurchaseOrderData"] as List<TranPurchaseOrderModels>;
                        int index = (int)number - 1;
                        lstTranPurchaseOrder[index] = data;
                    }
                    catch (Exception ex)
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                        resultDefaultData.Message = ex.Message;
                    }

                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            }
            for (int i = 0; i< lstTranPurchaseOrder.Count(); i++)
            {
                subtotal += lstTranPurchaseOrder[i].Amount;
                totalQuantity += lstTranPurchaseOrder[i].Quantity;
            }
            Session["TranPurchaseOrderData"] = lstTranPurchaseOrder;
            var jsonResult = new
            {
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                ResultDefaultData = resultDefaultData,
                LstTranPurchaseOrder = lstTranPurchaseOrder
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrepareToEditTranPurchaseOrderAction(int number, bool isMultiUnit, bool isMultiCurrency)
        {
            List<TranPurchaseOrderModels> lstTranPurchaseOrder = new List<TranPurchaseOrderModels>();
            TranPurchaseOrderModels data = new TranPurchaseOrderModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            string productCode = "", productName = "";
            int productId = 0, quantity = 0, price = 0;
            int? unitId = 0, currencyId = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isFOC = false;
            if (Session["TranPurchaseOrderData"] != null)
            {
                try
                {
                    lstTranPurchaseOrder = Session["TranPurchaseOrderData"] as List<TranPurchaseOrderModels>;
                    if (lstTranPurchaseOrder.Count() != 0)
                    {
                        data = lstTranPurchaseOrder[number - 1];
                        if (data != null)
                        {
                            productId = data.ProductID;
                            productCode = data.ProductCode;
                            productName = data.ProductName;
                            quantity = data.Quantity;
                            price = data.PurPrice;
                            unitId = data.UnitID;
                            currencyId = data.CurrencyID;
                            if (isMultiUnit) lstUnit = getUnit();
                            if (isMultiCurrency) lstCurrency = getCurrency();
                            isFOC = data.IsFOC;
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
                ProductID = productId,
                ProductCode = productCode,
                ProductName = productName,
                Quantity = quantity,
                Price = price,
                UnitID = unitId,
                CurrencyID = currencyId,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsFOC = isFOC,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurPriceAction(int productId)
        {
            int price = appData.selectPurPriceByProduct(getConnection(), productId);
            return Json(price, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductByKeywordAction(string keyword)
        {
            List<ProductModels.ProductModel> list = appData.selectProductByKeyword(getConnection(), keyword);
            Session["SearchProductData"] = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchProductClickAction (int productId, bool isMultiUnit, bool isMultiCurrency)
        {
            string productName = "", code = "";
            int purchasePrice = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["SearchProductData"] != null)
            {
                try
                {
                    List<ProductModels.ProductModel> list = Session["SearchProductData"] as List<ProductModels.ProductModel>;
                    var result = list.Where(p => p.ProductID == productId).SingleOrDefault();
                    productName = result.ProductName;
                    code = result.Code;
                    purchasePrice = result.PurchasePrice;
                    if (isMultiUnit) lstUnit = getUnit();
                    if (isMultiCurrency) lstCurrency = getCurrency();
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
                ProductName = productName,
                Code = code,
                PurPrice = purchasePrice,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductByCodeAction(string productCode, bool isMultiUnit, bool isMultiCurrency)
        {
            ProductModels.ProductModel data = new ProductModels.ProductModel();
            string productName = "";
            int productId = 0, price = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isExistProduct = true;
            data = appData.selectProductByCode(getConnection(), productCode);
            if (data.ProductID != 0)
            {
                productId = data.ProductID;
                productName = data.ProductName;
                price = data.PurchasePrice;
                if (isMultiUnit) lstUnit = getUnit();
                if (isMultiCurrency) lstCurrency = getCurrency();
            }
            else {
                isExistProduct = false;
            }
            var jsonResult = new
            {
                ProductID = productId,
                ProductName = productName,
                PurchasePrice = price,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsExistProduct = isExistProduct
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranPurOrderDeleteAction(int number)
        {
            List<TranPurchaseOrderModels> lstTranPurOrder = new List<TranPurchaseOrderModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int subtotal = 0, totalQuantity = 0;
            if (Session["TranPurchaseOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    lstTranPurOrder = Session["TranPurchaseOrderData"] as List<TranPurchaseOrderModels>;
                    lstTranPurOrder.RemoveAt(number - 1);
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }

            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            for (int i = 0; i < lstTranPurOrder.Count(); i++)
            {
                subtotal += lstTranPurOrder[i].Amount;
                totalQuantity += lstTranPurOrder[i].Quantity;
            }
            Session["TranPurchaseOrderData"] = lstTranPurOrder;
            var jsonResult = new
            {
                LstTranPurchaseOrder = lstTranPurOrder,
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelAction()
        {
            Session["TranPurchaseOrderData"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchAction(int userId, DateTime fromDate, DateTime toDate, string userVoucherNo, int supplierId)
        {
            List<PurchaseOrderViewModel.MasterPurchaseOrderModels> tempList = selectMasterPurOrder(true,fromDate, toDate, userVoucherNo, supplierId);
            PagingViewModel pagingViewModel = calcMasterPurOrderPaging(tempList);
            List<PurchaseOrderViewModel.MasterPurchaseOrderModels> lstMasterPurOrder = getMasterPurOrderByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                LstMasterPurOrder = lstMasterPurOrder,
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RefreshAction(int userId)
        {
            List<PurchaseOrderViewModel.MasterPurchaseOrderModels> tempList = selectMasterPurOrder(false);
            PagingViewModel pagingViewModel = calcMasterPurOrderPaging(tempList);
            List<PurchaseOrderViewModel.MasterPurchaseOrderModels> lstMasterPurOrder = getMasterPurOrderByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                LstMasterPurOrder = lstMasterPurOrder,
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PurOrderPagingAction(int currentPage)
        {
            bool isRequestSuccess = true;
            List<PurchaseOrderViewModel.MasterPurchaseOrderModels> lstMasterPurOrder = new List<PurchaseOrderViewModel.MasterPurchaseOrderModels>();
            PagingViewModel pagingViewModel = new PagingViewModel();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["MasterPurOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<PurchaseOrderViewModel.MasterPurchaseOrderModels> tempList = Session["MasterPurOrderData"] as List<PurchaseOrderViewModel.MasterPurchaseOrderModels>;
                    pagingViewModel = calcMasterPurOrderPaging(tempList, currentPage);
                    lstMasterPurOrder = getMasterPurOrderByPaging(tempList, paging.startItemIndex, paging.endItemIndex);
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
                LstMasterPurOrder = lstMasterPurOrder,
                TotalPage = pagingViewModel.TotalPageNum,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ViewAction(int purOrderId)
        {
            MasterPurOrderViewModel item = selectMasterPurOrderByPurOrdeID(purOrderId);
            List<TranPurchaseOrderModels> lstTranPurOrder = selectTranPurOrderByPurOrderID(purOrderId);
            var jsonResult = new
            {
                LstTranPurchaseOrder = lstTranPurOrder,
                UserVoucherNo = item.MasterPurchaseOrderModel.UserVoucherNo,
                VoucherID = item.MasterPurchaseOrderModel.VoucherID,
                LocationName = item.LocationName,
                PurOrderDateTime = item.MasterPurchaseOrderModel.OrderDateTime,
                User = item.UserName,
                SupplierName = item.SupplierName,
                Subtotal = item.MasterPurchaseOrderModel.Subtotal,
                TaxAmt = item.MasterPurchaseOrderModel.TaxAmt,
                ChargesAmt = item.MasterPurchaseOrderModel.ChargesAmt,
                Total = item.MasterPurchaseOrderModel.Total,
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAction(int purOrderId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["MasterPurOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    SqlCommand cmd = new SqlCommand(textQuery.deletePurchaseOrderQuery(purOrderId), (SqlConnection)getConnection());
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    List<PurchaseOrderViewModel.MasterPurchaseOrderModels> lstMasterPurOrder = Session["MasterPurOrderData"] as List<PurchaseOrderViewModel.MasterPurchaseOrderModels>;
                    int index = lstMasterPurOrder.FindIndex(p => p.PurchaseOrderID == purOrderId);
                    lstMasterPurOrder.RemoveAt(index);
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

        #endregion

        #region Method
        private PagingViewModel calcMasterPurOrderPaging(List<PurchaseOrderViewModel.MasterPurchaseOrderModels> tempList, [Optional] int currentPage)
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
                paging.startItemIndex = start -1;
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

        private List<PurchaseOrderViewModel.MasterPurchaseOrderModels> getMasterPurOrderByPaging(List<PurchaseOrderViewModel.MasterPurchaseOrderModels> tempList, int startRowIndex, int endRowIndex)
        {
            List<PurchaseOrderViewModel.MasterPurchaseOrderModels> list = new List<PurchaseOrderViewModel.MasterPurchaseOrderModels>();
            PurchaseOrderViewModel.MasterPurchaseOrderModels item = new PurchaseOrderViewModel.MasterPurchaseOrderModels();
            for (int page = startRowIndex; page < tempList.Count(); page++)
            {
                if (page > endRowIndex) break;
                item = new PurchaseOrderViewModel.MasterPurchaseOrderModels();
                item.PurchaseOrderID = tempList[page].PurchaseOrderID;
                item.OrderDateTime = tempList[page].OrderDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.SupplierName = tempList[page].SupplierName;
                item.Total = tempList[page].Total;
                list.Add(item);
            }
            return list;
        }

        private List<PurchaseOrderViewModel.MasterPurchaseOrderModels> selectMasterPurOrder(bool isSearch,[Optional]DateTime fromDate, [Optional] DateTime toDate, [Optional] string userVoucherNo, [Optional] int supplierId)
        {
            List<PurchaseOrderViewModel.MasterPurchaseOrderModels> tempList = new List<PurchaseOrderViewModel.MasterPurchaseOrderModels>();
            PurchaseOrderViewModel.MasterPurchaseOrderModels item = new PurchaseOrderViewModel.MasterPurchaseOrderModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterPurchaseOrderList, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IsSearch", isSearch);
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", appSetting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", appSetting.getLocalDate());
                cmd.Parameters.AddWithValue("@UserVoucherNo", "");
                cmd.Parameters.AddWithValue("@SupplierID", 0);
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                cmd.Parameters.AddWithValue("@SupplierID", supplierId);
            }
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new PurchaseOrderViewModel.MasterPurchaseOrderModels();
                item.PurchaseOrderID = Convert.ToInt32(reader["PurchaseOrderID"]);
                item.OrderDateTime = Convert.ToString(reader["PurOrderDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.SupplierName = Convert.ToString(reader["SupplierName"]);
                item.Total = Convert.ToInt32(reader["Total"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterPurOrderData"] = tempList;
            return tempList;
        }

        private  MasterPurOrderViewModel selectMasterPurOrderByPurOrdeID(int purOrderId)
        {
            MasterPurOrderViewModel item = new MasterPurOrderViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterPurOrderByPurOrderID, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseOrderID", purOrderId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.UserName = Convert.ToString(reader["UserName"]);
                item.SupplierName = Convert.ToString(reader["SupplierName"]);
                item.LocationName = Convert.ToString(reader["LocationName"]);
                item.MasterPurchaseOrderModel.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.MasterPurchaseOrderModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.MasterPurchaseOrderModel.OrderDateTime = Convert.ToString(reader["Date"]);
                item.MasterPurchaseOrderModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.MasterPurchaseOrderModel.SupplierID = Convert.ToInt32(reader["SupplierID"]);
                item.MasterPurchaseOrderModel.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.MasterPurchaseOrderModel.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.MasterPurchaseOrderModel.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.MasterPurchaseOrderModel.Total = Convert.ToInt32(reader["Total"]);
            }
            reader.Close();
            return item;
        }

        private List<TranPurchaseOrderModels> selectTranPurOrderByPurOrderID(int purOrderId)
        {
            List<TranPurchaseOrderModels> list = new List<TranPurchaseOrderModels>();
            TranPurchaseOrderModels item = new TranPurchaseOrderModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranPurOrderByPurOrderID, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseOrderID", purOrderId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranPurchaseOrderModels();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.PurPrice = Convert.ToInt32(reader["PurPrice"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                list.Add(item);
            }
            reader.Close();
            return list;
        }

        private void clearTranPurchaseOrder() {
            Session["TranPurchaseOrderData"] = null;
        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.PurchaseOrderModule, userId, getConnection());
            return userVoucherNo;
        }

        private void getProduct(int subMenuId)
        {
            purchaseOrderViewModel.ProductMenus.Products = appData.selectProduct(getConnection(), getFirstSubMenuID());
            Session["ProductData"] = purchaseOrderViewModel.ProductMenus.Products;
        }

        private int getFirstSubMenuID()
        {
            int subMenuId = 0;
            if (purchaseOrderViewModel.ProductMenus.SubMenus.Count() != 0)
            {
                SubMenuModels.SubMenuModel firstSubMenu = purchaseOrderViewModel.ProductMenus.SubMenus.FirstOrDefault();
                subMenuId = firstSubMenu.SubMenuID;
            }
            return subMenuId;
        }

        private void getSubMenu(int mainMenuId)
        {
            purchaseOrderViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(),getFirstMainMenuID());
        }

        private int getFirstMainMenuID()
        {
            int mainMenuId = 0;
            if (purchaseOrderViewModel.ProductMenus.MainMenus.Count() != 0)
            {
                MainMenuModels.MainMenuModel firstMainMenu = purchaseOrderViewModel.ProductMenus.MainMenus.FirstOrDefault();
                mainMenuId = firstMainMenu.MainMenuID;
            }
            return mainMenuId; 
        }

        private void getMainMenu()
        {
            purchaseOrderViewModel.ProductMenus.MainMenus = appData.selectMainMenu(getConnection());
        }

        private List<CurrencyModels> getCurrency()
        {
            List<CurrencyModels> list = new List<CurrencyModels>();
            if (Session["CurrencyData"] == null)
            {
                list = appData.selectCurrency(getConnection());
                Session["CurrencyData"] = list;
            }
            else {
                list = Session["CurrencyData"] as List<CurrencyModels>;
            }

            return list;
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

        private void getSupplier(bool isIncludeDefault)
        {
            if (isIncludeDefault) purchaseOrderViewModel.Suppliers.Add(new SelectListItem {Text=AppConstants.AllSupplier, Value="0" });
            List<SupplierModels.SupplierModel> list = appData.selectSupplier(getConnection());
            for (int i = 0; i < list.Count(); i++)
            {
                purchaseOrderViewModel.Suppliers.Add(new SelectListItem { Text = list[i].SupplierName, Value = Convert.ToString(list[i].SupplierID) });
            }
        }

        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation(getConnection());
            for (int i = 0; i< list.Count(); i++)
            {
                purchaseOrderViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value= Convert.ToString(list[i].LocationID)});
            }
        }
        private bool checkConnection()
        {
            if (Session[AppConstants.SQLConnection] != null) return true;
            else return false;
        }

        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();
                connection = Session[AppConstants.SQLConnection];
            return connection;
        }
        #endregion
    }
}
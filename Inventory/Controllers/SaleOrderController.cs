using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Common;
using Inventory.Models;
using Inventory.ViewModels;
using System.Runtime.InteropServices;
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class SaleOrderController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();
        AppData appData = new AppData();
        TextQuery textQuery = new TextQuery();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();

        #region Page
        [SessionTimeoutAttribute]
        public ActionResult SaleOrder(int userId, int? saleOrderId)
        {
            if (checkConnection())
            {
                getLocation();
                getCustomer(false);
                getUnit();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                clearTranSaleOrder();
                int totalQuantity = 0;
                if (saleOrderId != null) // saleorder edit
                {
                    ViewBag.IsEdit = true;
                    MasterSaleOrderViewModel data = selectMasterSaleOrder((int)saleOrderId);
                    List<TranSaleOrderModels> lstTranSaleOrder = selectTranSaleOrderBySaleOrderID((int)saleOrderId);
                    for (int i = 0;i < lstTranSaleOrder.Count(); i++)
                    {
                        totalQuantity += lstTranSaleOrder[i].Quantity;
                    }
                    Session["TranSaleOrderData"] = lstTranSaleOrder;
                    ViewBag.TotalItem = lstTranSaleOrder.Count();
                    ViewBag.UserVoucherNo = data.MasterSaleOrderModel.UserVoucherNo;
                    DateTime date = setting.convertStringToDate(data.MasterSaleOrderModel.OrderDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterSaleOrderModel.VoucherID;
                    ViewBag.CustomerID = data.MasterSaleOrderModel.CustomerID;
                    ViewBag.LocationID = data.MasterSaleOrderModel.LocationID;
                    ViewBag.Subtotal = data.MasterSaleOrderModel.Subtotal;
                    ViewBag.TaxAmt = data.MasterSaleOrderModel.TaxAmt;
                    ViewBag.ChargesAmt = data.MasterSaleOrderModel.ChargesAmt;
                    ViewBag.Total = data.MasterSaleOrderModel.Total;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.SaleOrderID = saleOrderId;
                }

                return View(saleOrderViewModel);
            }
            return RedirectToAction("Login", "User");          
        }

        [SessionTimeoutAttribute]
        public ActionResult ListSaleOrder()
        {
            getCustomer(true);
            List<SaleOrderViewModel.MasterSaleOrderViewModel> tempList = selectMasterSaleOrder(false);
            PagingViewModel pagingViewModel = calcMasterSaleOrderPaging(tempList);
            List<SaleOrderViewModel.MasterSaleOrderViewModel> lstMasterSaleOrder = getMasterSaleOrderByPaging(tempList, pagingViewModel.StartItemIndex,pagingViewModel.EndItemIndex);
            ViewData["LstMasterSaleOrder"] = lstMasterSaleOrder;
            ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
            ViewBag.CurrentPage = pagingViewModel.CurrentPage;
            return View(saleOrderViewModel);
        }

        #endregion

        #region SaleOrderAction
        public JsonResult TranSaleOrderAddEditAction(int productId, string productCode, string productName, int quantity, int price, int? unitId, string unitKeyword, int? currencyId, string currencyKeyword, bool isEdit, int? number, bool isItemFOC)
        {
            List<TranSaleOrderModels> lstTranSaleOrder = new List<TranSaleOrderModels>();
            TranSaleOrderModels data = new TranSaleOrderModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int subtotal = 0, totalQuantity = 0;
            data.Number = number;
            data.ProductID = productId;
            data.ProductName = productName;
            data.ProductCode = productCode;
            data.Quantity = quantity;
            data.SalePrice = price;
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
                if (Session["TranSaleOrderData"] != null)
                {

                    lstTranSaleOrder = Session["TranSaleOrderData"] as List<TranSaleOrderModels>;
                    lstTranSaleOrder.Add(data);
                }
                else
                {
                    lstTranSaleOrder.Add(data);

                }
                resultDefaultData.IsRequestSuccess = true;
            }
            else {
                if (Session["TranSaleOrderData"] != null)
                {
                    try
                    {
                        resultDefaultData.IsRequestSuccess = true;
                        lstTranSaleOrder = Session["TranSaleOrderData"] as List<TranSaleOrderModels>;
                        int Index = (int)number - 1;
                        lstTranSaleOrder[Index] = data;
                    }
                    catch (Exception ex)
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                        resultDefaultData.Message = ex.Message;
                    }

                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }

            for (int i = 0; i < lstTranSaleOrder.Count(); i++)
            {
                subtotal += lstTranSaleOrder[i].Amount;
                totalQuantity += lstTranSaleOrder[i].Quantity;

            }

            Session["TranSaleOrderData"] = lstTranSaleOrder;
            var result = new
            {
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                ResultDefaultData = resultDefaultData,
                LstTranSaleOrder = lstTranSaleOrder
            };
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrepareToEditTranSaleAction(int number, bool isMultiUnit, bool isMultiCurrency)
        {
            List<TranSaleOrderModels> lstTranSaleOrder = new List<TranSaleOrderModels>();
            TranSaleOrderModels data = new TranSaleOrderModels();
            string productCode = "", productName = "";
            int productId = 0, quantity = 0, price = 0;
            int? unitId = 0, currencyId = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            bool isFOC = false;
            if (Session["TranSaleOrderData"] != null)
            {
                try
                {
                    lstTranSaleOrder = Session["TranSaleOrderData"] as List<TranSaleOrderModels>;
                    if (lstTranSaleOrder.Count() != 0)
                    {
                        data = lstTranSaleOrder[number - 1];
                        if (data != null)
                        {
                            productId = data.ProductID;
                            productCode = data.ProductCode;
                            productName = data.ProductName;
                            quantity = data.Quantity;
                            price = data.SalePrice;
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

            var result = new
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

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchAction(int userId, DateTime fromDate, DateTime toDate, string userVoucherNo, int customerId)
        {
            List<SaleOrderViewModel.MasterSaleOrderViewModel> tempList = selectMasterSaleOrder(true, fromDate, toDate, userVoucherNo, customerId);
            PagingViewModel pagingViewModel = calcMasterSaleOrderPaging(tempList);
            List<SaleOrderViewModel.MasterSaleOrderViewModel> lstMasterSaleOrder = getMasterSaleOrderByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                LstMasterSaleOrder = lstMasterSaleOrder,
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaleOrderPagingAction(int currentPage)
        {
            List<SaleOrderViewModel.MasterSaleOrderViewModel> lstMasterSaleOrder = new List<SaleOrderViewModel.MasterSaleOrderViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["MasterSaleOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<SaleOrderViewModel.MasterSaleOrderViewModel> tempList = Session["MasterSaleOrderData"] as List<SaleOrderViewModel.MasterSaleOrderViewModel>;
                    pagingViewModel = calcMasterSaleOrderPaging(tempList, currentPage);
                    lstMasterSaleOrder = getMasterSaleOrderByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
                LstMasterSaleOrder = lstMasterSaleOrder,
                TotalPage = pagingViewModel.TotalPageNum,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshAction(int userId)
        {
            List<SaleOrderViewModel.MasterSaleOrderViewModel> tempList = selectMasterSaleOrder(false);
            PagingViewModel pagingViewModel = calcMasterSaleOrderPaging(tempList);
            List<SaleOrderViewModel.MasterSaleOrderViewModel> lstMasterSaleOrder = getMasterSaleOrderByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                LstMasterSaleOrder = lstMasterSaleOrder,
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ViewAction(int saleOrderId)
        {
            MasterSaleOrderViewModel item = selectMasterSaleOrder(saleOrderId);
            List<TranSaleOrderModels> lstTranSaleOrder = selectTranSaleOrderBySaleOrderID(saleOrderId);
            var jsonResult = new
            {
                LstTranSaleOrder = lstTranSaleOrder,
                UserVoucherNo = item.MasterSaleOrderModel.UserVoucherNo,
                VoucherID = item.MasterSaleOrderModel.VoucherID,
                LocationName = item.LocationName,
                SaleOrderDateTime = item.MasterSaleOrderModel.OrderDateTime,
                User = item.UserName,
                CustomerName = item.CustomerName,
                Subtotal = item.MasterSaleOrderModel.Subtotal,
                TaxAmt = item.MasterSaleOrderModel.TaxAmt,
                ChargesAmt = item.MasterSaleOrderModel.ChargesAmt,
                Total = item.MasterSaleOrderModel.Total,
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAction(int saleOrderId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            int totalPageNum = 0;
            if (Session["MasterSaleOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    SqlCommand cmd = new SqlCommand(textQuery.deleteSaleOrderQuery(saleOrderId), (SqlConnection)getConnection());
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    List<SaleOrderViewModel.MasterSaleOrderViewModel> lstMasterSaleOrder = Session["MasterSaleOrderData"] as List<SaleOrderViewModel.MasterSaleOrderViewModel>;
                    int index = lstMasterSaleOrder.FindIndex(x => x.SaleOrderID == saleOrderId);
                    lstMasterSaleOrder.RemoveAt(index);
                    if (lstMasterSaleOrder.Count > paging.eachItemCount)
                    {
                        totalPageNum = lstMasterSaleOrder.Count / paging.eachItemCount;
                        paging.lastItemCount = lstMasterSaleOrder.Count % paging.eachItemCount;
                        if (paging.lastItemCount != 0) totalPageNum += 1;
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
                TotalPage = totalPageNum,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSalePriceAction(int productId)
        {
            int price = appData.selectSalePriceByProduct(getConnection(), productId);
            return Json(price, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranSaleOrderDeleteAction(int number)
        {
            List<TranSaleOrderModels> lstTranSaleOrder = new List<TranSaleOrderModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int subtotal = 0, totalQuantity = 0;

            if (Session["TranSaleOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    lstTranSaleOrder = Session["TranSaleOrderData"] as List<TranSaleOrderModels>;
                    lstTranSaleOrder.RemoveAt(number - 1);
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }

            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            for (int i = 0; i< lstTranSaleOrder.Count(); i++)
            {
                subtotal += lstTranSaleOrder[i].Amount;
                totalQuantity += lstTranSaleOrder[i].Quantity;
            }

            Session["TranSaleOrderData"] = lstTranSaleOrder;

            var jsonResult = new
            {
                LstTranSaleOrder = lstTranSaleOrder,
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductByKeywordAction(string keyword)
        {
            List<ProductModels.ProductModel> list = appData.selectProductByKeyword(getConnection(), keyword);
            Session["SearchProductData"] = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductByCodeAction( string productCode, bool isMultiUnit, bool isMultiCurrency)
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
                price = data.SalePrice;
                if (isMultiUnit) lstUnit = getUnit();
                if (isMultiCurrency) lstCurrency = getCurrency();

            }
            else isExistProduct = false;
            var jsonResult = new {
                ProductID = productId,
                ProductName = productName,
                SalePrice = price,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsExistProduct = isExistProduct
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchProductClickAction(int productId, bool isMultiUnit, bool isMultiCurrency)
        {
            string productName = "", code = "";
            int salePrice = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["SearchProductData"] != null)
            {
                try
                {
                    List<ProductModels.ProductModel> list = Session["SearchProductData"] as List<ProductModels.ProductModel>;
                    var result = list.Where(c => c.ProductID == productId).SingleOrDefault();
                    if (result != null)
                    {
                        productName = result.ProductName;
                        code = result.Code;
                        salePrice = result.SalePrice;
                        if (isMultiUnit) lstUnit = getUnit();
                        if (isMultiCurrency) lstCurrency = getCurrency();
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

            var jsonResult = new {
                ProductName = productName,
                Code = code,
                SalePrice = salePrice,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelAction()
        {
            Session["TranSaleOrderData"] = null;
            return Json("",JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAction(string userVoucherNo, string date, string voucherId, int userId, int customerId, int locationId,
           int subtotal, int tax, int taxAmt, int charges, int chargesAmt, int total)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranSaleOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<TranSaleOrderModels> list = Session["TranSaleOrderData"] as List<TranSaleOrderModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("UnitID", typeof(int));
                    dt.Columns.Add("SalePrice", typeof(int));
                    dt.Columns.Add("CurrencyID", typeof(int));
                    dt.Columns.Add("Amount", typeof(int));
                    dt.Columns.Add("IsFOC", typeof(bool));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].SalePrice, list[i].CurrencyID, list[i].Amount, list[i].IsFOC);
                    }

                    DateTime saleOrderDateTime = DateTime.Parse(date);
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertSaleOrder, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@OrderDateTime", saleOrderDateTime);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Tax", tax);
                    cmd.Parameters.AddWithValue("@TaxAmt", taxAmt);
                    cmd.Parameters.AddWithValue("@Charges", charges);
                    cmd.Parameters.AddWithValue("@ChargesAmt", chargesAmt);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.SaleOrderModule);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) userVoucherNo = Convert.ToString(reader[0]);
                    reader.Close();
                    dataConnectorSQL.Close();
                    clearTranSaleOrder();
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

        [HttpGet]
        public JsonResult EditAction(int saleOrderId, string date, string voucherId, int customerId, int locationId, int taxAmt, int chargesAmt, int subtotal, int total)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranSaleOrderData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    List<TranSaleOrderModels> list = Session["TranSaleOrderData"] as List<TranSaleOrderModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("UnitID", typeof(int));
                    dt.Columns.Add("SalePrice", typeof(int));
                    dt.Columns.Add("CurrencyID", typeof(int));
                    dt.Columns.Add("Amount", typeof(int));
                    dt.Columns.Add("IsFOC", typeof(bool));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].SalePrice, list[i].CurrencyID, list[i].Amount, list[i].IsFOC);
                    }

                    DateTime saleOrderDateTime = DateTime.Parse(date);
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateSaleOrder, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@SaleOrderID", saleOrderId);
                    cmd.Parameters.AddWithValue("@OrderDateTime", saleOrderDateTime);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@TaxAmt", taxAmt);
                    cmd.Parameters.AddWithValue("@ChargesAmt", chargesAmt);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    dataConnectorSQL.Close();
                    clearTranSaleOrder();
                }
                catch (Exception ex) {
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

        #endregion SaleOrderAction

        #region Method
        private List<CurrencyModels> getCurrency()
        {
            List<CurrencyModels> list = new List<CurrencyModels>();
            if (Session["CurrencyData"] == null)
            {
                list = appData.selectCurrency(getConnection());
                Session["CurrencyData"] = list;
            }
            else list = Session["CurrencyData"] as List<CurrencyModels>;
            return list;
        }

        private List<SaleOrderViewModel.MasterSaleOrderViewModel> selectMasterSaleOrder(bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]string userVoucherNo, [Optional]int customerId)
        {
            List<SaleOrderViewModel.MasterSaleOrderViewModel> tempList = new List<SaleOrderViewModel.MasterSaleOrderViewModel>();
            SaleOrderViewModel.MasterSaleOrderViewModel item = new SaleOrderViewModel.MasterSaleOrderViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleOrderList, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IsSearch", isSearch);
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@UserVoucherNo", "");
                cmd.Parameters.AddWithValue("@CustomerID", 0);
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
            }
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new SaleOrderViewModel.MasterSaleOrderViewModel();
                item.SaleOrderID = Convert.ToInt32(reader["SaleOrderID"]);
                item.OrderDateTime = Convert.ToString(reader["SaleOrderDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.Total = Convert.ToInt32(reader["Total"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterSaleOrderData"] = tempList;
            return (tempList);
        }

        private MasterSaleOrderViewModel selectMasterSaleOrder(int saleOrderId)
        {
            MasterSaleOrderViewModel item = new MasterSaleOrderViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleOrderBySaleOrderID, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleOrderID", saleOrderId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.MasterSaleOrderModel.OrderDateTime = Convert.ToString(reader["Date"]);
                item.MasterSaleOrderModel.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.MasterSaleOrderModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.MasterSaleOrderModel.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                item.MasterSaleOrderModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.LocationName = Convert.ToString(reader["LocationName"]);
                item.MasterSaleOrderModel.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.MasterSaleOrderModel.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.MasterSaleOrderModel.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.MasterSaleOrderModel.Total = Convert.ToInt32(reader["Total"]);
            }

            reader.Close();

            return item;
        }

        private List<TranSaleOrderModels> selectTranSaleOrderBySaleOrderID(int saleOrderId)
        {
            List<TranSaleOrderModels> list = new List<TranSaleOrderModels>();
            TranSaleOrderModels item = new TranSaleOrderModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranSaleOrderBySaleOrderID,(SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleOrderID", saleOrderId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item =  new TranSaleOrderModels();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
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

        private PagingViewModel calcMasterSaleOrderPaging(List<SaleOrderViewModel.MasterSaleOrderViewModel> tempList, [Optional] int currentPage)
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

        private List<SaleOrderViewModel.MasterSaleOrderViewModel> getMasterSaleOrderByPaging(List<SaleOrderViewModel.MasterSaleOrderViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<SaleOrderViewModel.MasterSaleOrderViewModel> list = new List<SaleOrderViewModel.MasterSaleOrderViewModel>();
            SaleOrderViewModel.MasterSaleOrderViewModel item = new SaleOrderViewModel.MasterSaleOrderViewModel();
            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;
                item = new SaleOrderViewModel.MasterSaleOrderViewModel();
                item.SaleOrderID = tempList[page].SaleOrderID;
                item.OrderDateTime = tempList[page].OrderDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.CustomerName = tempList[page].CustomerName;
                item.Total = tempList[page].Total;
                list.Add(item);
            }
            return list;
        }

        private void clearTranSaleOrder()
        {
            Session["TranSaleOrderData"] = null;
        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.SaleOrderModule, userId, getConnection());
            return userVoucherNo;
        }

        private void getProduct(int subMenuId)
        {
            saleOrderViewModel.ProductMenus.Products = appData.selectProduct(getConnection(), subMenuId);
            Session["ProductData"] = saleOrderViewModel.ProductMenus.Products;
        }

        private int getFirstSubMenuID()
        {
            int subMenuId = 0;
            if (saleOrderViewModel.ProductMenus.SubMenus.Count() != 0)
            {
                SubMenuModels.SubMenuModel firstSubMenu = saleOrderViewModel.ProductMenus.SubMenus.First();
                subMenuId = firstSubMenu.SubMenuID;
            }
            return subMenuId;
        }

        private void getSubMenu(int mainMenuId)
        {
            saleOrderViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(), mainMenuId);
        }

        private int getFirstMainMenuID()
        {
            int mainMenuId = 0;
            if (saleOrderViewModel.ProductMenus.MainMenus.Count() != 0)
            {
                MainMenuModels.MainMenuModel firstMainMenu = saleOrderViewModel.ProductMenus.MainMenus.First();
                mainMenuId = firstMainMenu.MainMenuID;
            }
            return mainMenuId;
        }

        private void getMainMenu()
        {
            saleOrderViewModel.ProductMenus.MainMenus = appData.selectMainMenu(getConnection());
        }

        private bool checkConnection()
        {
            if (Session[AppConstants.SQLConnection] != null) return true;
            else return false;
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
            for (int i = 0; i< list.Count; i++)
            {
                saleOrderViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
        }

        private void getCustomer(bool isIncludeDefault)
        {
            if (isIncludeDefault) saleOrderViewModel.Customers.Add(new SelectListItem { Text = AppConstants.AllCustomer, Value = "0" });
            List < CustomerModels.CustomerModel > list = appData.selectCustomer(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                saleOrderViewModel.Customers.Add(new SelectListItem { Text = list[i].CustomerName, Value = Convert.ToString(list[i].CustomerID)});
            }
        }

        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();
            connection = Session[AppConstants.SQLConnection];
            return connection;
        }

        #endregion Method

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;
using Inventory.ViewModels;
using System.Runtime.InteropServices;
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class SaleController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        SaleViewModel saleViewModel = new SaleViewModel();
        AppData appData = new AppData();
        TextQuery textQuery = new TextQuery();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();

        #region Page

        [SessionTimeoutAttribute]
        public ActionResult POS(int userId, int? saleId, int? openBillId, int? clSaleOrderId)
        {
            if (checkConnection())
            {
                int totalQuantity = 0;
                getCustomer(false);
                getLocation();
                getMCurrency();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                clearTranSale();

                if (saleId != null)  // sale edit
                {
                    ViewBag.IsEdit = true;
                    MasterSaleVoucherViewModel data = selectMasterSale((int)saleId);
                    List<TranSaleModels> lstTranSale = selectTranSaleBySaleID((int)saleId);
                    for (int i = 0; i < lstTranSale.Count(); i++)
                    {
                        totalQuantity += lstTranSale[i].Quantity;
                    }
                    Session["TranSaleData"] = lstTranSale;                  
                    ViewBag.TotalItem = lstTranSale.Count();
                    ViewBag.UserVoucherNo = data.MasterSaleModel.UserVoucherNo;
                    DateTime date = setting.convertStringToDate(data.MasterSaleModel.SaleDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterSaleModel.VoucherID;
                    ViewBag.CustomerID = data.MasterSaleModel.CustomerID;
                    ViewBag.LocationID = data.MasterSaleModel.LocationID;
                    ViewBag.CurrencyID = data.MasterSaleModel.CurrencyID;
                    ViewBag.Subtotal = data.MasterSaleModel.Subtotal;
                    ViewBag.TaxAmt = data.MasterSaleModel.TaxAmt;
                    ViewBag.ChargesAmt = data.MasterSaleModel.ChargesAmt;
                    ViewBag.Total = data.MasterSaleModel.Total;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.SaleID = saleId;
                }
                else if (openBillId != null)  // add sale from open bill
                {
                    ViewBag.IsFromOpenBill = true;
                    SaleViewModel.MasterOpenBillViewModel data = selectMasterOpenBill((int)openBillId);
                    List<TranSaleModels> lstTranSale = selectTranOpenBillByID((int)openBillId);
                    for (int i = 0; i < lstTranSale.Count(); i++)
                    {
                        totalQuantity += lstTranSale[i].Quantity;
                    }
                    Session["TranSaleData"] = lstTranSale;                   
                    ViewBag.TotalItem = lstTranSale.Count();
                    ViewBag.UserVoucherNo = data.UserVoucherNo;
                    ViewBag.VoucherID = data.VoucherID;
                    ViewBag.CustomerID = data.CustomerID;
                    ViewBag.LocationID = data.LocationID;
                    ViewBag.CurrencyID = data.CurrencyID;
                    ViewBag.Subtotal = data.Subtotal;
                    ViewBag.TaxAmt = data.TaxAmt;
                    ViewBag.ChargesAmt = data.ChargesAmt;
                    ViewBag.Total = data.Total;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.OpenBillID = openBillId;
                }
                else if (clSaleOrderId != null)  // add sale from client sale order
                {
                    ViewBag.IsFromCLSaleOrder = true;
                    SaleViewModel.CLMasterSaleOrderViewModel data = selectCLMasterSaleOrder((int)clSaleOrderId);
                    List<TranSaleModels> lstTranSale = selectCLTranSaleOrderByID((int)clSaleOrderId);
                    for (int i = 0; i < lstTranSale.Count(); i++)
                    {
                        totalQuantity += lstTranSale[i].Quantity;
                    }
                    Session["TranSaleData"] = lstTranSale;
                    ViewBag.TotalItem = lstTranSale.Count();                 
                    ViewBag.CustomerID = data.CustomerID;                  
                    ViewBag.Subtotal = data.Subtotal;
                    ViewBag.TaxAmt = data.TaxAmt;
                    ViewBag.ChargesAmt = data.ChargesAmt;
                    ViewBag.Total = data.Total;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.CLSaleOrderID = clSaleOrderId;
                    ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                }
                else ViewBag.UserVoucherNo = getUserVoucherNo(userId); // new sale

                return View(saleViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        [SessionTimeoutAttribute]
        public ActionResult ListSale(int userId)
        {
            if (checkConnection())
            {
                getCustomer(true);
                List<SaleViewModel.MasterSaleViewModel> tempList = selectMasterSale(userId,false);
                PagingViewModel pagingViewModel = calcMasterSalePaging(tempList);
                List<SaleViewModel.MasterSaleViewModel> lstMasterSale = getMasterSaleByPaging(tempList,pagingViewModel.StartItemIndex,pagingViewModel.EndItemIndex);
                ViewData["LstMasterSale"] = lstMasterSale;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
                return View(saleViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        [SessionTimeoutAttribute]
        public ActionResult ListOpenBill(int userId)
        {
            if (checkConnection())
            {
                getCustomer(true);
                List<SaleViewModel.MasterOpenBillViewModel> tempList = selectMasterOpenBill(userId, false);
                PagingViewModel pagingViewModel = calcMasterOpenBillPaging(tempList);
                List<SaleViewModel.MasterOpenBillViewModel> lstMasterOpenBill = getMasterOpenBillByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstMasterOpenBill"] = lstMasterOpenBill;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
                return View(saleViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        [SessionTimeoutAttribute]
        public ActionResult SaleVoucher(string systemVoucherNo,int locationId)
        {
            VoucherSettingModels voucherSettingModel = appData.selectVoucherSettingByLocation(getConnection(), locationId);
            saleViewModel.VoucherSettings = voucherSettingModel;
            ViewBag.Base64Photo = voucherSettingModel.Base64Photo;
            MasterSaleVoucherViewModel masterSaleVoucherViewModel = selectMasterSale(systemVoucherNo);
            setMasterSaleDataToViewBag(masterSaleVoucherViewModel);           
            List<TranSaleModels> lstTranSale = selectTranSaleBySaleID(masterSaleVoucherViewModel.MasterSaleModel.SaleID);                    
            ViewData["LstTranSale"]= lstTranSale;         
            return View(saleViewModel);
        }

        #endregion

        #region SaleAction     

        [HttpGet]
        public JsonResult TranSaleAddEditAction(int productId, string productCode, string productName, int quantity, int price, int disPercent, int? unitId, string unitKeyword, int? currencyId, string currencyKeyword, bool isEdit, int? number, bool isItemFOC)
        {
            List<TranSaleModels> lstTranSale = new List<TranSaleModels>();
            TranSaleModels data = new TranSaleModels();
            int subtotal = 0, totalQuantity = 0, discount;
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            data.Number = number;
            data.ProductID = productId;
            data.ProductCode = productCode;
            data.Quantity = quantity;
            data.SalePrice = price;
            data.DiscountPercent = disPercent;
            data.UnitID = unitId;
            data.CurrencyID = currencyId;
            data.ProductName = productName;
            if (unitKeyword != null) data.UnitKeyword = unitKeyword;
            else data.UnitKeyword = "";
            if (currencyKeyword != null) data.CurrencyKeyword = currencyKeyword;
            else data.CurrencyKeyword = "";
            discount = ((price * quantity) * disPercent) / 100;
            data.Discount = discount;
            data.Amount = (quantity * price) - discount;
            data.IsFOC = isItemFOC;

            if (!isEdit)
            {
                if (Session["TranSaleData"] != null)
                {
                    lstTranSale = Session["TranSaleData"] as List<TranSaleModels>;
                    lstTranSale.Add(data);
                }
                else lstTranSale.Add(data);
                resultDefaultData.IsRequestSuccess = true;
            }
            else
            {
                if (Session["TranSaleData"] != null)
                {
                    try
                    {
                        lstTranSale = Session["TranSaleData"] as List<TranSaleModels>;
                        int index = (int)number - 1;
                        lstTranSale[index] = data;
                        resultDefaultData.IsRequestSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                        resultDefaultData.Message = ex.Message;
                    }
                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }

            for (int i = 0; i < lstTranSale.Count(); i++)
            {
                subtotal += lstTranSale[i].Amount;
                totalQuantity += lstTranSale[i].Quantity;
            }

            Session["TranSaleData"] = lstTranSale;

            var jsonResult = new
            {
                LstTranSale = lstTranSale,
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TranSaleDeleteAction(int number)
        {
            List<TranSaleModels> lstTranSale = new List<TranSaleModels>();
            int subtotal = 0, totalQuantity = 0;
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranSaleData"] != null)
            {
                try
                {
                    lstTranSale = Session["TranSaleData"] as List<TranSaleModels>;
                    lstTranSale.RemoveAt(number - 1);
                    resultDefaultData.IsRequestSuccess = true;
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            for (int i = 0; i < lstTranSale.Count(); i++)
            {
                subtotal += lstTranSale[i].Amount;
                totalQuantity += lstTranSale[i].Quantity;
            }

            Session["TranSaleData"] = lstTranSale;

            var jsonResult = new
            {
                LstTranSale = lstTranSale,
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CancelAction()
        {
            Session["TranSaleData"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PrepareToEditTranSaleAction(int number, bool isMultiUnit, bool isMultiCurrency)
        {
            List<TranSaleModels> lstTranSale = new List<TranSaleModels>();
            TranSaleModels data = new TranSaleModels();
            string productCode = "", productName = "";
            int productId = 0, quantity = 0, price = 0, disPercent = 0;
            int? unitId = 0, currencyId = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isFOC = false;
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranSaleData"] != null)
            {
                try
                {
                    lstTranSale = Session["TranSaleData"] as List<TranSaleModels>;

                    if (lstTranSale.Count() != 0)
                    {
                        data = lstTranSale[number - 1];
                        if (data != null)
                        {
                            productId = data.ProductID;
                            productCode = data.ProductCode;
                            productName = data.ProductName;
                            quantity = data.Quantity;
                            price = data.SalePrice;
                            unitId = data.UnitID;
                            currencyId = data.CurrencyID;
                            disPercent = data.DiscountPercent;
                            if (isMultiUnit) lstUnit = getUnit();
                            if (isMultiCurrency) lstCurrency = getCurrency();
                            isFOC = data.IsFOC;
                        }
                    }
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
                ProductID = productId,
                ProductCode = productCode,
                ProductName = productName,
                Quantity = quantity,
                Price = price,
                UnitID = unitId,
                CurrencyID = currencyId,
                DisPercent = disPercent,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsFOC = isFOC,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProductByCodeAction(string productCode, bool isMultiUnit, bool isMultiCurrency)
        {
            ProductModels.ProductModel data = new ProductModels.ProductModel();            
            string productName = "";
            int productId = 0, price = 0;
            short? disPercent = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isExistProduct = true;

            data = appData.selectProductByCode(getConnection(), productCode);
            if (data.ProductID != 0)
            {
                productId = data.ProductID;
                productName = data.ProductName;
                price = data.SalePrice;
                disPercent = data.DisPercent;
                if (isMultiUnit) lstUnit = getUnit();
                if (isMultiCurrency) lstCurrency = getCurrency();
            }
            else isExistProduct = false;

            var jsonResult = new
            {
                ProductID = productId,
                ProductName = productName,
                SalePrice = price,
                DisPercent = disPercent,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsExistProduct = isExistProduct
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProductByKeywordAction(string keyword)
        {
            List<ProductModels.ProductModel> list = appData.selectProductByKeyword(getConnection(), keyword);
            Session["SearchProductData"] = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchProductClickAction(int productId, bool isMultiUnit, bool isMultiCurrency)
        {
            string productName = "", code = "";
            int salePrice = 0;
            short? disPercent = 0;
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
                        disPercent = result.DisPercent;
                        if (isMultiUnit) lstUnit = getUnit();
                        if (isMultiCurrency) lstCurrency = getCurrency();
                    }
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
                SalePrice = salePrice,
                DisPercent = disPercent,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PaymentEditAction(int saleId, string date, string voucherId, int customerId, int locationId, int taxAmt, int chargesAmt, int subtotal, int total, int currencyId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranSaleData"] != null)
            {
                try
                {
                    List<TranSaleModels> list = Session["TranSaleData"] as List<TranSaleModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                    dt.Columns.Add(new DataColumn("SalePrice", typeof(int)));
                    dt.Columns.Add(new DataColumn("CurrencyID", typeof(int)));
                    dt.Columns.Add(new DataColumn("DiscountPercent", typeof(int)));
                    dt.Columns.Add(new DataColumn("Discount", typeof(int)));
                    dt.Columns.Add(new DataColumn("Amount", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsFOC", typeof(bool)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].SalePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                    }

                    DateTime saleDateTime = DateTime.Parse(date);
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateSale, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@SaleID", saleId);
                    cmd.Parameters.AddWithValue("@SaleDateTime", saleDateTime);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@CurrencyID", currencyId);
                    cmd.Parameters.AddWithValue("@TaxAmt", taxAmt);
                    cmd.Parameters.AddWithValue("@ChargesAmt", chargesAmt);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    dataConnectorSQL.Close();
                    clearTranSale();
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
        public JsonResult PaymentSubmitAction(string userVoucherNo, string date, string voucherId, int customerId, int locationId,
                int paymentId, int? payMethodId, int? limitedDayId, int? bankPaymentId, string remark, int? advancedPay,
                int? payPercent, int? payPercentAmt, int? vouDisPercent, int? vouDisAmount, int? voucherDiscount,
                int tax, int taxAmt, int charges, int chargesAmt, int subtotal, int total, int grandtotal, int userId, int? openBillId, int? clSaleOrderId, bool isVoucherFOC, int currencyId)
        {
            string systemVoucherNo = "";
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranSaleData"] != null)
            {
                try
                {
                    List<TranSaleModels> list = Session["TranSaleData"] as List<TranSaleModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                    dt.Columns.Add(new DataColumn("SalePrice", typeof(int)));
                    dt.Columns.Add(new DataColumn("CurrencyID", typeof(int)));
                    dt.Columns.Add(new DataColumn("DiscountPercent", typeof(int)));
                    dt.Columns.Add(new DataColumn("Discount", typeof(int)));
                    dt.Columns.Add(new DataColumn("Amount", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsFOC", typeof(bool)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].SalePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                    }

                    DateTime saleDateTime = DateTime.Parse(date);
                    if (payMethodId == null) payMethodId = 0;
                    if (limitedDayId == null) limitedDayId = 0;
                    if (bankPaymentId == null) bankPaymentId = 0;
                    if (advancedPay == null) advancedPay = 0;
                    if (payPercent == null) payPercent = 0;
                    if (payPercentAmt == null) payPercentAmt = 0;
                    if (vouDisPercent == null) vouDisPercent = 0;
                    if (vouDisAmount == null) vouDisAmount = 0;
                    if (voucherDiscount == null) voucherDiscount = 0;
                    if (openBillId == null) openBillId = 0;
                    if (clSaleOrderId == null) clSaleOrderId = 0;

                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertSale, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@SaleDateTime", saleDateTime);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@CurrencyID", currencyId);
                    cmd.Parameters.AddWithValue("@PaymentID", paymentId);
                    cmd.Parameters.AddWithValue("@VoucherDiscount", voucherDiscount);
                    cmd.Parameters.AddWithValue("@AdvancedPay", advancedPay);
                    cmd.Parameters.AddWithValue("@Tax", tax);
                    cmd.Parameters.AddWithValue("@TaxAmt", taxAmt);
                    cmd.Parameters.AddWithValue("@Charges", charges);
                    cmd.Parameters.AddWithValue("@ChargesAmt", chargesAmt);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@Grandtotal", grandtotal);
                    cmd.Parameters.AddWithValue("@VouDisPercent", vouDisPercent);
                    cmd.Parameters.AddWithValue("@VouDisAmount", vouDisAmount);
                    cmd.Parameters.AddWithValue("@PayMethodID", payMethodId);
                    cmd.Parameters.AddWithValue("@BankPaymentID", bankPaymentId);
                    cmd.Parameters.AddWithValue("@PaymentPercent", payPercent);
                    cmd.Parameters.AddWithValue("@IsClientSale", 0);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@LimitedDayID", limitedDayId);
                    cmd.Parameters.AddWithValue("@PayPercentAmt", payPercentAmt);
                    cmd.Parameters.AddWithValue("@Remark", remark);
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.SaleModule);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@OpenBillID", openBillId);
                    cmd.Parameters.AddWithValue("@CLSaleOrderID", clSaleOrderId);
                    cmd.Parameters.AddWithValue("@IsVoucherFOC", isVoucherFOC);
                    cmd.Parameters.AddWithValue("@AccountCode", AppConstants.SaleAccountCode);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) systemVoucherNo = Convert.ToString(reader[0]);
                    reader.Close();
                    dataConnectorSQL.Close();
                    clearTranSale();
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
                SystemVoucherNo = systemVoucherNo,
                LocationID = locationId,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(int userId,DateTime fromDate,DateTime toDate,string userVoucherNo,int customerId)
        {
            List<SaleViewModel.MasterSaleViewModel> tempList = selectMasterSale(userId, true, fromDate, toDate, userVoucherNo, customerId);
            PagingViewModel pagingViewModel = calcMasterSalePaging(tempList);
            List<SaleViewModel.MasterSaleViewModel> lstMasterSale = getMasterSaleByPaging(tempList,pagingViewModel.StartItemIndex,pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage=pagingViewModel.CurrentPage,
                LstMasterSale = lstMasterSale
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RefreshAction(int userId)
        {            
            List<SaleViewModel.MasterSaleViewModel> tempList = selectMasterSale(userId, false);
            PagingViewModel pagingViewModel = calcMasterSalePaging(tempList);
            List<SaleViewModel.MasterSaleViewModel> lstMasterSale = getMasterSaleByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterSale = lstMasterSale
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ViewAction(int saleId)
        {
            MasterSaleVoucherViewModel item = selectMasterSale(saleId);
            List<TranSaleModels> lstTranSale = selectTranSaleBySaleID(saleId);
            int grandTotalNotPayPercent = 0;
                   
            if (item.MasterSaleModel.PayPercentAmt != 0)
                grandTotalNotPayPercent = item.MasterSaleModel.Total - (item.MasterSaleModel.VoucherDiscount + item.MasterSaleModel.AdvancedPay);

            var jsonResult = new
            {
                LstTranSale = lstTranSale,
                UserVoucherNo = item.MasterSaleModel.UserVoucherNo,
                VoucherID = item.MasterSaleModel.VoucherID,
                Remark = item.MasterSaleModel.Remark,
                LocationName = item.LocationName,
                CurrencyKeyword = item.CurrencyKeyword,
                Payment = item.Payment,
                PayMethod = item.PayMethod,
                BankPayment = item.BankPayment,
                LimitedDay = item.LimitedDay,
                SaleDateTime = item.MasterSaleModel.SaleDateTime,
                UserName = item.UserName,
                CustomerName = item.CustomerName,
                SlipID = item.MasterSaleModel.SlipID,
                Subtotal = item.MasterSaleModel.Subtotal,
                TaxAmt = item.MasterSaleModel.TaxAmt,
                ChargesAmt = item.MasterSaleModel.ChargesAmt,
                Total = item.MasterSaleModel.Total,
                VoucherDiscount = item.MasterSaleModel.VoucherDiscount,
                AdvancedPay = item.MasterSaleModel.AdvancedPay,
                PayPercentAmt = item.MasterSaleModel.PayPercentAmt,
                Grandtotal = item.MasterSaleModel.Grandtotal,
                VouDisPercent = item.MasterSaleModel.VouDisPercent,
                PaymentPercent = item.MasterSaleModel.PaymentPercent,
                GrandTotalNotPayPercent = grandTotalNotPayPercent,
                IsVouFOC = item.MasterSaleModel.IsVouFOC
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int saleId)
        {           
            int totalPageNum = 0;          
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["MasterSaleData"] != null)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(textQuery.deleteSaleQuery(saleId), (SqlConnection)getConnection());
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    List<SaleViewModel.MasterSaleViewModel> lstMasterSale = Session["MasterSaleData"] as List<SaleViewModel.MasterSaleViewModel>;
                    int index = lstMasterSale.FindIndex(x => x.SaleID == saleId);
                    lstMasterSale.RemoveAt(index);

                    if (lstMasterSale.Count > paging.eachItemCount)
                    {
                        totalPageNum = lstMasterSale.Count / paging.eachItemCount;
                        paging.lastItemCount = lstMasterSale.Count % paging.eachItemCount;
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

        [HttpPost]
        public JsonResult HoldAction(string userVoucherNo, string date, string voucherId, int customerId, int locationId,
                string remark, int taxAmt, int chargesAmt, int subtotal, int total, int userId, int currencyId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranSaleData"] != null)
            {
                try
                {
                    List<TranSaleModels> list = Session["TranSaleData"] as List<TranSaleModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                    dt.Columns.Add(new DataColumn("SalePrice", typeof(int)));
                    dt.Columns.Add(new DataColumn("CurrencyID", typeof(int)));
                    dt.Columns.Add(new DataColumn("DiscountPercent", typeof(int)));
                    dt.Columns.Add(new DataColumn("Discount", typeof(int)));
                    dt.Columns.Add(new DataColumn("Amount", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsFOC", typeof(bool)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].SalePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                    }

                    DateTime openDateTime = DateTime.Parse(date);

                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertOpenBill, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@OpenDateTime", openDateTime);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@CurrencyID", currencyId);
                    cmd.Parameters.AddWithValue("@TaxAmt", taxAmt);
                    cmd.Parameters.AddWithValue("@ChargesAmt", chargesAmt);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Remark", remark);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@IsClientSale", 0);
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.SaleModule);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) userVoucherNo = Convert.ToString(reader[0]);
                    reader.Close();
                    dataConnectorSQL.Close();
                    clearTranSale();
                    resultDefaultData.IsRequestSuccess = true;
                    resultDefaultData.Message = AppConstants.Message.SaveSuccessOpenBill;
                }
                catch(Exception ex)
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
        public JsonResult SalePagingAction(int currentPage)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SaleViewModel.MasterSaleViewModel> lstMasterSale = new List<SaleViewModel.MasterSaleViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["MasterSaleData"] != null)
            {
                try
                {
                    List<SaleViewModel.MasterSaleViewModel> tempList = Session["MasterSaleData"] as List<SaleViewModel.MasterSaleViewModel>;
                    pagingViewModel = calcMasterSalePaging(tempList, currentPage);
                    lstMasterSale = getMasterSaleByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
                LstMasterSale = lstMasterSale,
                TotalPage = pagingViewModel.TotalPageNum,
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

        #endregion

        #region OpenBillAction

        [HttpGet]
        public JsonResult SearchOpenBillAction(int userId, DateTime fromDate, DateTime toDate, string userVoucherNo, int customerId)
        {
            List<SaleViewModel.MasterOpenBillViewModel> tempList = selectMasterOpenBill(userId, true, fromDate, toDate, userVoucherNo, customerId);
            PagingViewModel pagingViewModel = calcMasterOpenBillPaging(tempList);
            List<SaleViewModel.MasterOpenBillViewModel> lstMasterOpenBill = getMasterOpenBillByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterOpenBill = lstMasterOpenBill
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RefreshOpenBillAction(int userId)
        {
            List<SaleViewModel.MasterOpenBillViewModel> tempList = selectMasterOpenBill(userId, false);
            PagingViewModel pagingViewModel = calcMasterOpenBillPaging(tempList);
            List<SaleViewModel.MasterOpenBillViewModel> lstMasterOpenBill = getMasterOpenBillByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterOpenBill = lstMasterOpenBill
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteOpenBillAction(int openBillId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalPageNum = 0;

            if (Session["MasterOpenBillData"] != null)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(textQuery.deleteOpenBillQuery(openBillId), (SqlConnection)getConnection());
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    List<SaleViewModel.MasterOpenBillViewModel> lstMasterOpenBill = Session["MasterOpenBillData"] as List<SaleViewModel.MasterOpenBillViewModel>;
                    int index = lstMasterOpenBill.FindIndex(x => x.OpenBillID == openBillId);
                    lstMasterOpenBill.RemoveAt(index);

                    if (lstMasterOpenBill.Count > paging.eachItemCount)
                    {
                        totalPageNum = lstMasterOpenBill.Count / paging.eachItemCount;
                        paging.lastItemCount = lstMasterOpenBill.Count % paging.eachItemCount;
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
        public JsonResult OpenBillPagingAction(int currentPage)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SaleViewModel.MasterOpenBillViewModel> lstMasterOpenBill = new List<SaleViewModel.MasterOpenBillViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["MasterOpenBillData"] != null)
            {
                try
                {
                    List<SaleViewModel.MasterOpenBillViewModel> tempList = Session["MasterOpenBillData"] as List<SaleViewModel.MasterOpenBillViewModel>;
                    pagingViewModel = calcMasterOpenBillPaging(tempList, currentPage);
                    lstMasterOpenBill = getMasterOpenBillByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
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
                LstMasterOpenBill = lstMasterOpenBill,
                TotalPage = pagingViewModel.TotalPageNum,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Method

        private MasterSaleVoucherViewModel selectMasterSale(string systemVoucherNo)
        {
            MasterSaleVoucherViewModel item = new MasterSaleVoucherViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleBySystemVoucherNo, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SystemVoucherNo", systemVoucherNo);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.MasterSaleModel.SaleID = Convert.ToInt32(reader["SaleID"]);
                item.MasterSaleModel.SaleDateTime = Convert.ToString(reader["Date"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.MasterSaleModel.SlipID = Convert.ToInt32(reader["SlipID"]);
                item.MasterSaleModel.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.MasterSaleModel.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.MasterSaleModel.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.MasterSaleModel.Total = Convert.ToInt32(reader["Total"]);
                item.MasterSaleModel.VoucherDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                item.MasterSaleModel.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                item.MasterSaleModel.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                item.MasterSaleModel.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                item.MasterSaleModel.VouDisPercent = Convert.ToInt32(reader["VouDisPercent"]);
                item.MasterSaleModel.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);
                item.MasterSaleModel.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
            }
            reader.Close();         

            return item;
        }

        private MasterSaleVoucherViewModel selectMasterSale(int saleId)
        {
            MasterSaleVoucherViewModel item = new MasterSaleVoucherViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleBySaleID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleID", saleId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.MasterSaleModel.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.MasterSaleModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.MasterSaleModel.Remark = Convert.ToString(reader["Remark"]);
                item.LocationName = Convert.ToString(reader["LocationName"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                item.Payment = Convert.ToString(reader["Payment"]);
                item.PayMethod = Convert.ToString(reader["PayMethod"]);
                item.BankPayment = Convert.ToString(reader["BankPayment"]);
                item.LimitedDay = Convert.ToString(reader["LimitedDay"]);
                item.MasterSaleModel.SaleDateTime = Convert.ToString(reader["Date"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.MasterSaleModel.SlipID = Convert.ToInt32(reader["SlipID"]);
                item.MasterSaleModel.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.MasterSaleModel.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.MasterSaleModel.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.MasterSaleModel.Total = Convert.ToInt32(reader["Total"]);
                item.MasterSaleModel.VoucherDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                item.MasterSaleModel.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                item.MasterSaleModel.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                item.MasterSaleModel.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                item.MasterSaleModel.VouDisPercent = Convert.ToInt32(reader["VouDisPercent"]);
                item.MasterSaleModel.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);
                item.MasterSaleModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.MasterSaleModel.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.MasterSaleModel.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                item.MasterSaleModel.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
            }
            reader.Close();

            return item;
        }

        private List<SaleViewModel.MasterSaleViewModel> selectMasterSale(int userId, bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]string userVoucherNo, [Optional]int customerId)
        {           
            List<SaleViewModel.MasterSaleViewModel> tempList = new List<SaleViewModel.MasterSaleViewModel>();
            SaleViewModel.MasterSaleViewModel item = new SaleViewModel.MasterSaleViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
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
                item = new SaleViewModel.MasterSaleViewModel();
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                item.SaleDateTime = Convert.ToString(reader["SaleDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.PaymentKeyword = Convert.ToString(reader["PaymentKeyword"]);
                item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterSaleData"] = tempList;  // for paging

            return tempList;              
        }

        private PagingViewModel calcMasterSalePaging(List<SaleViewModel.MasterSaleViewModel> tempList, [Optional]int currentPage)
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

        private List<SaleViewModel.MasterSaleViewModel> getMasterSaleByPaging(List<SaleViewModel.MasterSaleViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<SaleViewModel.MasterSaleViewModel> list = new List<SaleViewModel.MasterSaleViewModel>();
            SaleViewModel.MasterSaleViewModel item = new SaleViewModel.MasterSaleViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new SaleViewModel.MasterSaleViewModel();
                item.SaleID = tempList[page].SaleID;
                item.SaleDateTime = tempList[page].SaleDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.CustomerName = tempList[page].CustomerName;
                item.PaymentKeyword = tempList[page].PaymentKeyword;
                item.Grandtotal = tempList[page].Grandtotal;
                list.Add(item);
            }
            return list;
        }

        private void setMasterSaleDataToViewBag(MasterSaleVoucherViewModel item)
        {
            ViewBag.SaleDateTime = item.MasterSaleModel.SaleDateTime;
            ViewBag.UserName = item.UserName;
            ViewBag.CustomerName = item.CustomerName;
            ViewBag.SlipID = item.MasterSaleModel.SlipID;
            ViewBag.Subtotal = item.MasterSaleModel.Subtotal;
            ViewBag.TaxAmt = item.MasterSaleModel.TaxAmt;
            ViewBag.ChargesAmt = item.MasterSaleModel.ChargesAmt;
            ViewBag.Total = item.MasterSaleModel.Total;
            ViewBag.VoucherDiscount = item.MasterSaleModel.VoucherDiscount;
            ViewBag.AdvancedPay = item.MasterSaleModel.AdvancedPay;
            ViewBag.PayPercentAmt = item.MasterSaleModel.PayPercentAmt;
            ViewBag.Grandtotal = item.MasterSaleModel.Grandtotal;
            ViewBag.VouDisPercent = item.MasterSaleModel.VouDisPercent;
            ViewBag.PaymentPercent = item.MasterSaleModel.PaymentPercent;
            if (item.MasterSaleModel.PayPercentAmt != 0)
                ViewBag.GrandTotalNotPayPercent = item.MasterSaleModel.Total - (item.MasterSaleModel.VoucherDiscount + item.MasterSaleModel.AdvancedPay);
            ViewBag.IsVouFOC = item.MasterSaleModel.IsVouFOC;
        }

        private List<TranSaleModels> selectTranSaleBySaleID(int saleId)
        {
            List<TranSaleModels> list = new List<TranSaleModels>();
            TranSaleModels item = new TranSaleModels();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranSaleBySaleID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleID", saleId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranSaleModels();              
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);              
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);             
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                item.ProductID= Convert.ToInt32(reader["ProductID"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                item.ProductCode= Convert.ToString(reader["Code"]);
                item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        private SaleViewModel.MasterOpenBillViewModel selectMasterOpenBill(int openBillId)
        {        
            SaleViewModel.MasterOpenBillViewModel item = new SaleViewModel.MasterOpenBillViewModel();

            SqlCommand cmd = new SqlCommand(textQuery.getMasterOpenBillQuery(openBillId), (SqlConnection)getConnection());
            cmd.CommandType = CommandType.Text;                          
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                item.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.Total = Convert.ToInt32(reader["Total"]);            
            }
            reader.Close();

            return item;
        }

        private List<SaleViewModel.MasterOpenBillViewModel> selectMasterOpenBill(int userId, bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]string userVoucherNo, [Optional]int customerId)
        {
            List<SaleViewModel.MasterOpenBillViewModel> tempList = new List<SaleViewModel.MasterOpenBillViewModel>();
            SaleViewModel.MasterOpenBillViewModel item = new SaleViewModel.MasterOpenBillViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterOpenBill, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
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
                item = new SaleViewModel.MasterOpenBillViewModel();
                item.OpenBillID = Convert.ToInt32(reader["OpenBillID"]);
                item.OpenDateTime = Convert.ToString(reader["OpenDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.Note = Convert.ToString(reader["Note"]);
                item.Total = Convert.ToInt32(reader["Total"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterOpenBillData"] = tempList;  // for paging

            return tempList;
        }

        private PagingViewModel calcMasterOpenBillPaging(List<SaleViewModel.MasterOpenBillViewModel> tempList, [Optional]int currentPage)
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

        private List<SaleViewModel.MasterOpenBillViewModel> getMasterOpenBillByPaging(List<SaleViewModel.MasterOpenBillViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<SaleViewModel.MasterOpenBillViewModel> list = new List<SaleViewModel.MasterOpenBillViewModel>();
            SaleViewModel.MasterOpenBillViewModel item = new SaleViewModel.MasterOpenBillViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new SaleViewModel.MasterOpenBillViewModel();
                item.OpenBillID = tempList[page].OpenBillID;
                item.OpenDateTime = tempList[page].OpenDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.CustomerName = tempList[page].CustomerName;
                item.Note = tempList[page].Note;
                item.Total = tempList[page].Total;
                list.Add(item);
            }
            return list;
        }

        private List<TranSaleModels> selectTranOpenBillByID(int openBillId)
        {
            List<TranSaleModels> list = new List<TranSaleModels>();
            TranSaleModels item = new TranSaleModels();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranOpenBillByID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OpenBillID", openBillId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranSaleModels();
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        private SaleViewModel.CLMasterSaleOrderViewModel selectCLMasterSaleOrder(int clSaleOrderId)
        {
            SaleViewModel.CLMasterSaleOrderViewModel item = new SaleViewModel.CLMasterSaleOrderViewModel();

            SqlCommand cmd = new SqlCommand(textQuery.getCLMasterSaleOrderQuery(clSaleOrderId), (SqlConnection)getConnection());
            cmd.CommandType = CommandType.Text;          
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {             
                item.CustomerID = Convert.ToInt32(reader["CustomerID"]);              
                item.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.Total = Convert.ToInt32(reader["Total"]);
            }
            reader.Close();

            return item;
        }

        private List<TranSaleModels> selectCLTranSaleOrderByID(int clSaleOrderId)
        {
            List<TranSaleModels> list = new List<TranSaleModels>();
            TranSaleModels item = new TranSaleModels();

            SqlCommand cmd = new SqlCommand(textQuery.getCLTranSaleOrderQuery(clSaleOrderId), (SqlConnection)getConnection());
            cmd.CommandType = CommandType.Text;         
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranSaleModels();
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);              
                item.Amount = Convert.ToInt32(reader["Amount"]);             
                item.ProductID = Convert.ToInt32(reader["ProductID"]);                             
                item.ProductCode = Convert.ToString(reader["Code"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        private void clearTranSale()
        {
            Session["TranSaleData"] = null;
        }

        private void getMainMenu()
        {
            saleViewModel.ProductMenus.MainMenus = appData.selectMainMenu(getConnection());
        }

        private int getFirstMainMenuID()
        {
            int mainMenuId = 0;
            if (saleViewModel.ProductMenus.MainMenus.Count() != 0) {
                MainMenuModels.MainMenuModel firstMainMenu = saleViewModel.ProductMenus.MainMenus.First();
                mainMenuId = firstMainMenu.MainMenuID;
            }
            return mainMenuId;
        }

        private void getSubMenu(int mainMenuId)
        {
            saleViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(), mainMenuId);
        }

        private int getFirstSubMenuID()
        {
            int subMenuId = 0;
            if (saleViewModel.ProductMenus.SubMenus.Count() != 0)
            {
                SubMenuModels.SubMenuModel firstSubMenu = saleViewModel.ProductMenus.SubMenus.First();
                subMenuId = firstSubMenu.SubMenuID;
            }
            return subMenuId;
        }

        private void getProduct(int subMenuId)
        {
            saleViewModel.ProductMenus.Products = appData.selectProduct(getConnection(), subMenuId);
            Session["ProductData"] = saleViewModel.ProductMenus.Products;
        }

        private void getCustomer(bool isIncludeDefault)
        {
            if (isIncludeDefault) saleViewModel.Customers.Add(new SelectListItem { Text = AppConstants.AllCustomer, Value = "0" });

            List<CustomerModels.CustomerModel> list = appData.selectCustomer(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                saleViewModel.Customers.Add(new SelectListItem { Text = list[i].CustomerName, Value = Convert.ToString(list[i].CustomerID) });
            }
        }

        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                saleViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.SaleModule, userId, getConnection());
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
            else list = Session["UnitData"] as List<UnitModels>;
            return list;
        }

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

        private void getMCurrency()
        {
            List<CurrencyModels> list = getCurrency();
            for (int i = 0; i < list.Count; i++)
            {
                saleViewModel.Currencies.Add(new SelectListItem { Text = list[i].Keyword, Value = Convert.ToString(list[i].CurrencyID) });
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
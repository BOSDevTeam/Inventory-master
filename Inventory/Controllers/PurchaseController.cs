using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Models;
using Inventory.Common;
using Inventory.ViewModels;
using System.Runtime.InteropServices;
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class PurchaseController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        AppData appData = new AppData();
        PurchaseViewModel purchaseViewModel = new PurchaseViewModel();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();
        TextQuery textQuery = new TextQuery();
        List<TranPurchaseLogModels> lstTranPurchaseLog = new List<TranPurchaseLogModels>();

        [SessionTimeoutAttribute]
        public ActionResult Purchase(int userId,int? purchaseId)
        {
            if(checkConnection())
            {
                int totalQuantity = 0;
                getSupplier(false);
                getLocation();
                getMainMenu();
                getMCurrency();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                clearTranPurchase();
                if(purchaseId!=null)    //edit mode
                {
                    ViewBag.IsEdit = true;
                    MasterPurchaseViewModel data = selectMasterPurchase((int)purchaseId);
                    List<TranPurchaseModels> lstTranPurchase = selectTranPurchaseByPurchaseID((int)purchaseId);
                    for (int i = 0; i < lstTranPurchase.Count(); i++)
                    {
                        totalQuantity += lstTranPurchase[i].Quantity;
                    }
                    Session["TranPurchaseData"] = lstTranPurchase;
                    Session["PurchaseLog"] = lstTranPurchaseLog;
                    ViewBag.TotalItem = lstTranPurchase.Count();
                    ViewBag.UserVoucherNo = data.MasterPurchaseModel.UserVoucherNo;
                    DateTime date = setting.convertStringToDate(data.MasterPurchaseModel.PurchaseDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterPurchaseModel.VoucherID;
                    ViewBag.SupplierID = data.MasterPurchaseModel.SupplierID;
                    ViewBag.LocationID = data.MasterPurchaseModel.LocationID;
                    ViewBag.MCurrencyID = data.MasterPurchaseModel.MCurrencyID;
                    ViewBag.Subtotal = data.MasterPurchaseModel.Subtotal;
                    ViewBag.TaxAmt = data.MasterPurchaseModel.TaxAmt;
                    ViewBag.ChargesAmt = data.MasterPurchaseModel.ChargesAmt;
                    ViewBag.Total = data.MasterPurchaseModel.Total;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.PurchaseID = purchaseId;
                }
                else
                {
                    ViewBag.UserVoucherNo = getUserVoucherNo(userId); // new Purchase
                }               
                return View(purchaseViewModel);
            }                       
            return RedirectToAction("Login", "User");
        }

        [SessionTimeoutAttribute]
        public ActionResult ListPurchase(int userId)
        {
            if(checkConnection())
            {
                getSupplier(true);
                List<PurchaseViewModel.MasterPurchaseViewModel> tempList = selectMasterPurchase(userId, false);
                PagingViewModel pagingViewModel = calcMasterPurchasePaging(tempList);
                List<PurchaseViewModel.MasterPurchaseViewModel> lstMasterPurchase = getMasterPurchaseByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstMasterPurchase"] = lstMasterPurchase;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
                return View(purchaseViewModel);
            }
            return RedirectToAction("Login", "User");
            
        }
        #region Purchase Action
        [HttpGet]
        public JsonResult GetProductByCodeAction(string productCode, bool isMultiUnit, bool isMultiCurrency)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            ProductModels.ProductModel data = new ProductModels.ProductModel();
            string productName = "";
            int productId = 0, price = 0;
            short? disPercent = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isExistProduct = true;
            try
            {
                data = appData.selectProductByCode(getConnection(), productCode);
                if (data.ProductID != 0)
                {
                    productId = data.ProductID;
                    productName = data.ProductName;
                    price = data.PurchasePrice;
                    disPercent = data.DisPercent;
                    if (isMultiUnit) lstUnit = getUnit();
                    if (isMultiCurrency) lstCurrency = getCurrency();
                }
                else isExistProduct = false;
                resultDefaultData.IsRequestSuccess = true;

            }
            catch(Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }            

            var jsonResult = new
            {
                ProductID = productId,
                ProductName = productName,
                PurchasePrice = price,
                DisPercent = disPercent,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsExistProduct = isExistProduct,
                ResultDefaultData = resultDefaultData
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
            int purchasePrice = 0;
            short? disPercent = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isRequestSuccess = false;

            if (Session["SearchProductData"] != null)
            {
                List<ProductModels.ProductModel> list = Session["SearchProductData"] as List<ProductModels.ProductModel>;
                var result = list.Where(c => c.ProductID == productId).SingleOrDefault();
                if (result != null)
                {
                    productName = result.ProductName;
                    code = result.Code;
                    purchasePrice = result.PurchasePrice;
                    disPercent = result.DisPercent;
                    if (isMultiUnit) lstUnit = getUnit();
                    if (isMultiCurrency) lstCurrency = getCurrency();
                    isRequestSuccess = true;
                }
            }

            var jsonResult = new
            {
                ProductName = productName,
                Code = code,
                PurPrice = purchasePrice,
                DisPercent = disPercent,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchasePriceAction(int productId)
        {
            int price = appData.selectPurPriceByProduct(getConnection(), productId);
            return Json(price, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TranPurchaseAddEditAction(int productId, string productCode, string productName, int quantity, int price, int disPercent, int? unitId, string unitKeyword, int? currencyId, string currencyKeyword, bool isEdit, int? number, bool isItemFOC,bool IsVoucherEdit)
        {
            List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            TranPurchaseModels data = new TranPurchaseModels();
            int subtotal = 0, totalQuantity = 0, discount;
            ResultDefaultData resultDefaultData = new ResultDefaultData();            

            data.Number = number;
            data.ProductID = productId;
            data.ProductCode = productCode;
            data.Quantity = quantity;
            data.PurchasePrice = price;
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
            if (IsVoucherEdit)
            {
                if(!isEdit)data.IsNewTran = true;
            }

            if (!isEdit)
            {
                if (Session["TranPurchaseData"] != null)
                {
                    lstTranPurchase = Session["TranPurchaseData"] as List<TranPurchaseModels>;
                    lstTranPurchase.Add(data);
                }
                else lstTranPurchase.Add(data);

                resultDefaultData.IsRequestSuccess = true;
            }
            else
            {
                if (Session["TranPurchaseData"] != null)
                {
                    try
                    {
                        lstTranPurchase = Session["TranPurchaseData"] as List<TranPurchaseModels>;
                        List<TranPurchaseModels> lstEditTran = lstTranPurchase.Where(x => x.Number == number).ToList();
                        if (lstEditTran.Count() != 0) data.IsNewTran = lstEditTran[0].IsNewTran;
                        int index = (int)number - 1;
                        lstTranPurchase[index] = data;
                        resultDefaultData.IsRequestSuccess = true;
                    }
                    catch(Exception ex)
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                        resultDefaultData.Message = ex.Message;
                    }
                    
                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }

            for (int i = 0; i < lstTranPurchase.Count(); i++)
            {
                subtotal += lstTranPurchase[i].Amount;
                totalQuantity += lstTranPurchase[i].Quantity;
            }

            Session["TranPurchaseData"] = lstTranPurchase;
            if (IsVoucherEdit)
            {
                TranPurchaseLogModels dataLog = new TranPurchaseLogModels();
                dataLog.Quantity = data.Quantity;
                dataLog.PurPrice = data.PurchasePrice;
                dataLog.Discount = data.Discount;
                dataLog.Amount = data.Amount;
                dataLog.ProductID = data.ProductID;
                dataLog.UnitID = data.UnitID;
                dataLog.CurrencyID = data.CurrencyID;
                dataLog.DiscountPercent = data.DiscountPercent;
                dataLog.IsFOC = data.IsFOC;
                if (isEdit && !data.IsNewTran) setLog(AppConstants.EditActionCode, dataLog, data, number);
            }

            var jsonResult = new
            {
                LstTranPurchase = lstTranPurchase,
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TranPurchaseDeleteAction(int number,bool isVoucherEdit)
        {
            List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            int subtotal = 0, totalQuantity = 0;
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            
            if (Session["TranPurchaseData"] != null)
            {
                try
                {
                    lstTranPurchase = Session["TranPurchaseData"] as List<TranPurchaseModels>;
                    TranPurchaseModels data = lstTranPurchase[number - 1];
                    if (isVoucherEdit)
                    {
                        TranPurchaseLogModels dataLog = new TranPurchaseLogModels();
                        dataLog.Quantity = data.Quantity;
                        dataLog.PurPrice = data.PurchasePrice;
                        dataLog.Discount = data.Discount;
                        dataLog.Amount = data.Amount;
                        dataLog.ProductID = data.ProductID;
                        dataLog.UnitID = data.UnitID;
                        dataLog.CurrencyID = data.CurrencyID;
                        dataLog.DiscountPercent = data.DiscountPercent;
                        dataLog.IsFOC = data.IsFOC;
                        if (!data.IsNewTran) setLog(AppConstants.DeleteActionCode, dataLog, data, number);
                    }
                    lstTranPurchase.RemoveAt(number - 1);
                    resultDefaultData.IsRequestSuccess = true;
                }
                catch(Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }             
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            for (int i = 0; i < lstTranPurchase.Count(); i++)
            {
                TranPurchaseModels data = lstTranPurchase[i];
                data.Number = i + 1;
                lstTranPurchase[i] = data;
                subtotal += lstTranPurchase[i].Amount;
                totalQuantity += lstTranPurchase[i].Quantity;
            }

            Session["TranPurchaseData"] = lstTranPurchase;

            var jsonResult = new
            {
                LstTranPurchase = lstTranPurchase,
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PrepareToEditTranPurchaseAction(int number, bool isMultiUnit, bool isMultiCurrency)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            TranPurchaseModels data = new TranPurchaseModels();
            string productCode = "", productName = "";
            int productId = 0, quantity = 0, price = 0, disPercent = 0;
            int? unitId = 0, currencyId = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isFOC = false;

            if (Session["TranPurchaseData"] != null)
            {
                lstTranPurchase = Session["TranPurchaseData"] as List<TranPurchaseModels>;

                if (lstTranPurchase.Count() != 0)
                {
                    data = lstTranPurchase[number - 1];
                    if (data != null)
                    {
                        productId = data.ProductID;
                        productCode = data.ProductCode;
                        productName = data.ProductName;
                        quantity = data.Quantity;
                        price = data.PurchasePrice;
                        unitId = data.UnitID;
                        currencyId = data.CurrencyID;
                        disPercent = data.DiscountPercent;
                        if (isMultiUnit) lstUnit = getUnit();
                        if (isMultiCurrency) lstCurrency = getCurrency();
                        isFOC = data.IsFOC;
                        resultDefaultData.IsRequestSuccess = true;
                    }
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
        public JsonResult CancelAction()
        {
            Session["TranPurchaseData"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PaymentSubmitAction(string userVoucherNo, string date, string voucherId, int supplierId, int locationId,int mcurrencyid,
                int paymentId, int? payMethodId, int? bankPaymentId, string remark, int? advancedPay,
                int? payPercent, int? payPercentAmt, int? vouDisPercent, int? vouDisAmount, int? voucherDiscount,
                int tax, int taxAmt, int charges, int chargesAmt, int subtotal, int total, int grandtotal, int userId, bool isVoucherFOC)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranPurchaseData"] != null)
            {
                try
                {
                    List<TranPurchaseModels> list = Session["TranPurchaseData"] as List<TranPurchaseModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                    dt.Columns.Add(new DataColumn("PurchasePrice", typeof(int)));
                    dt.Columns.Add(new DataColumn("CurrencyID", typeof(int)));
                    dt.Columns.Add(new DataColumn("DiscountPercent", typeof(int)));
                    dt.Columns.Add(new DataColumn("Discount", typeof(int)));
                    dt.Columns.Add(new DataColumn("Amount", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsFOC", typeof(bool)));

                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].PurchasePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                    }
                    DateTime purchaseDateTime = DateTime.Parse(date);
                    if (payMethodId == null) payMethodId = 0;
                    if (bankPaymentId == null) bankPaymentId = 0;
                    if (advancedPay == null) advancedPay = 0;
                    if (payPercent == null) payPercent = 0;
                    if (payPercentAmt == null) payPercentAmt = 0;
                    if (vouDisPercent == null) vouDisPercent = 0;
                    if (vouDisAmount == null) vouDisAmount = 0;
                    if (voucherDiscount == null) voucherDiscount = 0;
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertPurchase, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@PurchaseDateTime", purchaseDateTime);
                    cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@MCurrencyID", mcurrencyid);
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
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@PayPercentAmt", payPercentAmt);
                    cmd.Parameters.AddWithValue("@Remark", remark);
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.PurchaseModule);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@IsVoucherFOC", isVoucherFOC);
                    cmd.Parameters.AddWithValue("@AccountCode", AppConstants.PurchaseAccountCode);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) userVoucherNo = Convert.ToString(reader[0]);
                    reader.Close();
                    dataConnectorSQL.Close();
                    clearTranPurchase();
                    resultDefaultData.IsRequestSuccess = true;
                    resultDefaultData.Message = AppConstants.Message.SaveSuccess;
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
        public JsonResult PurchcasePagingAction(int currentPage)
        {
            bool isRequestSuccess = true;
            List<PurchaseViewModel.MasterPurchaseViewModel> lstMasterPurchase = new List<PurchaseViewModel.MasterPurchaseViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["MasterPurchaseData"] != null)
            {
                List<PurchaseViewModel.MasterPurchaseViewModel> tempList = Session["MasterPurchaseData"] as List<PurchaseViewModel.MasterPurchaseViewModel>;
                pagingViewModel = calcMasterPurchasePaging(tempList, currentPage);
                lstMasterPurchase = getMasterPurchaseByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                LstMasterPurchase = lstMasterPurchase,
                TotalPage = pagingViewModel.TotalPageNum,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult CheckLedgerForPurchaseEditAction(int purchaseId)
        {
            bool isSuccess = false;
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            try
            {
                SqlCommand cmd = new SqlCommand(Procedure.PrcCheckLedgerForPurchaseEdit, (SqlConnection)getConnection());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                cmd.Parameters.AddWithValue("@APAccountCode", AppConstants.APAccountCode);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) isSuccess = Convert.ToBoolean(reader[0]);
                reader.Close();
                if (isSuccess)
                {
                    resultDefaultData.IsRequestSuccess = true;
                }
                else
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                    resultDefaultData.Message = AppConstants.Message.NoEditByOutstanding;
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
        public JsonResult DeleteAction(int purchaseId,int userId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalPageNum = 0;
            bool isSuccess = false;
            if (Session["MasterPurchaseData"] != null)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(Procedure.PrcDeletePurchase, (SqlConnection)getConnection());
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PurchaseID",purchaseId);
                    cmd.Parameters.AddWithValue("@PurchaseAccountCode",AppConstants.PurchaseAccountCode);
                    cmd.Parameters.AddWithValue("@APAccountCode", AppConstants.APAccountCode);
                    cmd.Parameters.AddWithValue("@UpdatedUserID",userId);
                    cmd.Parameters.AddWithValue("@UpdatedDateTime",setting.getLocalDateTime());
                    cmd.Parameters.AddWithValue("@ActionCode",AppConstants.DeleteActionCode);
                    cmd.Parameters.AddWithValue("@ActionName",AppConstants.DeleteActionName);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) isSuccess = Convert.ToBoolean(reader[0]);
                    reader.Close();
                    if(isSuccess)
                    {
                        List<PurchaseViewModel.MasterPurchaseViewModel> lstMasterPurchase = Session["MasterPurchaseData"] as List<PurchaseViewModel.MasterPurchaseViewModel>;
                        int index = lstMasterPurchase.FindIndex(x => x.PurchaseID == purchaseId);
                        lstMasterPurchase.RemoveAt(index);
                        if (lstMasterPurchase.Count > paging.eachItemCount)
                        {
                            totalPageNum = lstMasterPurchase.Count / paging.eachItemCount;
                            paging.lastItemCount = lstMasterPurchase.Count % paging.eachItemCount;
                            if (paging.lastItemCount != 0) totalPageNum += 1;
                        }
                        resultDefaultData.IsRequestSuccess = true;
                        resultDefaultData.Message = AppConstants.Message.DeleteSuccess;
                    }
                    else
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                        resultDefaultData.Message = AppConstants.Message.NoDeleteByOutstanding;
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
        public JsonResult ViewAction(int purchaseId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            MasterPurchaseViewModel item = new MasterPurchaseViewModel();
            List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            int grandTotalNotPayPercent = 0;
            try
            {
                item = selectMasterPurchase(purchaseId);
                lstTranPurchase = selectTranPurchaseByPurchaseID(purchaseId);                

                if (item.MasterPurchaseModel.PayPercentAmt != 0)
                    grandTotalNotPayPercent = item.MasterPurchaseModel.Total - (item.MasterPurchaseModel.VoucherDiscount + item.MasterPurchaseModel.AdvancedPay);

                resultDefaultData.IsRequestSuccess = true;
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }

            var jsonResult = new
            {
                LstTranPurchase = lstTranPurchase,
                UserVoucherNo = item.MasterPurchaseModel.UserVoucherNo,
                VoucherID = item.MasterPurchaseModel.VoucherID,
                Remark = item.MasterPurchaseModel.Remark,
                LocationName = item.LocationName,
                Payment = item.Payment,
                PayMethod = item.PayMethod,
                BankPayment = item.BankPayment,

                PurchaseDateTime = item.MasterPurchaseModel.PurchaseDateTime,
                UserName = item.UserName,
                SupplierName = item.SupplierName,
                CurrencyName = item.MasterPurchaseModel.MCurrencyName,
                Subtotal = item.MasterPurchaseModel.Subtotal,
                TaxAmt = item.MasterPurchaseModel.TaxAmt,
                ChargesAmt = item.MasterPurchaseModel.ChargesAmt,
                Total = item.MasterPurchaseModel.Total,
                VoucherDiscount = item.MasterPurchaseModel.VoucherDiscount,
                AdvancedPay = item.MasterPurchaseModel.AdvancedPay,
                PayPercentAmt = item.MasterPurchaseModel.PayPercentAmt,
                Grandtotal = item.MasterPurchaseModel.Grandtotal,
                VouDisPercent = item.MasterPurchaseModel.VouDisPercent,
                PaymentPercent = item.MasterPurchaseModel.PaymentPercent,
                GrandTotalNotPayPercent = grandTotalNotPayPercent,
                IsVouFOC = item.MasterPurchaseModel.IsVouFOC,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(int userId, DateTime fromDate, DateTime toDate, string userVoucherNo, int supplierId)
        {
            List<PurchaseViewModel.MasterPurchaseViewModel> tempList = selectMasterPurchase(userId, true, fromDate, toDate, userVoucherNo, supplierId);
            PagingViewModel pagingViewModel = calcMasterPurchasePaging(tempList);
            List<PurchaseViewModel.MasterPurchaseViewModel> lstMasterPurchase = getMasterPurchaseByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterPurchase = lstMasterPurchase
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RefreshAction(int userId)
        {
            List<PurchaseViewModel.MasterPurchaseViewModel> tempList = selectMasterPurchase(userId, false);
            PagingViewModel pagingViewModel = calcMasterPurchasePaging(tempList);
            List<PurchaseViewModel.MasterPurchaseViewModel> lstMasterPurchase = getMasterPurchaseByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterPurchase = lstMasterPurchase
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult PaymentEditAction(int purchaseId, string date, string voucherId, int supplierId, int locationId,int mcurrencyId, int taxAmt, int chargesAmt, int subtotal, int total,int userId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranPurchaseData"] != null)
            {
                try
                {
                    List<TranPurchaseModels> list = Session["TranPurchaseData"] as List<TranPurchaseModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                    dt.Columns.Add(new DataColumn("PurchasePrice", typeof(int)));
                    dt.Columns.Add(new DataColumn("CurrencyID", typeof(int)));
                    dt.Columns.Add(new DataColumn("DiscountPercent", typeof(int)));
                    dt.Columns.Add(new DataColumn("Discount", typeof(int)));
                    dt.Columns.Add(new DataColumn("Amount", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsFOC", typeof(bool)));
                    
                    DataTable dtLog = new DataTable();
                    dtLog.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dtLog.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    dtLog.Columns.Add(new DataColumn("UnitID", typeof(int)));
                    dtLog.Columns.Add(new DataColumn("PurPrice", typeof(int)));
                    dtLog.Columns.Add(new DataColumn("CurrencyID", typeof(int)));
                    dtLog.Columns.Add(new DataColumn("DiscountPercent", typeof(int)));
                    dtLog.Columns.Add(new DataColumn("Discount", typeof(int)));
                    dtLog.Columns.Add(new DataColumn("Amount", typeof(int)));
                    dtLog.Columns.Add(new DataColumn("IsFOC", typeof(bool)));
                    dtLog.Columns.Add(new DataColumn("ActionCode", typeof(string)));
                    dtLog.Columns.Add(new DataColumn("ActionName", typeof(string)));
                    dtLog.Columns.Add(new DataColumn("OriginalQuantity", typeof(string)));

                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].PurchasePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                        if (list[i].IsNewTran)
                        {
                            dtLog.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].PurchasePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC, AppConstants.NewActionCode, AppConstants.NewActionName,0);
                        }
                    }
                    if (Session["PurchaseLog"] != null)
                    {
                        List<TranPurchaseLogModels> lstTranPurchaseLog = Session["PurchaseLog"] as List<TranPurchaseLogModels>;

                        for (int i = 0; i < lstTranPurchaseLog.Count; i++)
                        {
                            if (lstTranPurchaseLog[i].ActionCode != null)
                            {
                                string actionName = "";
                                if (lstTranPurchaseLog[i].ActionCode == AppConstants.EditActionCode) actionName = AppConstants.EditActionName;
                                else if (lstTranPurchaseLog[i].ActionCode == AppConstants.DeleteActionCode) actionName = AppConstants.DeleteActionName;
                                dtLog.Rows.Add(lstTranPurchaseLog[i].ProductID, lstTranPurchaseLog[i].Quantity, lstTranPurchaseLog[i].UnitID, lstTranPurchaseLog[i].PurPrice, lstTranPurchaseLog[i].CurrencyID, lstTranPurchaseLog[i].DiscountPercent, lstTranPurchaseLog[i].Discount, lstTranPurchaseLog[i].Amount, lstTranPurchaseLog[i].IsFOC, lstTranPurchaseLog[i].ActionCode, actionName,lstTranPurchaseLog[i].OriginalQuantity);
                            }
                        }
                    }

                    DateTime purchaseDateTime = DateTime.Parse(date);
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdatePurchase, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                    cmd.Parameters.AddWithValue("@PurchaseDateTime", purchaseDateTime);
                    cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@MCurrencyID", mcurrencyId);
                    cmd.Parameters.AddWithValue("@TaxAmt", taxAmt);
                    cmd.Parameters.AddWithValue("@ChargesAmt", chargesAmt);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@temptbllog", dtLog);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@AccountCode", AppConstants.PurchaseAccountCode);
                    cmd.Parameters.AddWithValue("@UpdatedUserID", userId);
                    cmd.Parameters.AddWithValue("@UpdatedDateTime", setting.getLocalDateTime());
                    cmd.Parameters.AddWithValue("@ActionCode", AppConstants.EditActionCode);
                    cmd.Parameters.AddWithValue("@ActionName", AppConstants.EditActionName);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    dataConnectorSQL.Close();
                    clearTranPurchase();
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


        #endregion
        #region methods
        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.PurchaseModule, userId, getConnection());
            return userVoucherNo;
        }
        private void getMainMenu()
        {
            purchaseViewModel.ProductMenus.MainMenus = appData.selectMainMenu(getConnection());
        }
        private int getFirstMainMenuID()
        {
            int mainMenuId = 0;
            if (purchaseViewModel.ProductMenus.MainMenus.Count() != 0)
            {
                MainMenuModels.MainMenuModel firstMainMenu = purchaseViewModel.ProductMenus.MainMenus.First();
                mainMenuId = firstMainMenu.MainMenuID;
            }
            return mainMenuId;
        }
        private void getSubMenu(int mainMenuId)
        {
            purchaseViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(), mainMenuId);
        }
        private int getFirstSubMenuID()
        {
            int subMenuId = 0;
            if (purchaseViewModel.ProductMenus.SubMenus.Count() != 0)
            {
                SubMenuModels.SubMenuModel firstSubMenu = purchaseViewModel.ProductMenus.SubMenus.First();
                subMenuId = firstSubMenu.SubMenuID;
            }
            return subMenuId;
        }
        private void getProduct(int subMenuId)
        {
            purchaseViewModel.ProductMenus.Products = appData.selectProduct(getConnection(), subMenuId);
            Session["ProductData"] = purchaseViewModel.ProductMenus.Products;
        }
        private void getSupplier(bool isIncludeDefault)
        {
            if (isIncludeDefault) purchaseViewModel.Suppliers.Add(new SelectListItem { Text = AppConstants.AllSupplier, Value = "0" });

            List<SupplierModels.SupplierModel> list = appData.selectSupplier(getConnection());
            for(int i=0;i<list.Count;i++)
            {
                purchaseViewModel.Suppliers.Add(new SelectListItem { Text = list[i].SupplierName, Value = Convert.ToString(list[i].SupplierID) });
            }
        }
        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                purchaseViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
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
                purchaseViewModel.Currencies.Add(new SelectListItem { Text = list[i].Keyword, Value = Convert.ToString(list[i].CurrencyID) });
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
        private void clearTranPurchase()
        {
            Session["TranPurchaseData"] = null;
        }

        private List<PurchaseViewModel.MasterPurchaseViewModel> selectMasterPurchase(int userId, bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]string userVoucherNo, [Optional]int supplierId)
        {
            List<PurchaseViewModel.MasterPurchaseViewModel> tempList = new List<PurchaseViewModel.MasterPurchaseViewModel>();
            PurchaseViewModel.MasterPurchaseViewModel item = new PurchaseViewModel.MasterPurchaseViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterPurchaseList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@IsSearch", isSearch);
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", setting.getLocalDate());
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
                item = new PurchaseViewModel.MasterPurchaseViewModel();
                item.PurchaseID = Convert.ToInt32(reader["PurchaseID"]);
                item.PurchaseDateTime = Convert.ToString(reader["PurchaseDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.SupplierName = Convert.ToString(reader["SupplierName"]);
                item.PaymentKeyword = Convert.ToString(reader["PaymentKeyword"]);
                item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterPurchaseData"] = tempList;  // for paging

            return tempList;
        }
        private PagingViewModel calcMasterPurchasePaging(List<PurchaseViewModel.MasterPurchaseViewModel> tempList, [Optional]int currentPage)
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
        private List<PurchaseViewModel.MasterPurchaseViewModel> getMasterPurchaseByPaging(List<PurchaseViewModel.MasterPurchaseViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<PurchaseViewModel.MasterPurchaseViewModel> list = new List<PurchaseViewModel.MasterPurchaseViewModel>();
            PurchaseViewModel.MasterPurchaseViewModel item = new PurchaseViewModel.MasterPurchaseViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new PurchaseViewModel.MasterPurchaseViewModel();
                item.PurchaseID = tempList[page].PurchaseID;
                item.PurchaseDateTime = tempList[page].PurchaseDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.SupplierName = tempList[page].SupplierName;
                item.PaymentKeyword = tempList[page].PaymentKeyword;
                item.Grandtotal = tempList[page].Grandtotal;
                list.Add(item);
            }
            return list;
        }
        private MasterPurchaseViewModel selectMasterPurchase(int purchaseId)
        {
            MasterPurchaseViewModel item = new MasterPurchaseViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterPurchaseByPurchaseID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.MasterPurchaseModel.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.MasterPurchaseModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.MasterPurchaseModel.Remark = Convert.ToString(reader["Remark"]);
                item.LocationName = Convert.ToString(reader["LocationName"]);
                item.Payment = Convert.ToString(reader["Payment"]);
                item.PayMethod = Convert.ToString(reader["PayMethod"]);
                item.BankPayment = Convert.ToString(reader["BankPayment"]);
                
                item.MasterPurchaseModel.PurchaseDateTime = Convert.ToString(reader["Date"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                item.SupplierName = Convert.ToString(reader["SupplierName"]);
                
                item.MasterPurchaseModel.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.MasterPurchaseModel.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.MasterPurchaseModel.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.MasterPurchaseModel.Total = Convert.ToInt32(reader["Total"]);
                item.MasterPurchaseModel.VoucherDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                item.MasterPurchaseModel.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                item.MasterPurchaseModel.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                item.MasterPurchaseModel.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                item.MasterPurchaseModel.VouDisPercent = Convert.ToInt32(reader["VouDisPercent"]);
                item.MasterPurchaseModel.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);
                item.MasterPurchaseModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.MasterPurchaseModel.SupplierID = Convert.ToInt32(reader["SupplierID"]);
                item.MasterPurchaseModel.MCurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.MasterPurchaseModel.MCurrencyName = Convert.ToString(reader["Keyword"]);
                item.MasterPurchaseModel.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
            }
            reader.Close();

            return item;
        }
        private List<TranPurchaseModels> selectTranPurchaseByPurchaseID(int purchaseId)
        {
            List<TranPurchaseModels> list = new List<TranPurchaseModels>();
            TranPurchaseModels item = new TranPurchaseModels();
            lstTranPurchaseLog = new List<TranPurchaseLogModels>();
            TranPurchaseLogModels itemLog = new TranPurchaseLogModels();
            int number = 0;
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranPurchaseByPurchaseID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranPurchaseModels();
                number++;
                item.Number = number;
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
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

                itemLog = new TranPurchaseLogModels();
                itemLog.Quantity = Convert.ToInt32(reader["Quantity"]);
                itemLog.OriginalQuantity = Convert.ToInt32(reader["Quantity"]);
                itemLog.PurPrice = Convert.ToInt32(reader["PurPrice"]);
                itemLog.Discount = Convert.ToInt32(reader["Discount"]);
                itemLog.Amount = Convert.ToInt32(reader["Amount"]);
                itemLog.ProductID = Convert.ToInt32(reader["ProductID"]);
                itemLog.UnitID = Convert.ToInt32(reader["UnitID"]);
                itemLog.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                itemLog.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                itemLog.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                lstTranPurchaseLog.Add(itemLog);
            }
            reader.Close();

            return list;
        }

        private void setLog(string actionCode, TranPurchaseLogModels dataLog, TranPurchaseModels data, int? number)
        {
            List<TranPurchaseLogModels> lstTranPurchaseLog = new List<TranPurchaseLogModels>();
            if (Session["PurchaseLog"] != null)
                lstTranPurchaseLog = Session["PurchaseLog"] as List<TranPurchaseLogModels>;

            if (actionCode == AppConstants.EditActionCode)  // edit
            {
                dataLog.ActionCode = AppConstants.EditActionCode;
                dataLog.OriginalQuantity = lstTranPurchaseLog[(int)number - 1].OriginalQuantity;
                if (number != null) lstTranPurchaseLog[(int)number - 1] = dataLog;
            }
            else if (actionCode == AppConstants.DeleteActionCode)  // delete
            {
                dataLog.ActionCode = AppConstants.DeleteActionCode;
                if (number != null) lstTranPurchaseLog[(int)number - 1] = dataLog;
            }

            Session["PurchaseLog"] = lstTranPurchaseLog;
        }

        #endregion
    }
}
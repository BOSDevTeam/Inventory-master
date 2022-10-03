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

namespace Inventory.Controllers
{
    public class SaleController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        SaleViewModel saleViewModel = new SaleViewModel();
        AppData appData = new AppData();
        TextQuery textQuery = new TextQuery();
        Setting setting = new Setting();

        public ActionResult POS(int userId, int? saleId)
        {
            if (checkConnection())
            {
                getCustomer(false);
                getLocation();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                clearTranSale();
                if (saleId == null) ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                else
                {
                    int totalQuantity = 0;
                    ViewBag.IsEdit = true;
                    MasterSaleVoucherViewModel masterSaleVoucherViewModel = selectMasterSale((int)saleId);
                    List<TranSaleModels> lstTranSale = selectTranSaleBySaleID((int)saleId);
                    for (int i = 0; i < lstTranSale.Count(); i++)
                    {                      
                        totalQuantity += lstTranSale[i].Quantity;
                    }
                    Session["TranSaleData"] = lstTranSale;
                    ViewData["LstTranSale"] = lstTranSale;
                    ViewBag.TotalItem = lstTranSale.Count();                                 
                    ViewBag.UserVoucherNo = masterSaleVoucherViewModel.MasterSaleModel.UserVoucherNo;
                    DateTime date =setting.convertStringToDate(masterSaleVoucherViewModel.MasterSaleModel.SaleDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = masterSaleVoucherViewModel.MasterSaleModel.VoucherID;
                    ViewBag.CustomerID = masterSaleVoucherViewModel.MasterSaleModel.CustomerID;
                    ViewBag.LocationID = masterSaleVoucherViewModel.MasterSaleModel.LocationID;
                    ViewBag.Subtotal = masterSaleVoucherViewModel.MasterSaleModel.Subtotal;
                    ViewBag.TaxAmt = masterSaleVoucherViewModel.MasterSaleModel.TaxAmt;
                    ViewBag.ChargesAmt = masterSaleVoucherViewModel.MasterSaleModel.ChargesAmt;
                    ViewBag.Total = masterSaleVoucherViewModel.MasterSaleModel.Total;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.SaleID = saleId;
                }

                return View(saleViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult ListSale(int userId)
        {
            if (checkConnection())
            {               
                getCustomer(true);
                List<SaleViewModel.MasterSaleViewModel> lstMasterSale = selectMasterSale(userId,false);
                ViewData["LstMasterSale"] = lstMasterSale;
                return View(saleViewModel);
            }
            return RedirectToAction("Login", "User");
        }

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

        #region events

        [HttpGet]
        public JsonResult MainMenuClickAction(int mainMenuId)
        {
            getSubMenu(mainMenuId);
            return Json(saleViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SubMenuClickAction(int subMenuId)
        {
            getProduct(subMenuId);
            return Json(saleViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ProductClickAction(int productId, bool isMultiUnit,bool isMultiCurrency)
        {
            string productName = "", code = "";
            int salePrice = 0;
            short? disPercent = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isRequestSuccess = false;

            if (Session["ProductData"] != null)
            {
                List<ProductModels.ProductModel> list = Session["ProductData"] as List<ProductModels.ProductModel>;
                var result = list.Where(c => c.ProductID == productId).SingleOrDefault();
                if (result != null)
                {
                    productName = result.ProductName;
                    code = result.Code;
                    salePrice = result.SalePrice;
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
                SalePrice = salePrice,
                DisPercent = disPercent,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TranSaleAddEditAction(int productId, string productCode, string productName, int quantity, int price, int disPercent, int? unitId, string unitKeyword, int? currencyId, string currencyKeyword, bool isEdit,int? number)
        {
            List<TranSaleModels> lstTranSale = new List<TranSaleModels>();
            TranSaleModels data = new TranSaleModels();
            int subtotal = 0, totalQuantity = 0, discount;
            bool isRequestSuccess = true;

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

            if (!isEdit)
            {
                if (Session["TranSaleData"] != null)
                {
                    lstTranSale = Session["TranSaleData"] as List<TranSaleModels>;                 
                    lstTranSale.Add(data);
                }
                else lstTranSale.Add(data);                
            }
            else
            {
                if (Session["TranSaleData"] != null)
                {
                    lstTranSale = Session["TranSaleData"] as List<TranSaleModels>;                  
                    int index = (int) number - 1;
                    lstTranSale[index] = data;
                }
                else isRequestSuccess = false;
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
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TranSaleDeleteAction(int number)
        {
            List<TranSaleModels> lstTranSale = new List<TranSaleModels>();
            int subtotal = 0, totalQuantity = 0;
            bool isRequestSuccess = true;

            if (Session["TranSaleData"] != null)
            {
                lstTranSale = Session["TranSaleData"] as List<TranSaleModels>;               
                lstTranSale.RemoveAt(number - 1);
            }
            else isRequestSuccess = false;

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
                IsRequestSuccess = isRequestSuccess
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
            bool isRequestSuccess = false;

            if (Session["TranSaleData"] != null)
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
                        isRequestSuccess = true;
                    }
                }
            }

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
                IsRequestSuccess = isRequestSuccess
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
            bool isRequestSuccess = false;

            if (Session["SearchProductData"] != null)
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
                    isRequestSuccess = true;
                }
            }

            var jsonResult = new
            {
                ProductName = productName,
                Code = code,
                SalePrice = salePrice,
                DisPercent = disPercent,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PaymentAction(bool isBankPayment)
        {
            List<PaymentModels> lstPayment = new List<PaymentModels>();
            List<PayMethodModels> lstPayMethod = new List<PayMethodModels>();
            bool isRequestSuccess = true;

            if (Session["TranSaleData"] != null)
            {
                lstPayment = getPayment();
                if (isBankPayment) lstPayMethod = getPayMethod();
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                LstPayment = lstPayment,
                LstPayMethod = lstPayMethod,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PaymentEditAction(int saleId, string date, string voucherId, int customerId, int locationId, int taxAmt, int chargesAmt, int subtotal, int total)
        {
            bool isRequestSuccess = true;

            if (Session["TranSaleData"] != null)
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
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                IsRequestSuccess = isRequestSuccess
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLimitedDayAction()
        {         
            List<LimitedDayModels> lstLimitedDay = new List<LimitedDayModels>();        
            lstLimitedDay = getLimitedDay();                      
            return Json(lstLimitedDay, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBankPaymentAction()
        {
            List<BankPaymentModels> lstBankPayment = new List<BankPaymentModels>();
            lstBankPayment = getBankPayment();
            return Json(lstBankPayment, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PaymentSubmitAction(string userVoucherNo, string date, string voucherId, int customerId, int locationId,
                int paymentId, int? payMethodId, int? limitedDayId, int? bankPaymentId, string remark, int? advancedPay,
                int? payPercent, int? payPercentAmt, int? vouDisPercent, int? vouDisAmount, int? voucherDiscount,
                int tax, int taxAmt, int charges, int chargesAmt, int subtotal, int total, int grandtotal, int userId)
        {
            string systemVoucherNo = "";
            bool isRequestSuccess = true;

            if (Session["TranSaleData"] != null)
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

                SqlCommand cmd = new SqlCommand(Procedure.PrcInsertSale, dataConnectorSQL.Connect());
                cmd.Parameters.AddWithValue("@SaleDateTime", saleDateTime);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
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
                cmd.Parameters.AddWithValue("@ModuleCode", 1);
                cmd.Parameters.AddWithValue("@temptbl", dt);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) systemVoucherNo = Convert.ToString(reader[0]);
                reader.Close();
                dataConnectorSQL.Close();
                clearTranSale();
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                SystemVoucherNo = systemVoucherNo,
                LocationID = locationId,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(int userId,DateTime fromDate,DateTime toDate,string userVoucherNo,int customerId)
        {
            List<SaleViewModel.MasterSaleViewModel> lstMasterSale = selectMasterSale(userId, true, fromDate, toDate, userVoucherNo, customerId);
            return Json(lstMasterSale, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RefreshAction(int userId)
        {
            List<SaleViewModel.MasterSaleViewModel> lstMasterSale = selectMasterSale(userId, false);
            return Json(lstMasterSale, JsonRequestBehavior.AllowGet);
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
                GrandTotalNotPayPercent = grandTotalNotPayPercent
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int saleId)
        {
            SqlCommand cmd = new SqlCommand(textQuery.deleteSaleQuery(saleId), (SqlConnection)getConnection());
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();          
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region methods

        public MasterSaleVoucherViewModel selectMasterSale(string systemVoucherNo)
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
            }
            reader.Close();         

            return item;
        }

        public MasterSaleVoucherViewModel selectMasterSale(int saleId)
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
                item.MasterSaleModel.CustomerID = Convert.ToInt32(reader["CustomerID"]);
            }
            reader.Close();

            return item;
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
        }

        public List<SaleViewModel.MasterSaleViewModel> selectMasterSale(int userId, bool isSearch,[Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]string userVoucherNo, [Optional]int customerId)
        {
            List<SaleViewModel.MasterSaleViewModel> list = new List<SaleViewModel.MasterSaleViewModel>();
            SaleViewModel.MasterSaleViewModel item = new SaleViewModel.MasterSaleViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@IsSearch", isSearch);
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@ToDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@UserVoucherNo", "");
                cmd.Parameters.AddWithValue("@CustomerID", 0);
            }else
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
                list.Add(item);     
            }
            reader.Close();
           
            return list;
        }

        public List<TranSaleModels> selectTranSaleBySaleID(int saleId)
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
            saleViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(),mainMenuId);
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

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.SaleModule, userId, getConnection());
            return userVoucherNo;
        }

        private void getCustomer(bool isIncludeDefault)
        {
            if (isIncludeDefault) saleViewModel.Customers.Add(new SelectListItem { Text = "All Customer", Value = "0" });

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

        private List<PaymentModels> getPayment()
        {
            List<PaymentModels> list = new List<PaymentModels>();
            if (Session["PaymentData"] == null)
            {
                list = appData.selectPayment(getConnection());
                Session["PaymentData"] = list;
            }
            else list = Session["PaymentData"] as List<PaymentModels>;
            return list;
        }

        private List<PayMethodModels> getPayMethod()
        {
            List<PayMethodModels> list = new List<PayMethodModels>();
            if (Session["PayMethodData"] == null)
            {
                list = appData.selectPayMethod(getConnection());
                Session["PayMethodData"] = list;
            }
            else list = Session["PayMethodData"] as List<PayMethodModels>;
            return list;
        }

        private List<LimitedDayModels> getLimitedDay()
        {
            List<LimitedDayModels> list = new List<LimitedDayModels>();
            if (Session["LimitedDayData"] == null)
            {
                list = appData.selectLimitedDay(getConnection());
                Session["LimitedDayData"] = list;
            }
            else list = Session["LimitedDayData"] as List<LimitedDayModels>;
            return list;
        }

        private List<BankPaymentModels> getBankPayment()
        {
            List<BankPaymentModels> list = new List<BankPaymentModels>();
            if (Session["BankPaymentData"] == null)
            {
                list = appData.selectBankPayment(getConnection());
                Session["BankPaymentData"] = list;
            }
            else list = Session["BankPaymentData"] as List<BankPaymentModels>;
            return list;
        }

        public bool checkConnection()
        {
            if (Session[AppConstants.SQLConnection] != null) return true;
            else return false;
        }

        public object getConnection()
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Models;
using Inventory.Common;
using Inventory.ViewModels;
using System.Runtime.InteropServices;

namespace Inventory.Controllers
{
    public class PurchaseController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        AppData appData = new AppData();
        PurchaseViewModel purchaseViewModel = new PurchaseViewModel();

        public ActionResult Purchase(int userId,int? purchaseId)
        {
            if(checkConnection())
            {
                //int totalQuantity = 0;
                getSupplier();
                getLocation();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                ViewBag.UserVoucherNo = getUserVoucherNo(userId); // new Purchase
                return View(purchaseViewModel);
            }                       
            return RedirectToAction("Login", "User");
        }

        public ActionResult ListPurchase(int userId)
        {
            return View();
        }
        #region Purchase Action
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
                price = data.PurchasePrice;
                disPercent = data.DisPercent;
                if (isMultiUnit) lstUnit = getUnit();
                if (isMultiCurrency) lstCurrency = getCurrency();
            }
            else isExistProduct = false;

            var jsonResult = new
            {
                ProductID = productId,
                ProductName = productName,
                PurchasePrice = price,
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
        public JsonResult TranPurchaseAddEditAction(int productId, string productCode, string productName, int quantity, int price, int disPercent, int? unitId, string unitKeyword, int? currencyId, string currencyKeyword, bool isEdit, int? number, bool isItemFOC)
        {
            List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            TranPurchaseModels data = new TranPurchaseModels();
            int subtotal = 0, totalQuantity = 0, discount;
            bool isRequestSuccess = true;

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

            if (!isEdit)
            {
                if (Session["TranPurchaseData"] != null)
                {
                    lstTranPurchase = Session["TranPurchaseData"] as List<TranPurchaseModels>;
                    lstTranPurchase.Add(data);
                }
                else lstTranPurchase.Add(data);
            }
            else
            {
                if (Session["TranPurchaseData"] != null)
                {
                    lstTranPurchase = Session["TranPurchaseData"] as List<TranPurchaseModels>;
                    int index = (int)number - 1;
                    lstTranPurchase[index] = data;
                }
                else isRequestSuccess = false;
            }

            for (int i = 0; i < lstTranPurchase.Count(); i++)
            {
                subtotal += lstTranPurchase[i].Amount;
                totalQuantity += lstTranPurchase[i].Quantity;
            }

            Session["TranPurchaseData"] = lstTranPurchase;

            var jsonResult = new
            {
                LstTranPurchase = lstTranPurchase,
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TranPurchaseDeleteAction(int number)
        {
            List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            int subtotal = 0, totalQuantity = 0;
            bool isRequestSuccess = true;

            if (Session["TranPurchaseData"] != null)
            {
                lstTranPurchase = Session["TranPurchaseData"] as List<TranPurchaseModels>;
                lstTranPurchase.RemoveAt(number - 1);
            }
            else isRequestSuccess = false;

            for (int i = 0; i < lstTranPurchase.Count(); i++)
            {
                subtotal += lstTranPurchase[i].Amount;
                totalQuantity += lstTranPurchase[i].Quantity;
            }

            Session["TranPurchaseData"] = lstTranPurchase;

            var jsonResult = new
            {
                LstTranPurchase = lstTranPurchase,
                SubTotal = subtotal,
                TotalQuantity = totalQuantity,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PrepareToEditTranPurchaseAction(int number, bool isMultiUnit, bool isMultiCurrency)
        {
            List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            TranPurchaseModels data = new TranPurchaseModels();
            string productCode = "", productName = "";
            int productId = 0, quantity = 0, price = 0, disPercent = 0;
            int? unitId = 0, currencyId = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isRequestSuccess = false, isFOC = false;

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
                IsFOC = isFOC,
                IsRequestSuccess = isRequestSuccess
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
        public JsonResult PaymentSubmitAction(string userVoucherNo, string date, string voucherId, int supplierId, int locationId,
                int paymentId, int? payMethodId, int? bankPaymentId, string remark, int? advancedPay,
                int? payPercent, int? payPercentAmt, int? vouDisPercent, int? vouDisAmount, int? voucherDiscount,
                int tax, int taxAmt, int charges, int chargesAmt, int subtotal, int total, int grandtotal, int userId, bool isVoucherFOC)
        {
            bool isRequestSuccess = true;

            if (Session["TranPurchaseData"] != null)
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
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) userVoucherNo = Convert.ToString(reader[0]);
                reader.Close();
                dataConnectorSQL.Close();
                clearTranPurchase();
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                UserVoucherNo = userVoucherNo,
                IsRequestSuccess = isRequestSuccess
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
        private void getSupplier()
        {
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
        #endregion
    }
}
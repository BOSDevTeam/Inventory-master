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

namespace Inventory.Controllers
{
    public class TransferController : MyController
    {
        #region Page
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        TransferViewModel transferViewModel = new TransferViewModel();
        AppData appData = new AppData();
        public ActionResult Transfer(int userId)
        {
            if (checkConnection()) {
                getLocation();
                getUnit();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                clearTranTransfer();
                ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                return View(transferViewModel);

            }
            return RedirectToAction("Login", "User");



        }

        public ActionResult ListTransfer()
        {
            return View();
        }

        #endregion

        #region Transfer Action

        public JsonResult PrepareToEditTranTransferAction(int number, bool isMultiUnit)
        {
            string productCode = "", productName = "";
            int productId = 0, quantity = 0;
            int? unitId = 0;

            List<TranTransferModels> list = new List<TranTransferModels>();
            TranTransferModels data = new TranTransferModels();
            List<UnitModels> lstUnit = new List<UnitModels>();
            bool isRequestSuccess = false;
            if (Session["TranTransferData"] != null)
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
                    isRequestSuccess = true;
                }
            }

            var jsonResult = new
            {
                ProductID = productId,
                ProductCode = productCode,
                ProductName = productName,
                Quantity = quantity,
                UnitID = unitId,
                LstUnit = lstUnit,
                IsRequestSuccess = isRequestSuccess
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult TranTransferAddEditAction(int productId, string productCode, string productName, int quantity, int? unitId, string unitKeyword, bool isEdit, int? number)
        {
            List<TranTransferModels> list = new List<TranTransferModels>();
            TranTransferModels data = new TranTransferModels();
            int totalQuantity = 0; bool isRequestSuccess = true;
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
                    list = Session["TranTransferData"] as List<TranTransferModels>;
                    list.Add(data);
                }
                else
                {
                    list.Add(data);
                }
            }
            else
            {
                if (Session["TranTransferData"] != null)
                {
                    list = Session["TranTransferData"] as List<TranTransferModels>;
                    int index = (int)number - 1;
                    list[index] = data;
                }
                else
                {
                    isRequestSuccess = false;
                }
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
                IsRequestSuccess = isRequestSuccess

            };


            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TransferDeleteAction(int number)
        {
            List<TranTransferModels> list = new List<TranTransferModels>();
            int totalQuantity = 0;
            bool isRequestSuccess = true;
            if (Session["TranTransferData"] != null)
            {
                list = Session["TranTransferData"] as List<TranTransferModels>;
                list.RemoveAt(number -1);
            }
            else isRequestSuccess = false;
            for (int i = 0; i < list.Count; i++)
            {
                totalQuantity += list[i].Quantity;
            }

            Session["TranTransferData"] = list;

            var JsonResult = new
            {
                TotalQuantity = totalQuantity,
                IsRequestSuccess = isRequestSuccess,
                LstTranTransfer = list

            };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveAction(int formLocation, int toLocation)
        {
            bool isRequestSuccess = true;
            if (formLocation != toLocation)
            {
                if (Session["TranTransferData"] != null)
                {
                    List<TranTransferModels> list = Session["TranTransferData"] as List<TranTransferModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add("TransferID", typeof(int));
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("UnitID", typeof(int));
                    for (int i = 0; i<list.Count; i++)
                    {
                        dt.Rows.Add(list[i].TransferID, list[i].ProductID, list[i].Quantity, list[i].UnitID);
                    }
                }
            }
            else
            {
                isRequestSuccess = false;

            }
            var jsonResult = new
            {
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
            bool isRequestSuccess = false;
            if (Session["SearchProductData"] != null)
            {
                List<ProductModels.ProductModel> list = Session["SearchProductData"] as List<ProductModels.ProductModel>;
                var result = list.Where(p => p.ProductID == productId).SingleOrDefault();
                productName = result.ProductName;
                code = result.Code;
                if (isMultiUnit) lstUnit = getUnit();
                isRequestSuccess = true;
            }
            var jsonResult = new
            {
                IsRequestSuccess = isRequestSuccess,
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
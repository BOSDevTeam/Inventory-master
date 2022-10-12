using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using Inventory.Common;
using Inventory.Models;

namespace Inventory.Controllers
{
    public class ProductMenuController : MyController
    {
        ProductMenuViewModel productMenuViewModel = new ProductMenuViewModel();
        AppData appData = new AppData();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();

        #region events

        [HttpGet]
        public JsonResult MainMenuClickAction(int mainMenuId)
        {
            getSubMenu(mainMenuId);
            return Json(productMenuViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SubMenuClickAction(int subMenuId)
        {
            getProduct(subMenuId);
            return Json(productMenuViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ProductClickAction(int productId, bool isMultiUnit, bool isMultiCurrency)
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

        #endregion

        #region methods

        private void getSubMenu(int mainMenuId)
        {
            productMenuViewModel.SubMenus = appData.selectSubMenu(getConnection(), mainMenuId);
        }

        private void getProduct(int subMenuId)
        {
            productMenuViewModel.Products = appData.selectProduct(getConnection(), subMenuId);
            Session["ProductData"] = productMenuViewModel.Products;
        }

        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();

            connection = Session[AppConstants.SQLConnection];
            return connection;
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

        #endregion
    }
}
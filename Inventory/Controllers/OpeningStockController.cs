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
    public class OpeningStockController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        OpeningStockViewModel openingStockViewModel = new OpeningStockViewModel();
        AppData appData = new AppData();
        TextQuery textQuery = new TextQuery();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();

        public ActionResult OpeningStock(int userId)
        {
            if (checkConnection())
            {               
                getLocation();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                clearOSSession();
                ViewBag.UserVoucherNo = getUserVoucherNo(userId);

                return View(openingStockViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult ListOpeningStock()
        {
            return View();
        }

        [HttpGet]
        public JsonResult AddQuantityAction(int productId,int quantity)
        {
          
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SubMenuClickAction(int subMenuId)
        {
            if(Session["TranOpeningStockData"] != null)
            {
                //lstTranSale = Session["TranOpeningStockData"] as List<>;
            }
            getProduct(subMenuId);
            setOSSession();
            return Json(openingStockViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ResetAction()
        {
            clearOSSession();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #region Method

        private void setOSSession()
        {
            Session["TranOpeningStockData"] = openingStockViewModel.ProductMenus.Products;
        }

        private void clearOSSession()
        {
            Session["TranOpeningStockData"] = null;
        }

        private void getMainMenu()
        {
            openingStockViewModel.ProductMenus.MainMenus = appData.selectMainMenu(getConnection());
        }

        private int getFirstMainMenuID()
        {
            int mainMenuId = 0;
            if (openingStockViewModel.ProductMenus.MainMenus.Count() != 0)
            {
                MainMenuModels.MainMenuModel firstMainMenu = openingStockViewModel.ProductMenus.MainMenus.First();
                mainMenuId = firstMainMenu.MainMenuID;
            }
            return mainMenuId;
        }

        private void getSubMenu(int mainMenuId)
        {
            openingStockViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(), mainMenuId);
        }

        private void getProduct(int subMenuId)
        {
            openingStockViewModel.ProductMenus.Products = appData.selectProduct(getConnection(), subMenuId);           
        }      

        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                openingStockViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.OpeningStockModule, userId, getConnection());
            return userVoucherNo;
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
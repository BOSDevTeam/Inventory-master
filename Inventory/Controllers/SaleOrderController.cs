using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using Inventory.Models;
using Inventory.ViewModels;


namespace Inventory.Controllers
{
    public class SaleOrderController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();
        AppData appData = new AppData();

        public ActionResult SaleOrder(int userId)
        {
            if (checkConnection())
            {
                getLocation();
                getCustomer(false);
                getUnit();
                getCurrency();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                return View(saleOrderViewModel);
            }
            return RedirectToAction("Login", "User");
           
        }

        public ActionResult ListSaleOrder(int userId)
        {
            getCustomer(true);
            return View(saleOrderViewModel);
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
            if (isIncludeDefault) saleOrderViewModel.Customers.Add(new SelectListItem { Text = "All Customer", Value = "0" });
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

    }
}
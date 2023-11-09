using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Models;
using Inventory.Filters;
using Inventory.Common;
using Inventory.ViewModels;

namespace Inventory.Controllers
{
    public class RpPurchaseItemSimpleController : MyController
    {
        AppSetting setting = new AppSetting();
        RpPurchaseItemSimpleViewModel purchaseItemSimpleViewModel = new RpPurchaseItemSimpleViewModel();
        // GET: RpPurchaseItemSimple
        [SessionTimeoutAttribute]
        public ActionResult PurchaseItemSimpleReportFilter()
        {
            try
            {
                ViewBag.MenuData = getMenuData();
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View();
        }

        public ActionResult PurchaseItemSimpleReport(DateTime fromDate,DateTime toDate, int? selectedLocationId)
        {
            try
            {
                string getVal = Request.QueryString["SubMenuID"];
                string concat = @"{""data"":" + getVal + "}";
                ValList vl = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ValList>(concat);
                ValList subMenu = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ValList>(concat);
                List<RpPurchaseItemSimpleViewModel.MainMenuViewModel> lstPurchaseItemSimpleRpt = GetPurchaseItemSimpleReport(fromDate, toDate, subMenu.data,selectedLocationId);
                purchaseItemSimpleViewModel.lstPurchaseRpt = lstPurchaseItemSimpleRpt;
                purchaseItemSimpleViewModel.FromDate = fromDate;
                purchaseItemSimpleViewModel.ToDate = toDate;
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(purchaseItemSimpleViewModel);
        }

        public List<RpPurchaseItemSimpleViewModel.MainMenuViewModel> GetPurchaseItemSimpleReport(DateTime fromDate, DateTime toDate, List<int> lstSubMenuID, int? selectedLocationId)
        {
            List<RpPurchaseItemSimpleViewModel.MainMenuViewModel> lstPurchaseItemSimpleRpt = new List<RpPurchaseItemSimpleViewModel.MainMenuViewModel>();
            RpPurchaseItemSimpleViewModel.MainMenuViewModel MainMenuModel = new RpPurchaseItemSimpleViewModel.MainMenuViewModel();
            RpPurchaseItemSimpleViewModel.SubMenuViewModel SubMenuModel = new RpPurchaseItemSimpleViewModel.SubMenuViewModel();
            RpPurchaseItemSimpleViewModel.PurchaseItemViewModel PurchaseItemModel = new RpPurchaseItemSimpleViewModel.PurchaseItemViewModel();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("SubMenuID", typeof(int)));
            for (int i = 0; i < lstSubMenuID.Count(); i++)
            {
                dt.Rows.Add(Convert.ToInt32(lstSubMenuID[i]));
            }
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptPurchaseItemSimple, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@temptbl", dt);
            cmd.Parameters.AddWithValue("@selectedLocationId", selectedLocationId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                MainMenuModel = new RpPurchaseItemSimpleViewModel.MainMenuViewModel();
                MainMenuModel.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                MainMenuModel.MainMenuName = Convert.ToString(reader["MainMenuName"]);
                if (lstPurchaseItemSimpleRpt.Where(m => m.MainMenuID == MainMenuModel.MainMenuID).Count() > 0) // MainMenu Already exist Condition
                {
                    foreach (var MainMenuData in lstPurchaseItemSimpleRpt.Where(m => m.MainMenuID == MainMenuModel.MainMenuID))
                    {
                        SubMenuModel = new RpPurchaseItemSimpleViewModel.SubMenuViewModel();
                        SubMenuModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                        SubMenuModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);
                        if (MainMenuData.lstSubMenu.Where(m => m.SubMenuID == SubMenuModel.SubMenuID).Count() > 0)// SubMenu Already exist Condition
                        {
                            PurchaseItemModel = new RpPurchaseItemSimpleViewModel.PurchaseItemViewModel();
                            PurchaseItemModel.Code = Convert.ToString(reader["Code"]);
                            PurchaseItemModel.ProductName = Convert.ToString(reader["ProductName"]);
                            PurchaseItemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                            PurchaseItemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                            PurchaseItemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                            PurchaseItemModel.Discount = Convert.ToInt32(reader["Discount"]);
                            PurchaseItemModel.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                            PurchaseItemModel.Amount = Convert.ToInt32(reader["Amount"]);
                            foreach (var SubMenuData in MainMenuData.lstSubMenu.Where(m => m.SubMenuID == SubMenuModel.SubMenuID))
                            {
                                SubMenuData.lstPurchaseItem.Add(PurchaseItemModel);
                            }
                        }
                        else
                        {
                            MainMenuData.lstSubMenu.Add(SubMenuModel);
                            PurchaseItemModel = new RpPurchaseItemSimpleViewModel.PurchaseItemViewModel();
                            PurchaseItemModel.Code = Convert.ToString(reader["Code"]);
                            PurchaseItemModel.ProductName = Convert.ToString(reader["ProductName"]);
                            PurchaseItemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                            PurchaseItemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                            PurchaseItemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                            PurchaseItemModel.Discount = Convert.ToInt32(reader["Discount"]);
                            PurchaseItemModel.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                            PurchaseItemModel.Amount = Convert.ToInt32(reader["Amount"]);
                            foreach (var SubMenuData in MainMenuData.lstSubMenu.Where(m => m.SubMenuID == SubMenuModel.SubMenuID))
                            {
                                List<RpPurchaseItemSimpleViewModel.PurchaseItemViewModel> lstPurchaseItem = new List<RpPurchaseItemSimpleViewModel.PurchaseItemViewModel>();
                                lstPurchaseItem.Add(PurchaseItemModel);
                                SubMenuData.lstPurchaseItem = lstPurchaseItem;
                            }
                        }
                    }
                }
                else
                {   //new MainMenu condition
                    lstPurchaseItemSimpleRpt.Add(MainMenuModel);
                    SubMenuModel = new RpPurchaseItemSimpleViewModel.SubMenuViewModel();
                    SubMenuModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                    SubMenuModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);
                    foreach (var MainMenuData in lstPurchaseItemSimpleRpt.Where(m => m.MainMenuID == MainMenuModel.MainMenuID))
                    {
                        List<RpPurchaseItemSimpleViewModel.SubMenuViewModel> lst = new List<RpPurchaseItemSimpleViewModel.SubMenuViewModel>();
                        lst.Add(SubMenuModel);
                        MainMenuData.lstSubMenu = lst;
                        PurchaseItemModel = new RpPurchaseItemSimpleViewModel.PurchaseItemViewModel();
                        PurchaseItemModel.Code = Convert.ToString(reader["Code"]);
                        PurchaseItemModel.ProductName = Convert.ToString(reader["ProductName"]);
                        PurchaseItemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                        PurchaseItemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                        PurchaseItemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                        PurchaseItemModel.Discount = Convert.ToInt32(reader["Discount"]);
                        PurchaseItemModel.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                        PurchaseItemModel.Amount = Convert.ToInt32(reader["Amount"]);
                        foreach (var SubMenuData in MainMenuData.lstSubMenu.Where(m => m.SubMenuID == SubMenuModel.SubMenuID))
                        {
                            List<RpPurchaseItemSimpleViewModel.PurchaseItemViewModel> lstPurchaseItem = new List<RpPurchaseItemSimpleViewModel.PurchaseItemViewModel>();
                            lstPurchaseItem.Add(PurchaseItemModel);
                            SubMenuData.lstPurchaseItem = lstPurchaseItem;
                        }
                    }
                }
            }
            reader.Close();
            setting.conn.Close();
            int totalQuanity = 0, totalAmount = 0, totalDis = 0;
            foreach (var MainMenu in lstPurchaseItemSimpleRpt)
            {
                foreach (var SubMenu in MainMenu.lstSubMenu)
                {
                    totalQuanity += SubMenu.lstPurchaseItem.Sum(m => m.Quantity);
                    totalAmount += SubMenu.lstPurchaseItem.Sum(m => m.Amount);
                    totalDis += SubMenu.lstPurchaseItem.Sum(m => m.Discount);
                }
            }
            purchaseItemSimpleViewModel.TotalQuantity = totalQuanity;
            purchaseItemSimpleViewModel.TotalAmount = totalAmount;
            purchaseItemSimpleViewModel.TotalDis = totalDis;
            return lstPurchaseItemSimpleRpt;
        }


        public List<SubMenuModels.SubMenuModel> getMenuData()
        {
            List<SubMenuModels.SubMenuModel> lst = new List<SubMenuModels.SubMenuModel>();
            SubMenuModels.SubMenuModel menu = new SubMenuModels.SubMenuModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMenuData, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                menu = new SubMenuModels.SubMenuModel();
                menu.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                menu.MainMenuName = Convert.ToString(reader["MainMenuName"]);
                menu.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                menu.SubMenuName = Convert.ToString(reader["SubMenuName"]);
                lst.Add(menu);
            }
            reader.Close();
            setting.conn.Close();
            return lst;
        }
        public class ValList
        {
            public List<int> data { get; set; }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Common;
using System.Runtime.InteropServices;
using Inventory.Filters;


namespace Inventory.Controllers
{
    public class RpSaleItemSimpleController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        RpSaleItemSimpleViewModel saleItemSimpleViewModel = new RpSaleItemSimpleViewModel();
        public ActionResult SaleItemSimpleReportFilter()
        {
            ViewBag.MenuData = getMenuData();
            return View();
        }

        public ActionResult SaleItemSimpleReport(DateTime FromDate,DateTime ToDate)
        {
            string getVal = Request.QueryString["SubMenuID"];
            string concat = @"{""data"":" + getVal + "}";
            ValList vl = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ValList>(concat);
            ValList subMenu = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ValList>(concat);            
            List<RpSaleItemSimpleViewModel.MainMenuViewModel> lstSaleItemSimpleRpt = GetSaleItemSimpleReport(FromDate,ToDate,subMenu.data);
            saleItemSimpleViewModel.lstSaleRpt = lstSaleItemSimpleRpt;
            saleItemSimpleViewModel.FromDate = FromDate;
            saleItemSimpleViewModel.ToDate = ToDate;
            return View(saleItemSimpleViewModel);
        }

        public List<RpSaleItemSimpleViewModel.MainMenuViewModel> GetSaleItemSimpleReport(DateTime FromDate,DateTime ToDate,List<int>lstSubMenuID)
        {
            List<RpSaleItemSimpleViewModel.MainMenuViewModel> lstSaleItemSimpleRpt = new List<RpSaleItemSimpleViewModel.MainMenuViewModel>();
            RpSaleItemSimpleViewModel.MainMenuViewModel MainMenuModel = new RpSaleItemSimpleViewModel.MainMenuViewModel();
            RpSaleItemSimpleViewModel.SubMenuViewModel SubMenuModel = new RpSaleItemSimpleViewModel.SubMenuViewModel();
            RpSaleItemSimpleViewModel.SaleItemViewModel SaleItemModel = new RpSaleItemSimpleViewModel.SaleItemViewModel();           
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("SubMenuID", typeof(int)));
            for(int i = 0; i < lstSubMenuID.Count(); i++)
            {
                dt.Rows.Add(Convert.ToInt32(lstSubMenuID[i]));
            }
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleItemSimple, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@temptbl", dt);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                MainMenuModel = new RpSaleItemSimpleViewModel.MainMenuViewModel();
                MainMenuModel.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                MainMenuModel.MainMenuName = Convert.ToString(reader["MainMenuName"]);
                if(lstSaleItemSimpleRpt.Where(m=>m.MainMenuID==MainMenuModel.MainMenuID).Count()>0) // MainMenu Already exist Condition
                {
                    foreach(var MainMenuData in lstSaleItemSimpleRpt.Where(m => m.MainMenuID == MainMenuModel.MainMenuID))
                    {
                        SubMenuModel = new RpSaleItemSimpleViewModel.SubMenuViewModel();
                        SubMenuModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                        SubMenuModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);
                        if (MainMenuData.lstSubMenu.Where(m => m.SubMenuID == SubMenuModel.SubMenuID).Count() > 0)// SubMenu Already exist Condition
                        {
                            SaleItemModel = new RpSaleItemSimpleViewModel.SaleItemViewModel();
                            SaleItemModel.Code = Convert.ToString(reader["Code"]);
                            SaleItemModel.ProductName = Convert.ToString(reader["ProductName"]);
                            SaleItemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                            SaleItemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                            SaleItemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                            SaleItemModel.Discount = Convert.ToInt32(reader["Discount"]);
                            SaleItemModel.SalePrice = Convert.ToInt32(reader["SalePrice"]);                           
                            SaleItemModel.Amount = Convert.ToInt32(reader["Amount"]);
                            foreach(var SubMenuData in MainMenuData.lstSubMenu.Where(m => m.SubMenuID == SubMenuModel.SubMenuID))
                            {
                                SubMenuData.lstSaleItem.Add(SaleItemModel);
                            }                           
                        }
                        else
                        {                          
                            MainMenuData.lstSubMenu.Add(SubMenuModel);
                            SaleItemModel = new RpSaleItemSimpleViewModel.SaleItemViewModel();
                            SaleItemModel.Code = Convert.ToString(reader["Code"]);
                            SaleItemModel.ProductName = Convert.ToString(reader["ProductName"]);
                            SaleItemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                            SaleItemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                            SaleItemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                            SaleItemModel.Discount = Convert.ToInt32(reader["Discount"]);
                            SaleItemModel.SalePrice = Convert.ToInt32(reader["SalePrice"]);                            
                            SaleItemModel.Amount = Convert.ToInt32(reader["Amount"]);
                            foreach (var SubMenuData in MainMenuData.lstSubMenu.Where(m => m.SubMenuID == SubMenuModel.SubMenuID))
                            {
                                List<RpSaleItemSimpleViewModel.SaleItemViewModel> lstsaleitem = new List<RpSaleItemSimpleViewModel.SaleItemViewModel>();
                                lstsaleitem.Add(SaleItemModel);
                                SubMenuData.lstSaleItem=lstsaleitem;
                            }
                        }
                    }                                      
                }
                else
                {   //new MainMenu condition
                    lstSaleItemSimpleRpt.Add(MainMenuModel);
                    SubMenuModel = new RpSaleItemSimpleViewModel.SubMenuViewModel();
                    SubMenuModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                    SubMenuModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);                 
                    foreach(var MainMenuData in lstSaleItemSimpleRpt.Where(m => m.MainMenuID == MainMenuModel.MainMenuID))
                    {
                        List<RpSaleItemSimpleViewModel.SubMenuViewModel> lst = new List<RpSaleItemSimpleViewModel.SubMenuViewModel>();
                        lst.Add(SubMenuModel);
                        MainMenuData.lstSubMenu = lst;
                        SaleItemModel = new RpSaleItemSimpleViewModel.SaleItemViewModel();
                        SaleItemModel.Code = Convert.ToString(reader["Code"]);
                        SaleItemModel.ProductName = Convert.ToString(reader["ProductName"]);
                        SaleItemModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                        SaleItemModel.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                        SaleItemModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                        SaleItemModel.Discount = Convert.ToInt32(reader["Discount"]);
                        SaleItemModel.SalePrice = Convert.ToInt32(reader["SalePrice"]);                        
                        SaleItemModel.Amount = Convert.ToInt32(reader["Amount"]);
                        foreach(var SubMenuData in MainMenuData.lstSubMenu.Where(m => m.SubMenuID == SubMenuModel.SubMenuID))
                        {
                            List<RpSaleItemSimpleViewModel.SaleItemViewModel> lstsaleitem = new List<RpSaleItemSimpleViewModel.SaleItemViewModel>();
                            lstsaleitem.Add(SaleItemModel);
                            SubMenuData.lstSaleItem=lstsaleitem;
                        }
                    }
                }               
            }
            reader.Close();
            dataConnectorSQL.Close();
            int totalQuanity=0,totalAmount=0;
            foreach (var MainMenu in lstSaleItemSimpleRpt)
            {
                foreach(var SubMenu in MainMenu.lstSubMenu)
                {
                    totalQuanity+=SubMenu.lstSaleItem.Sum(m => m.Quantity);
                    totalAmount += SubMenu.lstSaleItem.Sum(m => m.Amount);
                }
            }
            saleItemSimpleViewModel.TotalQuantity = totalQuanity;
            saleItemSimpleViewModel.TotalAmount = totalAmount;
            return lstSaleItemSimpleRpt;
        }

        public List<SubMenuModels.SubMenuModel> getMenuData()
        {
            List<SubMenuModels.SubMenuModel> lst = new List<SubMenuModels.SubMenuModel>();
            SubMenuModels.SubMenuModel menu = new SubMenuModels.SubMenuModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMenuData, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
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
            dataConnectorSQL.Close();
            return lst;
        }
        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();

            connection = Session[AppConstants.SQLConnection];
            return connection;
        }

        public class ValList
        {
            public List<int> data { get; set; }
        }
    }
}
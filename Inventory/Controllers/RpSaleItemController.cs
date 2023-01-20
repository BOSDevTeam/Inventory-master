using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class RpSaleItemController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        RpSaleItemViewModel saleItemViewModel = new RpSaleItemViewModel();
        [SessionTimeoutAttribute]
        public ActionResult SaleItemReportFilter()
        {
            return View();
        }

        [SessionTimeoutAttribute]
        public ActionResult SaleItemReport(DateTime fromDate,DateTime toDate, bool isAll, bool isCash,bool isCredit)
        {
            List<RpSaleItemViewModel.MasterSaleViewModel> lstSaleItemRpt = GetSaleItemReport(fromDate, toDate, isAll, isCash, isCredit);
            saleItemViewModel.lstMasterSaleRpt = lstSaleItemRpt;
            saleItemViewModel.FromDate = fromDate;
            saleItemViewModel.ToDate = toDate;
            saleItemViewModel.IsAll = isAll;
            saleItemViewModel.IsCash = isCash;
            saleItemViewModel.IsCredit = isCredit;       
            return View(saleItemViewModel);
        }
        public List<RpSaleItemViewModel.MasterSaleViewModel>GetSaleItemReport(DateTime fromDate,DateTime toDate,bool isAll,bool isCash,bool isCredit)
        {
            string code = "", productName = "", unitKeyword = "";
            int unitID = 0;
            List<RpSaleItemViewModel.MasterSaleViewModel> lstSaleItemRpt = new List<RpSaleItemViewModel.MasterSaleViewModel>();          
            RpSaleItemViewModel.MasterSaleViewModel item = new RpSaleItemViewModel.MasterSaleViewModel();
            List<RpSaleItemViewModel.SaleItemViewModel> lst = new List<RpSaleItemViewModel.SaleItemViewModel>();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleItemList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            cmd.Parameters.AddWithValue("@isAll", isAll);
            cmd.Parameters.AddWithValue("@isCash", isCash);
            cmd.Parameters.AddWithValue("@isCredit", isCredit);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {               
                code= Convert.ToString(reader["Code"]);
                productName= Convert.ToString(reader["ProductName"]);
                unitID = Convert.ToInt32(reader["UnitID"]);
                unitKeyword= Convert.ToString(reader["UnitKeyword"]);
                if(lstSaleItemRpt.Where(m=>m.Code==code && m.ProductName==productName && m.UnitID==unitID && m.UnitKeyword==unitKeyword).Count()>0)
                {
                    foreach(var data in lstSaleItemRpt.Where(m => m.Code == code && m.ProductName == productName && m.UnitID == unitID && m.UnitKeyword == unitKeyword))
                    {
                        RpSaleItemViewModel.SaleItemViewModel model = new RpSaleItemViewModel.SaleItemViewModel();
                        model.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                        model.PaymentName = Convert.ToString(reader["Keyword"]);
                        model.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                        if (model.IsFOC) model.PaymentName = "FOC";
                        model.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                        model.Quantity = Convert.ToInt32(reader["Quantity"]);
                        model.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                        model.Discount = Convert.ToInt32(reader["Discount"]);                       
                        model.Amount = Convert.ToInt32(reader["Amount"]);
                        data.lstSaleItem.Add(model);
                    }                    
                }
                else
                {
                    item = new RpSaleItemViewModel.MasterSaleViewModel();
                    item.Code = code;
                    item.ProductName = productName;
                    item.UnitID = unitID;
                    item.UnitKeyword = unitKeyword;

                    lst = new List<RpSaleItemViewModel.SaleItemViewModel>();
                    RpSaleItemViewModel.SaleItemViewModel model = new RpSaleItemViewModel.SaleItemViewModel();
                    model.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                    model.PaymentName = Convert.ToString(reader["Keyword"]);
                    model.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                    if (model.IsFOC) model.PaymentName = "FOC";
                    model.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                    model.Quantity = Convert.ToInt32(reader["Quantity"]);
                    model.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                    model.Discount = Convert.ToInt32(reader["Discount"]);                    
                    model.Amount = Convert.ToInt32(reader["Amount"]);
                    lst.Add(model);             
                    item.lstSaleItem=lst;
                    lstSaleItemRpt.Add(item);
                }              
            }
            reader.Close();
            return lstSaleItemRpt;
        }

        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();

            connection = Session[AppConstants.SQLConnection];
            return connection;
        }
        private bool checkConnection()
        {
            if (Session[AppConstants.SQLConnection] != null) return true;
            else return false;
        }
    }
}
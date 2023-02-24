using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Common;
using Inventory.ViewModels;
using Inventory.Filters;
using Inventory.Models;

namespace Inventory.Controllers
{
    public class RpSaleAmountBySalePersonController : MyController
    {
        AppSetting setting = new AppSetting();
        RpSaleAmountBySalePersonViewModel saleAmountBySalePerson = new RpSaleAmountBySalePersonViewModel();
        [SessionTimeoutAttribute]
        public ActionResult SaleAmountBySalePersonReportFilter()
        {
            try
            {
                ViewBag.UserData = GetUserList();
                ViewBag.ClientData = GetClientList();
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View();
        }

        [SessionTimeoutAttribute]
        public ActionResult SaleAmountBySalePersonReport(DateTime fromDate, DateTime toDate)
        {
            try
            {
                string getVal = Request.QueryString["lstUserID"];
                string concat = @"{""data"":" + getVal + "}";
                ValList vl = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ValList>(concat);
                ValList lstUser = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ValList>(concat);

                string getClientVal = Request.QueryString["lstClientID"];
                string concatID = @"{""data"":" + getClientVal + "}";
                ValList vlClient = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ValList>(concatID);
                ValList lstClient = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ValList>(concatID);
                saleAmountBySalePerson.lstSalePerson = GetSaleAmountBySalePersonReport(fromDate, toDate, lstUser.data, lstClient.data);
                saleAmountBySalePerson.FromDate = fromDate;
                saleAmountBySalePerson.ToDate = toDate;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(saleAmountBySalePerson);
        }

        public List<RpSaleAmountBySalePersonViewModel.SalePerson> GetSaleAmountBySalePersonReport(DateTime fromDate,DateTime toDate, List<int>lstUserID,List<int>lstClientID)
        {
            List<RpSaleAmountBySalePersonViewModel.SalePerson> lstSaleAmountBySalePerson = new List<RpSaleAmountBySalePersonViewModel.SalePerson>();
            RpSaleAmountBySalePersonViewModel.SalePerson SalePerson = new RpSaleAmountBySalePersonViewModel.SalePerson();
            RpSaleAmountBySalePersonViewModel.SaleItem item = new RpSaleAmountBySalePersonViewModel.SaleItem();
            DataTable tblUser = new DataTable();
            tblUser.Columns.Add(new DataColumn("UserID", typeof(int)));
            for (int i = 0; i < lstUserID.Count(); i++)
            {
                tblUser.Rows.Add(Convert.ToInt32(lstUserID[i]));
            }
            DataTable tblClient = new DataTable();
            tblClient.Columns.Add(new DataColumn("ClientID", typeof(int)));
            for (int i = 0; i < lstClientID.Count(); i++)
            {
                tblClient.Rows.Add(Convert.ToInt32(lstClientID[i]));
            }

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptSaleAmountBySalePerson, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@TempUsertbl", tblUser);
            cmd.Parameters.AddWithValue("@TempClienttbl", tblClient);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                SalePerson = new RpSaleAmountBySalePersonViewModel.SalePerson();
                SalePerson.SalePersonID = Convert.ToInt32(reader["UserID"]);
                if (SalePerson.SalePersonID > 0)
                {
                    if (lstSaleAmountBySalePerson.Where(m => m.SalePersonID == SalePerson.SalePersonID).Count() > 0)
                    {
                        item = new RpSaleAmountBySalePersonViewModel.SaleItem();
                        item.SaleID = Convert.ToInt32(reader["SaleID"]);
                        item.SaleDateTime = Convert.ToDateTime(reader["SaleDateTime"]);
                        item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                        item.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                        item.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                        item.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                        item.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                        item.VouDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                        item.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                        item.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                        item.VouFOC = Convert.ToInt32(reader["VoucherFOC"]);
                        item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                        foreach (var person in lstSaleAmountBySalePerson.Where(m => m.SalePersonID == SalePerson.SalePersonID))
                        {
                            person.lstSaleItem.Add(item);
                        }
                    }
                    else
                    {
                        SalePerson.SalePersonName = Convert.ToString(reader["UserName"]);
                        List<RpSaleAmountBySalePersonViewModel.SaleItem> lst = new List<RpSaleAmountBySalePersonViewModel.SaleItem>();
                        item = new RpSaleAmountBySalePersonViewModel.SaleItem();
                        item.SaleID = Convert.ToInt32(reader["SaleID"]);
                        item.SaleDateTime = Convert.ToDateTime(reader["SaleDateTime"]);
                        item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                        item.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                        item.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                        item.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                        item.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                        item.VouDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                        item.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                        item.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                        item.VouFOC = Convert.ToInt32(reader["VoucherFOC"]);
                        item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                        lst.Add(item);
                        SalePerson.lstSaleItem = lst;
                        lstSaleAmountBySalePerson.Add(SalePerson);
                    }
                }
                else
                {
                    SalePerson.SalePersonID2 = Convert.ToInt32(reader["ClientID"]);
                    if (lstSaleAmountBySalePerson.Where(m => m.SalePersonID2 == SalePerson.SalePersonID2).Count() > 0)
                    {
                        item = new RpSaleAmountBySalePersonViewModel.SaleItem();
                        item.SaleID = Convert.ToInt32(reader["SaleID"]);
                        item.SaleDateTime = Convert.ToDateTime(reader["SaleDateTime"]);
                        item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                        item.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                        item.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                        item.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                        item.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                        item.VouDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                        item.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                        item.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                        item.VouFOC = Convert.ToInt32(reader["VoucherFOC"]);
                        item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                        foreach(var person in lstSaleAmountBySalePerson.Where(m => m.SalePersonID2 == SalePerson.SalePersonID2))
                        {
                            person.lstSaleItem.Add(item);
                        }
                    }
                    else
                    {
                        SalePerson.SalePersonName2 = Convert.ToString(reader["ClientName"]);
                        List<RpSaleAmountBySalePersonViewModel.SaleItem> lst = new List<RpSaleAmountBySalePersonViewModel.SaleItem>();
                        item = new RpSaleAmountBySalePersonViewModel.SaleItem();
                        item.SaleID = Convert.ToInt32(reader["SaleID"]);
                        item.SaleDateTime = Convert.ToDateTime(reader["SaleDateTime"]);
                        item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                        item.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                        item.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                        item.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                        item.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                        item.VouDiscount = Convert.ToInt32(reader["VoucherDiscount"]);
                        item.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                        item.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                        item.VouFOC = Convert.ToInt32(reader["VoucherFOC"]);
                        item.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);
                        lst.Add(item);
                        SalePerson.lstSaleItem = lst;
                        lstSaleAmountBySalePerson.Add(SalePerson);
                    }
                }                  
            }
            reader.Close();
            setting.conn.Close();
            int totalCash = 0, totalCredit = 0, totalTaxAmt = 0, totalChargesAmt = 0,totalVouDis=0,totalAdvanPay=0,totalPaypercent=0,totalVouFoc=0,totalGrand=0 ;
            foreach(var person in lstSaleAmountBySalePerson)
            {
                totalCash += person.lstSaleItem.Where(m=>m.PaymentID==1).Sum(m => m.Subtotal);
                totalCredit += person.lstSaleItem.Where(m => m.PaymentID == 2).Sum(m => m.Subtotal);
                totalTaxAmt += person.lstSaleItem.Sum(m => m.TaxAmt);
                totalChargesAmt += person.lstSaleItem.Sum(m => m.ChargesAmt);
                totalVouDis += person.lstSaleItem.Sum(m => m.VouDiscount);
                totalAdvanPay += person.lstSaleItem.Sum(m => m.AdvancedPay);
                totalPaypercent += person.lstSaleItem.Sum(m => m.PayPercentAmt);
                totalVouFoc += person.lstSaleItem.Sum(m => m.VouFOC);
                totalGrand += person.lstSaleItem.Sum(m => m.Grandtotal);
            }
            saleAmountBySalePerson.TotalCash = totalCash;
            saleAmountBySalePerson.TotalCredit = totalCredit;
            saleAmountBySalePerson.TotalTaxAmt = totalTaxAmt;
            saleAmountBySalePerson.TotalChargesAmt = totalChargesAmt;
            saleAmountBySalePerson.TotalVouDis = totalVouDis;
            saleAmountBySalePerson.TotalAdvancedPay = totalAdvanPay;
            saleAmountBySalePerson.TotalPaypercentAmt = totalPaypercent;
            saleAmountBySalePerson.TotalVouFOC = totalVouFoc;
            saleAmountBySalePerson.TotalGrandTotal = totalGrand;
            return lstSaleAmountBySalePerson;
        }

        public List<UserModels.UserModel> GetUserList()
        {
            List<UserModels.UserModel> lstUser = new List<UserModels.UserModel>();
            UserModels.UserModel user = new UserModels.UserModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetUser, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                user = new UserModels.UserModel();
                user.UserID = Convert.ToInt32(reader["UserID"]);
                user.UserName = Convert.ToString(reader["UserName"]);
                lstUser.Add(user);
            }
            reader.Close();
            setting.conn.Close();
            return lstUser;
        }
        public List<ClientModels> GetClientList()
        {
            List<ClientModels> lstClient = new List<ClientModels>();
            ClientModels client = new ClientModels();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(TextQuery.clientSalePersonQuery, setting.conn);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                client = new ClientModels();
                client.ClientID = Convert.ToInt32(reader["ClientID"]);
                client.ClientName = Convert.ToString(reader["ClientName"]);
                lstClient.Add(client);
            }
            reader.Close();
            setting.conn.Close();
            return lstClient;
        }
        public class ValList
        {
            public List<int> data { get; set; }
        }
    }
}
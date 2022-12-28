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
using System.Runtime.InteropServices;

namespace Inventory.Controllers
{
    public class SaleReturnController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        MasterSaleReturnViewModel saleReturnViewModel = new MasterSaleReturnViewModel();
        AppData appData = new AppData();
        AppSetting appSetting = new AppSetting();
        TextQuery textQuery = new TextQuery();
        #region Page
        public ActionResult SaleReturn(int userId, int? saleReturnId)
        {
            if (checkConnection()) {
                ViewBag.PaymentID = 
                ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                clearTranSaleReturn();
                int totalQuantity = 0;
                if (saleReturnId != null) // saleReturn edit
                {
                    ViewBag.IsEdit = true;
                    MasterSaleReturnViewModel data = selectMasterSaleReturnBySaleReturnID((int)saleReturnId);
                    List<TranSaleReturnModels> lstTranSaleReturn = selectTranSaleReturnBySaleReturnID((int)saleReturnId);
                    for (int i = 0; i < lstTranSaleReturn.Count(); i++)
                    {
                        totalQuantity += lstTranSaleReturn[i].Quantity;
                    }
                    Session["TranSaleReturnData"] = lstTranSaleReturn;
                    ViewBag.TotalItem = lstTranSaleReturn.Count();
                    ViewBag.UserVoucherNo = data.UserVoucherNo;
                    DateTime date = appSetting.convertStringToDate(data.ReturnDateTime);
                    ViewBag.Date = appSetting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterSaleReturnModel.VoucherID;
                    ViewBag.LocationID = data.LocationID;
                    ViewBag.Total = data.Total;
                    ViewBag.TotalQuantity = totalQuantity;
                    ViewBag.SaleReturnID = saleReturnId;
                }

            }
            return View();
        }

        public ActionResult ListSaleReturn(int userId)
        {
            List<MasterSaleReturnViewModel> list = selectMasterSaleReturn(userId, false);
            ViewData["LstMasterSaleReturn"] = list;
            return View(saleReturnViewModel);
        }
        #endregion

        #region Action
        [HttpPost]
        public JsonResult SaveAction(string returnVoucherNo, string userVoucherNo, string date, string voucherId, int userId, int locationId, int totalAmount, string remark)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranSaleReturnData"] != null)
            {

                resultDefaultData.IsRequestSuccess = true;
                List<TranSaleReturnModels> list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
                DataTable dt = new DataTable();
                dt.Columns.Add("ProductID", typeof(int));
                dt.Columns.Add("Quantity", typeof(int));
                dt.Columns.Add("UnitID", typeof(int));
                dt.Columns.Add("SalePrice", typeof(int));
                dt.Columns.Add("CurrencyID", typeof(int));
                dt.Columns.Add("DiscountPercent", typeof(int));
                dt.Columns.Add("Discount", typeof(int));
                dt.Columns.Add("Amount", typeof(int));
                dt.Columns.Add("IsFOC", typeof(bool));
                for (int i = 0; i < list.Count; i++)
                {
                    dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].SalePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                }
                DateTime saleReturnDateTime = DateTime.Parse(date);
                SqlCommand cmd = new SqlCommand(Procedure.PrcInsertSaleReturn, dataConnectorSQL.Connect());

                cmd.Parameters.AddWithValue("@ReturnVoucherNo", returnVoucherNo);
                cmd.Parameters.AddWithValue("@ReturnDateTime", saleReturnDateTime);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@Remark", remark);
                cmd.Parameters.AddWithValue("@Total", totalAmount);
                cmd.Parameters.AddWithValue("@temptbl", dt);
                cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.SaleReturnModule);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    userVoucherNo = Convert.ToString(reader[0]);
                }

                reader.Close();
                dataConnectorSQL.Close();
                clearTranSaleReturn();

            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                UserVoucherNo = userVoucherNo
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult SearchActionByPaymentID(int paymentId, string keyword)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels item = new TranSaleReturnModels();
            int totalQuantity = 0;
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterANDTranSaleByPaymentID, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", keyword);
            cmd.Parameters.AddWithValue("@PaymentID", paymentId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranSaleReturnModels();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                item.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.SaleID = Convert.ToInt32(reader["SaleID"]);
                list.Add(item); 

            }
            reader.Close();

            for (int i = 0; i < list.Count; i++)
            {
                totalQuantity += list[i].Quantity;
            }

            Session["TranSaleData"] = list;
            var jsonResult = new
            {
                List = list,
                TotalQuantity = totalQuantity,
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaleClickAction(int number)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels data = new TranSaleReturnModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            string productName = "", unitName = "", currencyKeyword ="" ;
            int productId = 0, quantity = 0, price = 0, disPercent = 0,locationId = 0,
                unitId = 0, currencyId = 0, saleId = 0;


            if (Session["TranSaleData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    list = Session["TranSaleData"] as List<TranSaleReturnModels>;
                    data = list[number - 1];
                    productId = data.ProductID;
                    productName = data.ProductName;
                    unitId = data.UnitID;
                    unitName = data.UnitKeyword;
                    quantity = data.Quantity;
                    price = data.SalePrice;
                    currencyId = data.CurrencyID;
                    currencyKeyword = data.CurrencyKeyword;
                    disPercent = data.DiscountPercent;
                    locationId = data.LocationID;
                    saleId = data.SaleID;
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }

            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            var jsonResult = new
            {
                ProductID = productId,
                ProductName = productName,
                Quantity = quantity,
                UnitID = unitId,
                UnitName = unitName,
                SalePrice = price,
                CurrencyID = currencyId,
                CurrencyKeyword = currencyKeyword,
                DisPercent = disPercent,
                LocationID = locationId,
                SaleID = saleId,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranSaleReturnAddEditAction(int productId, int locationId, string productName, int quantity,int price,int unitId, string unitKeyword, int currencyId, string currencyKeyword, bool isEdit, int? number)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels data = new TranSaleReturnModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalAmount = 0;
            data.ProductID = productId;
            data.LocationID = locationId;
            data.ProductName = productName;
            data.Quantity = quantity;
            data.UnitID = unitId;
            data.UnitKeyword = unitKeyword;
            data.SalePrice = price;
            data.CurrencyID = currencyId;
            data.CurrencyKeyword = currencyKeyword;
            data.Amount = (quantity * price);
            if (!isEdit)
            {
                if (Session["TranSaleReturnData"] != null)
                {
                    list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
                    list.Add(data);
                }
                else
                {
                    list.Add(data);
                }
                resultDefaultData.IsRequestSuccess = true;

            }
            else {
                if (Session["TranSaleReturnData"] != null)
                {
                    try
                    {
                        resultDefaultData.IsRequestSuccess = true;
                        list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
                        int index = (int)number - 1;
                        list[index] = data;

                    }
                    catch (Exception ex)
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                        resultDefaultData.Message = ex.Message;
                    }

                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }

            for (int i = 0; i < list.Count(); i++)
            {
                totalAmount += list[i].Amount;
            }

            Session["TranSaleReturnData"] = list;

            var jsonResult = new
            {
                List = list,
                TotalAmount = totalAmount,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrepareToEditTranSaleReturnAction(int number)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels data = new TranSaleReturnModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            string productName = "", unitName = "", currencyKeyword = "";
            int productId = 0, locationId = 0,quantity = 0, price = 0, disPercent = 0;
            if (Session["TranSaleReturnData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
                    data = list[number - 1];
                    productId = data.ProductID;
                    locationId = data.LocationID;
                    productName = data.ProductName;
                    quantity = data.Quantity;
                    unitName = data.UnitKeyword;
                    price = data.SalePrice;
                    currencyKeyword = data.CurrencyKeyword;
                    disPercent = data.DiscountPercent;

                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }

            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ProductID = productId,
                LocationID = locationId,
                ProductName = productName,
                Quantity = quantity,
                UnitKeyword = unitName,
                SalePrice = price,
                CurrencyKeyword = currencyKeyword,
                DisPercent = disPercent
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranSaleReturnDeleteAction(int number)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels data = new TranSaleReturnModels();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalAmount = 0;
            if (Session["TranSaleReturnData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    list = Session["TranSaleReturnData"] as List<TranSaleReturnModels>;
                    list.RemoveAt(number - 1);
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }


            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            for (int i = 0; i < list.Count(); i++)
            {
                totalAmount += list[i].Amount;
            }

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                List = list,
                TotalAmount = totalAmount
            };


            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelAction()
        {
            Session["TranSaleReturnData"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }


        public JsonResult SearchAction(int userId, bool isSearch, DateTime fromDate, DateTime toDate, string userVoucherNo)
        {
            List<MasterSaleReturnViewModel> list = selectMasterSaleReturn(userId, isSearch, fromDate, toDate, userVoucherNo);
            var jsonResult = new
            {
                List = list
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshAction(int userId, bool isSearch)
        {
            List<MasterSaleReturnViewModel> list = selectMasterSaleReturn(userId, isSearch);
            var jsonResult = new
            {
                List = list
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ViewAction(int saleReturnId)
        {
            MasterSaleReturnViewModel item = selectMasterSaleReturnBySaleReturnID(saleReturnId);
            List<TranSaleReturnModels> lstTranSaleReturn = selectTranSaleReturnBySaleReturnID(saleReturnId);
            var jsonResult = new
            {
                LstTranSaleReturn = lstTranSaleReturn,
                List = item,
                UserVoucherNo = item.UserVoucherNo,
                VoucherID = item.MasterSaleReturnModel.VoucherID,
                LocationName = item.ShortName,
                ReturnDateTime = item.ReturnDateTime,
                User = item.UserName,
                Total = item.Total,
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAction(int saleReturnId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            int totalPageNum = 0;
            if (Session["MasterSaleReturnData"] != null)
            {
                try
                {
                    resultDefaultData.IsRequestSuccess = true;
                    //SqlCommand cmd = new SqlCommand(textQuery.deleteSaleReturnQuery(saleReturnId), (SqlConnection)getConnection());
                    //cmd.CommandType = CommandType.Text;
                    //cmd.ExecuteNonQuery();
                    List<MasterSaleReturnViewModel> lstMasterSaleReturn = Session["MasterSaleReturnData"] as List<MasterSaleReturnViewModel>;
                    int index = lstMasterSaleReturn.FindIndex(x => x.SaleReturnID == saleReturnId);
                    lstMasterSaleReturn.RemoveAt(index);
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;

                }

            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                TotalPage = totalPageNum,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region Method

        private MasterSaleReturnViewModel selectMasterSaleReturnBySaleReturnID(int saleReturnId)
        {
            List<MasterSaleReturnViewModel> list = new List<MasterSaleReturnViewModel>();
            MasterSaleReturnViewModel data = new MasterSaleReturnViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleReturnBySaleReturnID, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleReturnID", saleReturnId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                data.SaleReturnID = Convert.ToInt32(reader["SaleReturnID"]);
                data.ReturnDateTime = Convert.ToString(reader["Date"]);
                data.ReturnVoucherNo = Convert.ToString(reader["ReturnVoucherNo"]);
                data.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                data.MasterSaleReturnModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                data.UserName = Convert.ToString(reader["UserName"]);
                data.ShortName = Convert.ToString(reader["LocationName"]);
                data.Total = Convert.ToInt32(reader["Total"]);
            }

            reader.Close();
            
            return data;
        }

        private List<TranSaleReturnModels> selectTranSaleReturnBySaleReturnID(int saleReturnId)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels data = new TranSaleReturnModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranSaleReturnBySaleReturnID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SaleReturnID", saleReturnId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                data.ProductID = Convert.ToInt32(reader["ProductID"]);
                data.ProductName = Convert.ToString(reader["ProductName"]);
                data.Quantity = Convert.ToInt32(reader["Quantity"]);
                data.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                data.Amount = Convert.ToInt32(reader["Amount"]);
                data.UnitID = Convert.ToInt32(reader["UnitID"]);
                data.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                data.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                data.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                data.Discount = Convert.ToInt32(reader["Discount"]);
                data.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                list.Add(data);
            }

            reader.Close();

            return list;
        }


        private List<MasterSaleReturnViewModel> selectMasterSaleReturn(int userId, bool isSearch, [Optional]DateTime fromDate,[Optional]DateTime toDate,[Optional]string userVoucherNo)
        {
            List<MasterSaleReturnViewModel> list = new List<MasterSaleReturnViewModel>();
            MasterSaleReturnViewModel data = new MasterSaleReturnViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterSaleReturnList, (SqlConnection) getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@IsSearch", isSearch);
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", appSetting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", appSetting.getLocalDate());
                cmd.Parameters.AddWithValue("@UserVoucherNo", "");
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
            }
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data = new MasterSaleReturnViewModel();
                data.SaleReturnID = Convert.ToInt32(reader["SaleReturnID"]);
                data.MasterSaleReturnModel.UserID = Convert.ToInt32(reader["UserID"]);
                data.MasterSaleReturnModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                data.MasterSaleReturnModel.Remark = Convert.ToString(reader["Remark"]);
                data.ReturnDateTime = Convert.ToString(reader["ReturnDateTime"]);
                data.ReturnVoucherNo = Convert.ToString(reader["ReturnVoucherNo"]);
                data.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                data.Total = Convert.ToInt32(reader["Total"]);
                data.LocationID = Convert.ToInt32(reader["LocationID"]);
                data.ShortName = Convert.ToString(reader["ShortName"]);
                list.Add(data);
            }
            reader.Close();
            Session["MasterSaleReturnData"] = list;
            return list;
        }


        private void clearTranSaleReturn()
        {
            Session["TranSaleReturnData"] = null;

        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.SaleReturnModule, userId, getConnection());
            return userVoucherNo;
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
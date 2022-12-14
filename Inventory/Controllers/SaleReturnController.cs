using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;


namespace Inventory.Controllers
{
    public class SaleReturnController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        AppData appData = new AppData();
        #region Page
        public ActionResult SaleReturn(int userId)
        {
            if (checkConnection()) {

                ViewBag.UserVoucherNo = getUserVoucherNo(userId);
            }
            return View();
        }

        public ActionResult ListSaleReturn()
        {
            return View();
        }
        #endregion

        #region Action
        [HttpGet]
        public JsonResult SearchActionByPaymentID(int paymentId, string keyword)
        {
            List<TranSaleReturnModels> list = new List<TranSaleReturnModels>();
            TranSaleReturnModels item = new TranSaleReturnModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGETMasterANDTranSaleByPaymentID, (SqlConnection) getConnection());
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
                list.Add(item); 

            }
            reader.Close();
            var jsonResult = new
            {
                List = list
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Method

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
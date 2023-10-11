using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using Inventory.Common;
using Inventory.Models;
using System.Data.SqlClient;
using System.Data;

namespace Inventory.Controllers
{
    public class PaymentDialogController : MyController
    {
        AppData appData = new AppData();
        TextQuery textQuery = new TextQuery();
        AppSetting setting = new AppSetting();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();

        #region events

        [HttpGet]
        public JsonResult PaymentAction(bool isBankPayment, int moduleCode)
        {
            List<PaymentModels> lstPayment = new List<PaymentModels>();
            List<PayMethodModels> lstPayMethod = new List<PayMethodModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if ((moduleCode == AppConstants.SaleModule && Session["TranSaleData"] != null) || (moduleCode == AppConstants.PurchaseModule && Session["TranPurchaseData"] != null))
            {
                try
                {
                    lstPayment = getPayment();
                    if (isBankPayment) lstPayMethod = getPayMethod();
                    resultDefaultData.IsRequestSuccess = true;
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
                LstPayment = lstPayment,
                LstPayMethod = lstPayMethod,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PaymentEditAction(bool isBankPayment, int moduleCode, int saleId)
        {
            List<PaymentModels> lstPayment = new List<PaymentModels>();
            List<PayMethodModels> lstPayMethod = new List<PayMethodModels>();
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            MasterSaleModels masterSaleModel = new MasterSaleModels();
            List<LimitedDayModels> lstLimitedDay = new List<LimitedDayModels>();
            List<BankPaymentModels> lstBankPayment = new List<BankPaymentModels>();

            if ((moduleCode == AppConstants.SaleModule && Session["TranSaleData"] != null) || (moduleCode == AppConstants.PurchaseModule && Session["TranPurchaseData"] != null))
            {
                try
                {
                    lstPayment = getPayment();
                    if (isBankPayment) lstPayMethod = getPayMethod();
                    
                    if (moduleCode == AppConstants.SaleModule)
                    {
                        masterSaleModel = getSalePayment(saleId);
                    }
                    
                    resultDefaultData.IsRequestSuccess = true;
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
                LstPayment = lstPayment,
                LstPayMethod = lstPayMethod,
                MasterSaleModel = masterSaleModel,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLimitedDayAction()
        {
            List<LimitedDayModels> lstLimitedDay = new List<LimitedDayModels>();
            lstLimitedDay = getLimitedDay();
            return Json(lstLimitedDay, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBankPaymentAction()
        {
            List<BankPaymentModels> lstBankPayment = new List<BankPaymentModels>();
            lstBankPayment = getBankPayment();
            return Json(lstBankPayment, JsonRequestBehavior.AllowGet);
        }       

        #endregion

        #region methods

        public MasterSaleModels getSalePayment(int saleId)
        {
            MasterSaleModels model = new MasterSaleModels();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(textQuery.getSalePaymentQuery(saleId), setting.conn);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                model.PayMethodID = Convert.ToInt32(reader["PayMethodID"]);
                model.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
                model.LimitedDayID = Convert.ToInt32(reader["LimitedDayID"]);
                model.Remark = Convert.ToString(reader["Remark"]);
                model.VouDisPercent = Convert.ToInt32(reader["VouDisPercent"]);
                model.VouDisAmount = Convert.ToInt32(reader["VouDisAmount"]);
                model.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                model.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);
                model.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
            }
            reader.Close();

            if(model.PayMethodID == 3)
            {
                model.IsMultiPay = true;
                cmd = new SqlCommand(textQuery.getMultiPayCashInHand(saleId), setting.conn);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = setting.conn;
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.CashInHand = Convert.ToInt32(reader["Amount"]);
                }
                reader.Close();
            }
            setting.conn.Close();
            return model;
        }

        //private void getMultiBankPay(int saleId)
        //{
        //    List<MultiPayMethodSaleModels> lstMultiBankPay = new List<MultiPayMethodSaleModels>();
        //    MultiPayMethodSaleModels multiBankPay;

        //    setting.conn.Open();
        //    SqlCommand cmd = new SqlCommand(textQuery.getMultiPayBanking(saleId), setting.conn);
        //    cmd.CommandType = CommandType.Text;
        //    cmd.Connection = setting.conn;
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        multiBankPay = new MultiPayMethodSaleModels();
        //        multiBankPay.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
        //        multiBankPay.BankPaymentName = Convert.ToString(reader["BankPaymentName"]);
        //        multiBankPay.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);
        //        multiBankPay.Amount = Convert.ToInt32(reader["Amount"]);
        //        //model.BankingTotal += multiBankPay.Amount;
        //        lstMultiBankPay.Add(multiBankPay);
        //    }
        //    reader.Close();
        //    Session["MultiPayMethodData"] = lstMultiBankPay;
        //}

        private List<PaymentModels> getPayment()
        {
            List<PaymentModels> list = new List<PaymentModels>();
            if (Session["PaymentData"] == null)
            {
                list = appData.selectPayment(getConnection());
                Session["PaymentData"] = list;
            }
            else list = Session["PaymentData"] as List<PaymentModels>;
            return list;
        }

        private List<PayMethodModels> getPayMethod()
        {
            List<PayMethodModels> list = new List<PayMethodModels>();
            if (Session["PayMethodData"] == null)
            {
                list = appData.selectPayMethod(getConnection());
                Session["PayMethodData"] = list;
            }
            else list = Session["PayMethodData"] as List<PayMethodModels>;
            return list;
        }

        private List<LimitedDayModels> getLimitedDay()
        {
            List<LimitedDayModels> list = new List<LimitedDayModels>();
            if (Session["LimitedDayData"] == null)
            {
                list = appData.selectLimitedDay(getConnection());
                Session["LimitedDayData"] = list;
            }
            else list = Session["LimitedDayData"] as List<LimitedDayModels>;
            return list;
        }

        private List<BankPaymentModels> getBankPayment()
        {
            List<BankPaymentModels> list = new List<BankPaymentModels>();
            if (Session["BankPaymentData"] == null)
            {
                list = appData.selectBankPayment(getConnection());
                Session["BankPaymentData"] = list;
            }
            else list = Session["BankPaymentData"] as List<BankPaymentModels>;
            return list;
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
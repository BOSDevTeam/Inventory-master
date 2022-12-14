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
    public class PaymentDialogController : MyController
    {
        AppData appData = new AppData();
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
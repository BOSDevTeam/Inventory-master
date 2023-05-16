using Inventory.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class SaleVoucherDesignController : MyController
    {
        TextQuery textQuery = new TextQuery();
        AppSetting setting = new AppSetting();

        public ActionResult SaleVoucherDesign()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveSaleVoucherDesignAction(short saleVoucherDesignType)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            try
            {
                setting.conn.Open();
                SqlCommand cmd = new SqlCommand(textQuery.updateSaleVoucherDesignQuery(saleVoucherDesignType), setting.conn);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = setting.conn;
                cmd.ExecuteNonQuery();
                setting.conn.Close();
                resultDefaultData.IsRequestSuccess = true;
                resultDefaultData.Message = AppConstants.Message.SaveSuccess;
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}
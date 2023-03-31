using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using System.Data;
using System.Data.SqlClient;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class GeneralSettingController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        TechnicianSettingModels model = new TechnicianSettingModels();
        Procedure procedure = new Procedure();

        public ActionResult GeneralSetting()
        {
            getShopType();
            getCompanySetting();
            return View(model);
        }

        [HttpPost]
        public JsonResult SaveShopTypeAction(string shopTypeId)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcSaveShopType, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShopTypeID", Convert.ToInt32(shopTypeId));
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("");
        }

        [HttpPost]
        public JsonResult SaveOtherSettingAction(string isMultiCurrency, string isMultiUnit, string isBankPayment,string isClientPhoneVerify, string isLimitUser, short limitedUserCount)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcSaveOtherSetting, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;           
            cmd.Parameters.AddWithValue("@IsMultiCurrency", Convert.ToBoolean(isMultiCurrency));
            cmd.Parameters.AddWithValue("@IsMultiUnit", Convert.ToBoolean(isMultiUnit));
            cmd.Parameters.AddWithValue("@IsBankPayment", Convert.ToBoolean(isBankPayment));
            cmd.Parameters.AddWithValue("@IsClientPhoneVerify", Convert.ToBoolean(isClientPhoneVerify));
            cmd.Parameters.AddWithValue("@IsLimitUser", Convert.ToBoolean(isLimitUser));
            cmd.Parameters.AddWithValue("@LimitedUserCount", Convert.ToInt16(limitedUserCount));
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("");
        }

        private void getShopType()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select ID,ShopType From SysShopType", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.ShopTypes.Add(new SelectListItem { Text = Convert.ToString(reader["ShopType"]), Value = Convert.ToString(reader["ID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void getCompanySetting()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetCompanySetting, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {              
                ViewBag.IsMultiUnit = reader["IsMultiUnit"];
                ViewBag.IsMultiCurrency = reader["IsMultiCurrency"];
                ViewBag.IsBankPayment = reader["IsBankPayment"];
                ViewBag.IsClientPhoneVerify = reader["IsClientPhoneVerify"];              
                ViewBag.ShopTypeID = reader["ShopTypeID"].ToString();
                ViewBag.IsLimitUser = reader["IsLimitUser"];
                ViewBag.LimitedUserCount = reader["LimitedUserCount"];
            }
            reader.Close();
            dataConnectorSQL.Close();
        }
    }
}
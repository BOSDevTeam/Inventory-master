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
    public class TechnicianController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        TechnicianSettingModels model = new TechnicianSettingModels();
        Procedure procedure = new Procedure();

        public ActionResult TechnicianSetting()
        {
            getShopType();
            getCompanySetting();
            return View(model);
        }

        [HttpPost]
        public JsonResult SaveShopTypeAction(string shopTypeId)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcSaveShopType, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShopTypeID", Convert.ToInt32(shopTypeId));
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("");
        }

        [HttpPost]
        public JsonResult SaveOtherSettingAction(string isMultiBranch, string isMultiCurrency, string isMultiUnit, string isBankPayment, string isProductPhoto, string isProductColor, string isProductSize)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcSaveOtherSetting, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IsMultiBranch", Convert.ToBoolean(isMultiBranch));
            cmd.Parameters.AddWithValue("@IsMultiCurrency", Convert.ToBoolean(isMultiCurrency));
            cmd.Parameters.AddWithValue("@IsMultiUnit", Convert.ToBoolean(isMultiUnit));
            cmd.Parameters.AddWithValue("@IsBankPayment", Convert.ToBoolean(isBankPayment));
            cmd.Parameters.AddWithValue("@IsProductPhoto", Convert.ToBoolean(isProductPhoto));
            cmd.Parameters.AddWithValue("@IsProductColor", Convert.ToBoolean(isProductColor));
            cmd.Parameters.AddWithValue("@IsProductSize", Convert.ToBoolean(isProductSize));
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("");
        }

        private void getShopType()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select ID,ShopType From Sys_ShopType", (SqlConnection)Session["SQLConnection"]);
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
            SqlCommand cmd = new SqlCommand(procedure.PrcGetCompanySetting, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                ViewBag.IsMultiBranch = reader["IsMultiBranch"];
                ViewBag.IsMultiUnit = reader["IsMultiUnit"];
                ViewBag.IsMultiCurrency = reader["IsMultiCurrency"];
                ViewBag.IsBankPayment = reader["IsBankPayment"];
                ViewBag.IsProductPhoto = reader["IsProductPhoto"];
                ViewBag.IsProductColor = reader["IsProductColor"];
                ViewBag.IsProductSize = reader["IsProductSize"];
                ViewBag.ShopTypeID = reader["ShopTypeID"].ToString();
            }
            reader.Close();
            dataConnectorSQL.Close();
        }
    }
}
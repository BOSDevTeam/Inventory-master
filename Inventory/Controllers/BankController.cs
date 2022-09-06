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
    public class BankController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        BankModels model = new BankModels();
        Procedure procedure = new Procedure();

        [HttpGet]
        public ActionResult BankEntry(int bankId)
        {          
            if (bankId != 0 && Session["LstBank"] != null)  // edit
            {
                ViewBag.BankID = bankId;
                List<BankModels> LstBank = Session["LstBank"] as List<BankModels>;
                var editBank = LstBank.Where(c => c.BankID == bankId);
                foreach (var e in editBank)
                {
                    ViewBag.Name = e.Name;
                    ViewBag.ShortName = e.ShortName;
                    ViewBag.IsEdit = 1;
                }
            }
            else
            {
                ViewBag.IsEdit = 0;
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult BankList(int p)
        {          
            getBankList(p, "");
            return View(model);
        }

        private void getBankList(int currentPage, string searchValue)
        {
            List<BankModels> tempList = new List<BankModels>();
            BankModels bankModel = new BankModels();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcSearchBank, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", searchValue);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                bankModel = new BankModels();
                bankModel.BankID = Convert.ToInt32(reader["BankID"]);
                bankModel.Name = Convert.ToString(reader["BankName"]);
                bankModel.ShortName = Convert.ToString(reader["ShortName"]);

                model.LstBank.Add(bankModel);
                Session["LstBank"] = model.LstBank;
                //tempList.Add(bankModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            //if (tempList.Count > common.pageSize)
            //{
            //    model.TotalPageNum = tempList.Count / common.pageSize;
            //    common.left = tempList.Count % common.pageSize;
            //    if (common.left != 0) model.TotalPageNum += 1;

            //    int i = currentPage * common.pageSize;
            //    int j = (i - common.pageSize) + 1;
            //    int start = j;
            //    int end = i;
            //    common.startRowIndex = start - 1;
            //    common.endRowIndex = end - 1;
            //    Session["PageNumber"] = "Page : " + currentPage;
            //}
            //else
            //{
            //    common.startRowIndex = 0;
            //    common.endRowIndex = tempList.Count - 1;
            //    Session["PageNumber"] = "";
            //}

            //for (int p = common.startRowIndex; p < tempList.Count; p++)
            //{
            //    if (p > common.endRowIndex) break;

            //    leagueModel = new LeagueModels();
            //    leagueModel.ID = tempList[p].ID;
            //    leagueModel.LeagueName = tempList[p].LeagueName;
            //    leagueModel.LeagueSort = tempList[p].LeagueSort;
            //    leagueModel.LeagueCountry = tempList[p].LeagueCountry;
            //    leagueModel.ColorName = tempList[p].ColorName;
            //    leagueModel.BodyTax = tempList[p].BodyTax;
            //    leagueModel.ColorID = tempList[p].ColorID;

            //    model.LstLeague.Add(leagueModel);
            //    Session["LstLeague"] = model.LstLeague;
            //}
        }

        [HttpGet]
        public JsonResult SearchAction(string searchValue, int currentPage)
        {
            getBankList(currentPage, searchValue);
            var myResult = new
            {
                LstBank = model.LstBank
                //TotalPageNum = model.TotalPageNum
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAction(string name, string shortName)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcInsertBank, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@ShortName", shortName);
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("");
        }

        [HttpPost]
        public JsonResult EditAction(int bankId, string name, string shortName)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcUpdateBank, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BankID", bankId);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@ShortName", shortName);

            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("");
        }

        [HttpPost]
        public JsonResult DeleteAction(int bankId)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcDeleteBank, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BankID", bankId);

            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
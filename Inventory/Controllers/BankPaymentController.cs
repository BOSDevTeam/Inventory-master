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
    public class BankPaymentController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        BankPaymentModels model = new BankPaymentModels();
        Procedure procedure = new Procedure();

        [HttpGet]
        public ActionResult BankPaymentEntry(int bankPaymentId)
        {           
            getBank();
            if (bankPaymentId != 0 && Session["LstBankPayment"] != null)  // edit
            {
                ViewBag.BankPaymentID = bankPaymentId;
                List<BankPaymentModels> LstBankPayment = Session["LstBankPayment"] as List<BankPaymentModels>;
                var editBank = LstBankPayment.Where(c => c.BankPaymentID == bankPaymentId);
                foreach (var e in editBank)
                {
                    ViewBag.Name = e.Name;
                    ViewBag.BankID = e.BankID;
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
        public ActionResult BankPaymentList(int p)
        {            
            getBankwithDefault();
            getBankPaymentList(p, "", 0);
            return View(model);
        }

        private void getBankPaymentList(int currentPage, string searchValue, int bankId)
        {
            List<BankPaymentModels> tempList = new List<BankPaymentModels>();
            BankPaymentModels bankPaymentModel = new BankPaymentModels();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcSearchBankPayment, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", searchValue);
            cmd.Parameters.AddWithValue("@BankID", bankId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                bankPaymentModel = new BankPaymentModels();
                bankPaymentModel.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
                bankPaymentModel.Name = Convert.ToString(reader["BankPaymentName"]);
                bankPaymentModel.BankName = Convert.ToString(reader["BankName"]);
                bankPaymentModel.BankID = Convert.ToInt32(reader["BankID"]);

                model.LstBankPayment.Add(bankPaymentModel);
                Session["LstBankPayment"] = model.LstBankPayment;
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
        public JsonResult SearchAction(string searchValue, int currentPage, int bankId)
        {
            getBankPaymentList(currentPage, searchValue, bankId);
            var myResult = new
            {
                LstBankPayment = model.LstBankPayment
                //TotalPageNum = model.TotalPageNum
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAction(string name, int bankId)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcInsertBankPayment, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@BankID", bankId);
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("");
        }

        [HttpPost]
        public JsonResult EditAction(int bankId, string name, int bankPaymentId)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcUpdateBankPayment, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BankPaymentID", bankPaymentId);
            cmd.Parameters.AddWithValue("@BankID", bankId);
            cmd.Parameters.AddWithValue("@Name", name);


            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("");
        }

        [HttpPost]
        public JsonResult DeleteAction(int bankPaymentId)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcDeleteBankPayment, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BankPaymentID", bankPaymentId);

            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        private void getBank()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select BankID,BankName From S_Bank", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.Banks.Add(new SelectListItem { Text = Convert.ToString(reader["BankName"]), Value = Convert.ToString(reader["BankID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
            if (model.Banks.Count != 0) ViewBag.TopBankID = model.Banks[0].Value;
        }

        private void getBankwithDefault()
        {
            model.Banks.Add(new SelectListItem { Text = "All Bank", Value = "0" });
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select BankID,BankName From S_Bank", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.Banks.Add(new SelectListItem { Text = Convert.ToString(reader["BankName"]), Value = Convert.ToString(reader["BankID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
            if (model.Banks.Count != 0) ViewBag.TopBankID = model.Banks[0].Value;
        }
    }
}
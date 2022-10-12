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
    public class TownshipController : MyController
    {    
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        Procedure procdure = new Procedure();
        InventoryDBEntities Entities = new InventoryDBEntities();
        TownshipModels.TownshipModel model = new TownshipModels.TownshipModel();
        static List<TownshipModels.TownshipModel> lstTownshipList = new List<TownshipModels.TownshipModel>();
        static int editTownshipID;

        public ActionResult TownshipEntry(int townshipId)
        {
            GetDivision();
            if (townshipId != 0)
            {
                Session["IsEdit"] = 1;
                editTownshipID = townshipId;
                var editTownship = lstTownshipList.Where(c => c.TownshipID == townshipId);
                foreach (var e in editTownship)
                {
                    
                    Session["EditDivisionID"] = e.DivisionID;
                    Session["EditTownshipName"] = e.TownshipName;
                    Session["EditCode"] = e.Code;
                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

        private void GetDivision()
        {
            foreach (var division in Entities.S_Division.OrderBy(m=>m.Code))
            {
                model.Division.Add(new SelectListItem { Text = division.DivisionName, Value = division.DivisionID.ToString() });
            }
        }

        
        public ActionResult TownshipList()
        {
            GetDivision();
            TownshipModels.TownshipModel townshipModel = new TownshipModels.TownshipModel();
            model.LstTownship = new List<TownshipModels.TownshipModel>();
            lstTownshipList = new List<TownshipModels.TownshipModel>();

            foreach (var town in Entities.PrcGetTownship())
            {
                townshipModel = new TownshipModels.TownshipModel();
                townshipModel.TownshipID = town.TownshipID;
                townshipModel.TownshipName = town.TownshipName;
                townshipModel.Code = town.Code;
                townshipModel.DivisionID = Convert.ToInt32(town.DivisionID);
                townshipModel.DivisionName = town.DivisionName;
                townshipModel.IsDefault = town.IsDefault;
                model.LstTownship.Add(townshipModel);
                lstTownshipList.Add(townshipModel);
            }

            return View(model);
        }

    

        [HttpGet]
        public JsonResult SaveAction(string townshipName, string code,int divisionId)
        {
            string message;
            int saveOk;
            var towns = (from town in Entities.S_Township where town.Code == code select town).ToList();
            if (towns.Count() == 0)
            {
                Entities.PrcInsertTownship(townshipName, code,divisionId);
                message = "Saved Successfully!";
                saveOk = 1;
            }
            else
            {
                message = "Code Duplicate!";
                saveOk = 0;
            }

            var Result = new
            {
                MESSAGE = message,
                SAVEOK = saveOk
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string townshipName, string code, int divisionId)
        {
            string message;
            int editOk;
            var towns = (from town in Entities.S_Township where town.Code == code where town.TownshipID != editTownshipID select town).ToList();
            if (towns.Count() == 0)
            {
                Entities.PrcUpdateTownship(editTownshipID, townshipName, code, divisionId);
                message = "Edited Successfully!";
                editOk = 1;
            }
            else
            {
                message = "Code Duplicate!";
                editOk = 0;
            }

            var Result = new
            {
                MESSAGE = message,
                EDITOK = editOk
            };

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(string keyword, int? divisionId)
        {

            TownshipModels.TownshipModel townshipModel = new TownshipModels.TownshipModel();
            model.LstTownship = new List<TownshipModels.TownshipModel>();
            lstTownshipList = new List<TownshipModels.TownshipModel>();

            foreach (var town in Entities.PrcSearchTownship(keyword, divisionId))
            {
                townshipModel = new TownshipModels.TownshipModel();  
                townshipModel.TownshipID = town.TownshipID;
                townshipModel.DivisionID = town.DivisionID;
                townshipModel.DivisionName = town.DivisionName;
                townshipModel.TownshipName = town.TownshipName;
                townshipModel.Code = town.Code;
                townshipModel.IsDefault = town.IsDefault;

                model.LstTownship.Add(townshipModel);
                lstTownshipList.Add(townshipModel);
            }

            return Json(model.LstTownship, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int townshipId)
        {
            string message = "";
            bool IsSuccess = false;
            List<TownshipModels> townList = new List<TownshipModels>();
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcDeleteTownship, (SqlConnection) Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TownshipID", townshipId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                message = Convert.ToString(reader["Message"]);
                IsSuccess = Convert.ToBoolean(reader["IsSuccess"]);
            }
            reader.Close();
            dataConnectorSQL.Close();
            var result = new
            {
                Message = message,
                IsSuccess = IsSuccess

            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
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
    
    public class DivisionController : Controller
    {
        InventoryDBEntities Entites = new InventoryDBEntities();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        DivisionModels model = new DivisionModels();
        Procedure procedure = new Procedure();
        static int editDivisionID;
        public ActionResult DivisionEntry(int divisionId)
        {
            if (divisionId != 0)  // edit
            {
                ViewBag.DivisionID = divisionId;
                editDivisionID = divisionId;
                List<DivisionModels> LstDivision = Session["LstDivision"] as List<DivisionModels>;
                var editDivision = LstDivision.Where(c => c.DivisionID == divisionId);
                foreach (var e in editDivision)
                {
                    ViewBag.DivisionName = e.DivisionName;
                    ViewBag.Code = e.Code;
                    ViewBag.IsEdit = 1;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

       [HttpPost]
        public ActionResult EditDivision(string DivisionID, string Code,string DivisionName)
        {
            string message;
            int editOk;
            var editdivision = (from div in Entites.S_Division where div.Code == Code where div.DivisionID != editDivisionID select div).ToList();
            if (editdivision.Count == 0)
            {
                if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateDivision, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DivisionID", DivisionID);
                cmd.Parameters.AddWithValue("@Code", Code);
                cmd.Parameters.AddWithValue("@DivisionName", DivisionName);
                cmd.ExecuteNonQuery();
                dataConnectorSQL.Close();
                message = "Edited Successfully!";
                editOk = 1;
            }
            else
            {
                message = "Code Duplicate!";
                editOk = 0;
            }

            var result = new
            {
                MESSAGE = message,
                EDITOK = editOk
            };
            
            return Json(result);
        }

        
        public ActionResult SaveDivision(string Code , string DivisionName)
        {
            string message;
            int saveOk;
            var division = (from div in Entites.S_Division where div.Code == Code select div).ToList();
            if (division.Count() == 0)
            {
                if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand(Procedure.PrcInsertDivision, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Code", Code);
                cmd.Parameters.AddWithValue("DivisionName", DivisionName);
                cmd.ExecuteNonQuery();
                message = "Save Successfully!";
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
        public JsonResult DeleteDivision(int divisionId)
        {
            string message = "";
            bool IsSuccess = false;
            List<DivisionModels> DivList = new List<DivisionModels>();
            DivisionModels divisionmodel = new DivisionModels();
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcDeleteDivision, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DivisionID", divisionId);
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

        [HttpGet]
        public ActionResult DivisionList()
        {
            List<DivisionModels> DivList = new List<DivisionModels>();
            DivisionModels divisionModel = new DivisionModels();
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetDivision, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                divisionModel = new DivisionModels();
                divisionModel.DivisionID = Convert.ToInt32(rd["DivisionID"]);
                divisionModel.Code = Convert.ToString(rd["Code"]);
                divisionModel.DivisionName = Convert.ToString(rd["DivisionName"]);
                model.LstDivision.Add(divisionModel);
                Session["LstDivision"] = model.LstDivision;
            }
            rd.Close();
            dataConnectorSQL.Close();

            return View(model);
        }

        [HttpGet]
       public JsonResult SearchDivision (string Keyword)
        {
            List<DivisionModels> DivList = new List<DivisionModels>();
            DivisionModels divisionModel = new DivisionModels();
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcSearchDivision, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Keyword", Keyword);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                divisionModel = new DivisionModels();
                divisionModel.DivisionID = Convert.ToInt32(rd["DivisionID"]);
                divisionModel.Code = Convert.ToString(rd["Code"]);
                divisionModel.DivisionName = Convert.ToString(rd["DivisionName"]);
                model.LstDivision.Add(divisionModel);
                Session["LstDivision"] = model.LstDivision;
            }
            rd.Close();
            dataConnectorSQL.Close();
            return Json(model.LstDivision, JsonRequestBehavior.AllowGet);          
        }
       
    }
}
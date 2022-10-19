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
    public class ClientController : Controller
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        ClientModels model = new ClientModels();
        Procedure procedure = new Procedure();
        public ActionResult ClientList()
        {
            GetDivisionDefaultInclude();
            GetTownshipDefaultInclude();
            List<ClientModels> CList = new List<ClientModels>();
            ClientModels clientModel = new ClientModels();
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetClient, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                clientModel = new ClientModels();
                clientModel.ClientID = Convert.ToInt32(rd["ClientID"]);
                clientModel.ClientName = Convert.ToString(rd["ClientName"]);
                clientModel.ShopName = Convert.ToString(rd["ShopName"]);
                clientModel.Phone = Convert.ToString(rd["Phone"]);
                clientModel.DivisionID = Convert.ToInt32(rd["DivisionID"]);
                clientModel.DivisionName = Convert.ToString(rd["DivisionName"]);
                clientModel.TownshipID = Convert.ToInt32(rd["TownshipID"]);
                clientModel.TownshipName = Convert.ToString(rd["TownshipName"]);
                clientModel.Address = Convert.ToString(rd["Address"]);
                clientModel.IsSalePerson = Convert.ToBoolean(rd["IsSalePerson"]);
                model.LstClient.Add(clientModel);
                Session["LstClient"] = model.LstClient;
            }
            rd.Close();
            dataConnectorSQL.Close();

            return View(model);
        }

        //[HttpGet]
        //public JsonResult SearchClient(string Keyword, int? divisionId, int? townshipId, int? clienttype)
        //{
        //    List<ClientModels> CList = new List<ClientModels>();
        //    ClientModels clientModel = new ClientModels();

        //    foreach (var client in Entities.PrcSearchClient(Keyword, divisionId, townshipId, clienttype))
        //    {
        //        clientModel = new ClientModels();
        //        clientModel.ClientID = client.ClientID;
        //        clientModel.ClientName = client.ClientName;
        //        clientModel.ShopName = client.ShopName;
        //        clientModel.Phone = client.Phone;
        //        clientModel.TownshipID = client.TownshipID;
        //        clientModel.TownshipName = client.TownshipName;
        //        clientModel.DivisionID = client.DivisionID;
        //        clientModel.DivisionName = client.DivisionName;
        //        clientModel.Address = client.Address;
        //        clientModel.IsSalePerson = client.IsSalePerson;
        //        model.LstClient.Add(clientModel);
        //        CList.Add(clientModel);
        //    }

        //    return Json(model.LstClient, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult SearchClient(string Keyword, int? divisionId, int? townshipId, int? clienttype)
        {
            List<ClientModels> CList = new List<ClientModels>();
            ClientModels clientModel = new ClientModels();
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcSearchClient, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Keyword", Keyword);
            cmd.Parameters.AddWithValue("@DivisionID", divisionId);
            cmd.Parameters.AddWithValue("@TownshipID", townshipId);
            cmd.Parameters.AddWithValue("@ClientType", clienttype);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                clientModel = new ClientModels();
                clientModel.ClientID = Convert.ToInt32(rd["ClientID"]);
                clientModel.ClientName = Convert.ToString(rd["ClientName"]);
                clientModel.ShopName = Convert.ToString(rd["ShopName"]);
                clientModel.Phone = Convert.ToString(rd["Phone"]);
                clientModel.DivisionID = Convert.ToInt32(rd["DivisionID"]);
                clientModel.DivisionName = Convert.ToString(rd["DivisionName"]);
                clientModel.TownshipID = Convert.ToInt32(rd["TownshipID"]);
                clientModel.TownshipName = Convert.ToString(rd["TownshipName"]);
                clientModel.Address = Convert.ToString(rd["Address"]);
                clientModel.IsSalePerson = Convert.ToBoolean(rd["IsSalePerson"]);
                model.LstClient.Add(clientModel);
                Session["LstClient"] = model.LstClient;
            }
            rd.Close();
            dataConnectorSQL.Close();
            return Json(model.LstClient, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDivsionSelectTownship(int? divisionId)
        {
            TownshipModels.TownshipModel town = new TownshipModels.TownshipModel();
            List<TownshipModels.TownshipModel> lstwon = new List<TownshipModels.TownshipModel>();
            foreach (var township in Entities.PrcGetTownshipByDivision(divisionId))
            {
                town = new TownshipModels.TownshipModel();
                town.TownshipID = township.TownshipID;
                town.TownshipName = township.TownshipName;
                lstwon.Add(town);
            }

            if (lstwon.Count == 0)
            {
                Session["TownshipID"] = 0;
            }

            return Json(lstwon, JsonRequestBehavior.AllowGet);
        }

        private void GetDivisionDefaultInclude()
        {
            model.Divisions.Add(new SelectListItem { Text = "Division", Value = "0" });
            foreach (var div in Entities.S_Division.OrderBy(m => m.Code))
            {
                model.Divisions.Add(new SelectListItem { Text = div.DivisionName, Value = div.DivisionID.ToString() });
            }
        }

        private void GetTownshipDefaultInclude()
        {
            model.Townships.Add(new SelectListItem { Text = "Township", Value = "0" });
            foreach (var town in Entities.S_Township.OrderBy(m => m.Code))
            {
                model.Townships.Add(new SelectListItem { Text = town.TownshipName, Value = town.TownshipID.ToString() });
            }
        }
    }
}
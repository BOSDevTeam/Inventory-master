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
            ViewData["LstClient"] = model.LstClient;

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

        public JsonResult ViewAction(int clientId)
        {
            string clientName = "", shopName = "", phone = "", address = "";
            int divisionId = 0, townshipId = 0,clientID = 0;
            bool salePerson = true;
            List<ClientModels> list = new List<ClientModels>();
            ClientModels model = new ClientModels();
            List<ClientModels> lstClientList = Session["LstClient"] as List<ClientModels>;
            var viewClient = lstClientList.Where(c =>c.ClientID == clientId);
            foreach (var e in viewClient)
            {
                clientID = e.ClientID;
                clientName = e.ClientName;
                shopName = e.ShopName;
                phone = e.Phone;
                divisionId = e.DivisionID;
                townshipId = e.TownshipID;
                foreach (var town in Entities.STownships.Where(t =>t.DivisionID == divisionId))
                {
                    model = new ClientModels();
                    model.TownshipID = town.TownshipID;
                    model.TownshipName = town.TownshipName;
                    list.Add(model);
                }
                address = e.Address;
                salePerson = Convert.ToBoolean(e.IsSalePerson);

            }

            var jsonResult = new
            {
                TownshipList = list,
                ClientID = clientID,
                ClientName = clientName,
                ShopName = shopName,
                Phone = phone,
                Division = divisionId,
                Township = townshipId,
                Address = address,
                IsSalePerson = salePerson
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAction(int clientId,string clientName, string shopName, string phone,int divisionId, int townshipId, string address)
        {
            ClientModels clientModel = new ClientModels();
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateClient, (SqlConnection) Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ClientID", clientId);
            cmd.Parameters.AddWithValue("@ClientName", clientName);
            cmd.Parameters.AddWithValue("@ShopName", shopName);
            cmd.Parameters.AddWithValue("@Phone", phone);
            cmd.Parameters.AddWithValue("@DivisionID", divisionId);
            cmd.Parameters.AddWithValue("@TownshipID", townshipId);
            cmd.Parameters.AddWithValue("@Address", address);
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand comd = new SqlCommand(Procedure.PrcGetClient, (SqlConnection)Session["SQLConnection"]);
            comd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = comd.ExecuteReader();
            while (reader.Read())
            {
                clientModel = new ClientModels();
                clientModel.ClientID = Convert.ToInt32(reader["ClientID"]);
                clientModel.ClientName = Convert.ToString(reader["ClientName"]);
                clientModel.ShopName = Convert.ToString(reader["ShopName"]);
                clientModel.Phone = Convert.ToString(reader["Phone"]);
                clientModel.DivisionID = Convert.ToInt32(reader["DivisionID"]);
                clientModel.DivisionName = Convert.ToString(reader["DivisionName"]);
                clientModel.TownshipID = Convert.ToInt32(reader["TownshipID"]);
                clientModel.TownshipName = Convert.ToString(reader["TownshipName"]);
                clientModel.Address = Convert.ToString(reader["Address"]);
                clientModel.IsSalePerson = Convert.ToBoolean(reader["IsSalePerson"]);
                model.LstClient.Add(clientModel);
                Session["LstClient"] = model.LstClient;

            }
            reader.Close();
            dataConnectorSQL.Close();

            List<ClientModels> list = Session["LstClient"] as List<ClientModels>;
            var result = list.Where(c => c.ClientID == clientId).SingleOrDefault();
            if (Session["LstClient"] != null)
            {
                int index = list.FindIndex(c => c.ClientID == clientId);
                ClientModels newItem = new ClientModels();
                newItem.ClientID = clientId;
                newItem.ClientName = clientName;
                newItem.ShopName = shopName;
                newItem.Phone = phone;
                newItem.DivisionID = divisionId;
                newItem.DivisionName = result.DivisionName;         
                newItem.TownshipID = townshipId;
                newItem.TownshipName = result.TownshipName;
                list[index] = newItem;
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAction(int clientId)
        {
            bool isSuccess = false;
            string message = "";
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcDeleteClient, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ClientID", clientId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isSuccess = Convert.ToBoolean(reader["IsSuccess"]);
                message = Convert.ToString(reader["Message"]);
            }
            var jsonResult = new
            {
                IsSuccess = isSuccess,
                Message = message
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        private void GetDivisionDefaultInclude()
        {
            model.Divisions.Add(new SelectListItem { Text = "Division", Value = "0" });
            foreach (var div in Entities.SDivisions.OrderBy(m => m.Code))
            {
                model.Divisions.Add(new SelectListItem { Text = div.DivisionName, Value = div.DivisionID.ToString() });
            }
        }

        private void GetTownshipDefaultInclude()
        {
            model.Townships.Add(new SelectListItem { Text = "Township", Value = "0" });
            foreach (var town in Entities.STownships.OrderBy(m => m.Code))
            {
                model.Townships.Add(new SelectListItem { Text = town.TownshipName, Value =town.TownshipID.ToString() });
            }
        }

        private void GetTownshipByDivision(int? editdivisionId)
        {
            foreach (var town in Entities.STownships.Where(m => m.DivisionID == editdivisionId).OrderBy(m => m.Code))
            {
                model.Townships.Add(new SelectListItem { Text = town.TownshipName, Value = town.TownshipID.ToString() });
            }
        }
    }
}
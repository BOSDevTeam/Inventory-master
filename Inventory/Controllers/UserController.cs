using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using System.Data.Entity.Core.Objects;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class UserController : MyController
    {
        UserModels.UserModel model = new UserModels.UserModel();
        static List<UserModels.UserModel> lstUserList = new List<UserModels.UserModel>();    
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        Procedure procedure = new Procedure();

        public ActionResult ChangeLanguage(string lang)
        {
            new LanguageMang().SetLanguage(lang);
            return RedirectToAction("Login", "User");
        }

        public ActionResult Login()
        {
            getAllUser();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(int userId, string userPassword, string userName,bool clickedLogin)
        {
            if (clickedLogin)
            {
                int result = -1;
                if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand(Procedure.PrcValidateUser, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@UserPassword", userPassword);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    result = Convert.ToInt16(reader[0]);
                    switch (result)
                    {
                        case 1:
                            Session["LoginUserID"] = userId;
                            Session["LoginUserName"] = userName;                        
                            Session["IsMultiCurrency"] = Convert.ToBoolean(reader["IsMultiCurrency"]);
                            Session["IsMultiUnit"] = Convert.ToBoolean(reader["IsMultiUnit"]);                          
                            Session["IsBankPayment"] = Convert.ToBoolean(reader["IsBankPayment"]);
                            Session["Tax"] = Convert.ToInt32(reader["Tax"]);
                            Session["ServiceCharges"] = Convert.ToInt32(reader["ServiceCharges"]);
                            reader.Close();
                            dataConnectorSQL.Close();
                            return RedirectToAction("Dashboard", "Home");

                        case -1:
                            ViewBag.Message = "Password is incorrect.";
                            break;

                        default: break;
                    }
                }
                reader.Close();
                dataConnectorSQL.Close();
            }
            getAllUser();

            return View(model);
        }

        public ActionResult UserEntry(int userId)
        {                  
            if (userId != 0)
            {
                Session["IsEdit"] = 1;             
                Session["EditUserID"] = userId;
                var editUser = lstUserList.Where(c => c.UserID == userId);
                foreach (var e in editUser)
                {
                    Session["EditUserName"] = e.UserName;
                    Session["EditPassword"] = e.UserPassword;                   
                    Session["EditLocationID"] = e.LocationID;
                    if (e.IsDefaultLocation) Session["EditIsDefaultLocationVal"] = 1;
                    else if (!e.IsDefaultLocation) Session["EditIsDefaultLocationVal"] = 0;

                    getAllLocation();               

                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;              
                getAllLocation();               
            }
            return View(model);
        }

        public ActionResult UserList()
        {                    
            UserModels.UserModel userModel = new UserModels.UserModel();
            model.LstUser = new List<UserModels.UserModel>();
            lstUserList = new List<UserModels.UserModel>();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
          
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                userModel = new UserModels.UserModel();
                userModel.UserID = Convert.ToInt32(reader["UserID"]);
                userModel.UserName = Convert.ToString(reader["UserName"]);
                userModel.UserPassword = Convert.ToString(reader["UserPassword"]);              
                userModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                userModel.LocationName = Convert.ToString(reader["LocationName"]);
                userModel.IsDefaultLocation = Convert.ToBoolean(reader["IsDefaultLocation"]);

                model.LstUser.Add(userModel);
                lstUserList.Add(userModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            return View(model);
        }       

        [HttpGet]
        public JsonResult SaveAction(string userName, string password, bool isDefaultLocation, int? locationId)
        {         
            if (!isDefaultLocation) locationId = 0;
            
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcInsertUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", userName);
            cmd.Parameters.AddWithValue("@UserPassword", password);           
            cmd.Parameters.AddWithValue("@IsDefaultLocation", isDefaultLocation);
            cmd.Parameters.AddWithValue("@LocationID", locationId);
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            var Result = new
            {
                Message = "Saved Successfully!"
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string userName, string password, bool isDefaultLocation, int? locationId, int userId)
        {                   
            if (!isDefaultLocation) locationId = 0;
            
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@UserName", userName);
            cmd.Parameters.AddWithValue("@UserPassword", password);          
            cmd.Parameters.AddWithValue("@IsDefaultLocation", isDefaultLocation);
            cmd.Parameters.AddWithValue("@LocationID", locationId);
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(string keyword)
        {
            UserModels.UserModel userModel = new UserModels.UserModel();
            model.LstUser = new List<UserModels.UserModel>();
            lstUserList = new List<UserModels.UserModel>();            

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcSearchUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", keyword);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                userModel = new UserModels.UserModel();
                userModel.UserID = Convert.ToInt32(reader["UserID"]);
                userModel.UserName = Convert.ToString(reader["UserName"]);
                userModel.UserPassword = Convert.ToString(reader["UserPassword"]);              
                userModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                userModel.LocationName = Convert.ToString(reader["LocationName"]);
                userModel.IsDefaultLocation = Convert.ToBoolean(reader["IsDefaultLocation"]);

                model.LstUser.Add(userModel);
                lstUserList.Add(userModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            return Json(model.LstUser, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int userId)
        {         
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Delete From S_User Where UserID="+userId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            return Json("", JsonRequestBehavior.AllowGet);
        }       

        private void getAllLocation()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select LocationID,LocationName From S_Location Order By Code", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) model.Locations.Add(new SelectListItem { Text = Convert.ToString(reader["LocationName"]), Value = Convert.ToString(reader["LocationID"]) });
            reader.Close();
            dataConnectorSQL.Close();          
        }     
      
        private void getAllUser()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select UserID,UserName From S_User", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) model.Users.Add(new SelectListItem { Text = Convert.ToString(reader["UserName"]), Value = Convert.ToString(reader["UserID"]) });
            reader.Close();
            dataConnectorSQL.Close();         
        }                 
    }
}
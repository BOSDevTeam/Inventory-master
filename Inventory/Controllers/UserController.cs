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
            if (checkIsMultiBranch())
            {
                ViewBag.IsMultiBranch = true;
                getAllBranch();
                getUser(true);
            }
            else
            {
                ViewBag.IsMultiBranch = false;
                getAllUser();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(int? branchId, int userId, string userPassword, string userName, string branchName, bool clickedLogin)
        {
            if (clickedLogin)
            {
                int result = -1;
                if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand(procedure.PrcValidateUser, (SqlConnection)Session["SQLConnection"]);
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
                            Session["LoginBranchID"] = branchId;
                            Session["LoginBranchName"] = branchName;
                            Session["IsMultiCurrency"] = Convert.ToBoolean(reader["IsMultiCurrency"]);
                            Session["IsMultiUnit"] = Convert.ToBoolean(reader["IsMultiUnit"]);
                            Session["IsProductPhoto"] = Convert.ToBoolean(reader["IsProductPhoto"]);
                            Session["IsBankPayment"] = Convert.ToBoolean(reader["IsBankPayment"]);
                            Session["IsDifProductByBranch"] = Convert.ToBoolean(reader["IsDifProductByBranch"]);
                            Session["IsBarcode"] = Convert.ToBoolean(reader["IsBarcode"]);
                            Session["IsQRcode"] = Convert.ToBoolean(reader["IsQRcode"]);
                            Session["IsProductVariant"] = Convert.ToBoolean(reader["IsProductVariant"]);
                            Session["IsProductColor"] = Convert.ToBoolean(reader["IsProductColor"]);
                            Session["IsProductSize"] = Convert.ToBoolean(reader["IsProductSize"]);
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

            if (checkIsMultiBranch())
            {
                ViewBag.IsMultiBranch = true;
                getAllBranch();
                getUserByBranch(branchId);
            }
            else
            {
                ViewBag.IsMultiBranch = false;
                getAllUser();
            }

            return View(model);
        }

        public ActionResult UserEntry(int userId,bool isMultiBranch)
        {        
            ViewBag.IsMultiBranch = isMultiBranch;

            if (userId != 0)
            {
                Session["IsEdit"] = 1;             
                Session["EditUserID"] = userId;
                var editUser = lstUserList.Where(c => c.UserID == userId);
                foreach (var e in editUser)
                {
                    Session["EditUserName"] = e.UserName;
                    Session["EditPassword"] = e.UserPassword;
                    Session["EditBranchID"] = e.BranchID;
                    Session["EditLocationID"] = e.LocationID;
                    if (e.IsDefaultLocation) Session["EditIsDefaultLocationVal"] = 1;
                    else if (!e.IsDefaultLocation) Session["EditIsDefaultLocationVal"] = 0;

                    if (e.BranchID != 0) getLocationByBranch(e.BranchID);
                    else getAllLocation();

                    if (isMultiBranch) getAllBranch();

                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
                if (isMultiBranch)
                {
                    getAllBranch();
                    getLocation(isMultiBranch);
                }
                else
                {
                    getAllLocation();
                }
            }
            return View(model);
        }

        public ActionResult UserList()
        {           
            ViewBag.IsMultiBranch = checkIsMultiBranch();
            UserModels.UserModel userModel = new UserModels.UserModel();
            model.LstUser = new List<UserModels.UserModel>();
            lstUserList = new List<UserModels.UserModel>();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcGetUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
          
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                userModel = new UserModels.UserModel();
                userModel.UserID = Convert.ToInt32(reader["UserID"]);
                userModel.UserName = Convert.ToString(reader["UserName"]);
                userModel.UserPassword = Convert.ToString(reader["UserPassword"]);
                userModel.BranchID = Convert.ToInt32(reader["BranchID"]);
                userModel.BranchName = Convert.ToString(reader["BranchName"]);
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
        public JsonResult BranchSelectAction(int branchId)
        {
            LocationModels.LocationModel locationModel = new LocationModels.LocationModel();
            List<LocationModels.LocationModel> lstLocation = new List<LocationModels.LocationModel>();
           
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select LocationID,LocationName From S_Location Where BranchID=" + branchId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.Branches.Add(new SelectListItem { Text = Convert.ToString(reader["LocationName"]), Value = Convert.ToString(reader["LocationID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();

            return Json(lstLocation, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaveAction(string userName, string password, int? branchId, bool isDefaultLocation, int? locationId, bool isMultiBranch)
        {
            if (!isMultiBranch) branchId = 0;
            if (!isDefaultLocation) locationId = 0;
            
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcInsertUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", userName);
            cmd.Parameters.AddWithValue("@UserPassword", password);
            cmd.Parameters.AddWithValue("@BranchID", branchId);
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
        public JsonResult EditAction(string userName, string password, int? branchId, bool isDefaultLocation, int? locationId, bool isMultiBranch, int userId)
        {         
            if (!isMultiBranch) branchId = 0;
            if (!isDefaultLocation) locationId = 0;
            
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcUpdateUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@UserName", userName);
            cmd.Parameters.AddWithValue("@UserPassword", password);
            cmd.Parameters.AddWithValue("@BranchID", branchId);
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
            SqlCommand cmd = new SqlCommand(procedure.PrcSearchUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", keyword);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                userModel = new UserModels.UserModel();
                userModel.UserID = Convert.ToInt32(reader["UserID"]);
                userModel.UserName = Convert.ToString(reader["UserName"]);
                userModel.UserPassword = Convert.ToString(reader["UserPassword"]);
                userModel.BranchID = Convert.ToInt32(reader["BranchID"]);
                userModel.BranchName = Convert.ToString(reader["BranchName"]);
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

        private void getAllBranch()
        {           
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select BranchID,BranchName From S_Branch Order By Code", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.Branches.Add(new SelectListItem { Text = Convert.ToString(reader["BranchName"]), Value = Convert.ToString(reader["BranchID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void getLocation(bool isMultiBranch)
        {
            if (isMultiBranch)
            {
                int firstBranchId = getFirstBranchID();
                getLocationByBranch(firstBranchId);                             
            }
            else getAllLocation();                        
        }

        private void getLocationByBranch(int? branchId)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select LocationID,LocationName From S_Location Where BranchID=" + branchId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) model.Locations.Add(new SelectListItem { Text = Convert.ToString(reader["LocationName"]), Value = Convert.ToString(reader["LocationID"]) });
            reader.Close();
            dataConnectorSQL.Close();           
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

        private void getUser(bool isMultiBranch)
        {
            if (isMultiBranch)
            {
                int firstBranchId = getFirstBranchID();
                getUserByBranch(firstBranchId);               
            }
            else getAllUser();                        
        }

        private void getUserByBranch(int? branchId)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select UserID,UserName From S_User Where BranchID=" + branchId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) model.Users.Add(new SelectListItem { Text = Convert.ToString(reader["UserName"]), Value = Convert.ToString(reader["UserID"]) });
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
     
        private bool checkIsMultiBranch()
        {
            bool result = false;
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select IsMultiBranch From S_CompanySetting", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                result = Convert.ToBoolean(reader["IsMultiBranch"]);
            }
            reader.Close();
            dataConnectorSQL.Close();
            return result;
        }

        private int getFirstBranchID()
        {
            int firstBranchId = 0;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select Top(1) BranchID From S_Branch ORDER BY Code", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) firstBranchId = Convert.ToInt32(reader["BranchID"]);
            reader.Close();
            dataConnectorSQL.Close();

            return firstBranchId;
        }

        //[HttpPost]
        //public ActionResult AdminLogin(string adminName, string adminPassword)
        //{
        //    int? result = Convert.ToInt32(Entities.PrcValidateAdmin(adminName, adminPassword).FirstOrDefault());

        //    switch (result.Value)
        //    {
        //        case 1:
        //            Session["LoginUserName"] = adminName;
        //            return RedirectToAction("Dashboard", "Home");
        //        case -1:
        //            ViewBag.Message = "Invalid Admin.";
        //            break;

        //        default: break;
        //    }

        //    return View();
        //}
    }
}
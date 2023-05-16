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
        AppSetting setting = new AppSetting();

        public ActionResult ChangeLanguage(string lang)
        {
            new LanguageMang().SetLanguage(lang);
            return RedirectToAction("Login", "User");
        }

        public ActionResult Login()
        {
            Session["LstUser"] = getLoginUser();
            return View(model);
        }

        [HttpGet]
        public JsonResult LoginAction(int userId, string userName, string userPassword)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            short result = 0, isTechnician = 0, saleVoucherDesignType = 0;
            bool isMultiCurrency = false, isMultiUnit = false, isBankPayment = false;
            int tax = 0, serviceCharges = 0;

            if (Session["LstUser"] != null)
            {
                try
                {
                    List<UserModels.UserModel> lstUser = Session["LstUser"] as List<UserModels.UserModel>;
                    UserModels.UserModel userModel = lstUser.Where(x => x.UserID == userId).Where(x => x.UserName == userName).SingleOrDefault();

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcValidateUser, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@UserPassword", userPassword);
                    cmd.Connection = setting.conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        result = Convert.ToInt16(reader[0]);
                        switch (result)
                        {
                            case 1:
                                Session["LoginUserID"] = userId;
                                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();
                                isTechnician = userModel.IsTechnician;
                                isMultiCurrency = Convert.ToBoolean(reader["IsMultiCurrency"]);
                                isMultiUnit = Convert.ToBoolean(reader["IsMultiUnit"]);
                                isBankPayment = Convert.ToBoolean(reader["IsBankPayment"]);
                                tax = Convert.ToInt32(reader["Tax"]);
                                serviceCharges = Convert.ToInt32(reader["ServiceCharges"]);
                                saleVoucherDesignType = Convert.ToInt16(reader["SaleVoucherDesignType"]);
                                resultDefaultData.IsRequestSuccess = true;
                                break;
                            case 0:
                                resultDefaultData.Message = "Invalid User!";
                                break;
                            default: break;
                        }
                    }
                    reader.Close();
                    setting.conn.Close();

                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                UserID = userId,
                UserName = userName,
                IsTechnician = isTechnician,
                isMultiCurrency = isMultiCurrency,
                isMultiUnit = isMultiUnit,
                isBankPayment = isBankPayment,
                Tax = tax,
                ServiceCharges = serviceCharges,
                SaleVoucherDesignType = saleVoucherDesignType
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult UserEntry(int userId,short? isTechnician)
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
            if (isTechnician == 1)
            {
                model.IsTechnician = 1;
                model.Layout = "~/Views/Shared/_LayoutTechnicianSetting.cshtml";
            }
            else model.Layout = "~/Views/Shared/_LayoutSetting.cshtml";
            return View(model);
        }

        public ActionResult UserList(short? isTechnician)
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
            if (isTechnician == 1)
            {
                model.IsTechnician = 1;
                model.Layout = "~/Views/Shared/_LayoutTechnicianSetting.cshtml";
            }
            else model.Layout = "~/Views/Shared/_LayoutSetting.cshtml";
            return View(model);
        }       

        [HttpGet]
        public JsonResult SaveAction(string userName, string password, bool isDefaultLocation, int? locationId)
        {
            bool isSuccess = false;
            string message = "";

            if (!isDefaultLocation) locationId = 0;
            
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcInsertUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", userName);
            cmd.Parameters.AddWithValue("@UserPassword", password);           
            cmd.Parameters.AddWithValue("@IsDefaultLocation", isDefaultLocation);
            cmd.Parameters.AddWithValue("@LocationID", locationId);
            SqlDataReader reder = cmd.ExecuteReader();
            if (reder.Read())isSuccess = Convert.ToBoolean(reder["IsSuccess"]);           
            reder.Close();
            dataConnectorSQL.Close();

            if (isSuccess) message = Common.AppConstants.Message.SaveSuccess;
            else message = Common.AppConstants.Message.UserLimitOver;

            var Result = new
            {
                Message = message,
                IsSuccess = isSuccess
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
            string message = "";
            bool IsSuccess = false;
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcDeleteUser, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", userId);
            SqlDataReader reder = cmd.ExecuteReader();
            if (reder.Read())
            {
                IsSuccess = Convert.ToBoolean(reder["IsSuccess"]);
                message = Convert.ToString(reder["Message"]);
            }

            reder.Close();
            dataConnectorSQL.Close();

            var result = new
            {
                IsSuccess = IsSuccess,
                Message = message
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }       

        private void getAllLocation()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select LocationID,LocationName From SLocation Order By Code", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) model.Locations.Add(new SelectListItem { Text = Convert.ToString(reader["LocationName"]), Value = Convert.ToString(reader["LocationID"]) });
            reader.Close();
            dataConnectorSQL.Close();          
        }     

        private List<UserModels.UserModel> getLoginUser()
        {
            List<UserModels.UserModel> lstUser = new List<UserModels.UserModel>();
            UserModels.UserModel userModel = new UserModels.UserModel();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(TextQuery.allUserQuery, setting.conn);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                userModel = new UserModels.UserModel();
                userModel.UserID = Convert.ToInt32(reader["UserID"]);
                userModel.UserName = Convert.ToString(reader["UserName"]);
                userModel.IsTechnician = Convert.ToInt16(reader["IsTechnician"]);
                lstUser.Add(userModel);
            }
            reader.Close();         
            setting.conn.Close();
            return lstUser;
        }     
    }
}
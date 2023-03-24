using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;
using System.Data.SqlClient;
using System.Data;

namespace Inventory.Controllers
{
    public class UserRightController : MyController
    {
        AppData appData = new AppData();
        UserRightModels model = new UserRightModels();
        AppSetting setting = new AppSetting();
        TextQuery textQuery = new TextQuery();

        public ActionResult UserRight(short isTechnician,int userId)
        {
            getUser();
            ViewData["SetupModuleAccess"] = getSetupModule(0);
            ViewData["EntryModuleAccess"] = getEntryModule(0);
            if (isTechnician == 1)
            {
                model.IsTechnician = 1;
                model.Layout = "~/Views/Shared/_LayoutTechnicianSetting.cshtml";
            }
            else model.Layout = "~/Views/Shared/_LayoutSetting.cshtml";
            return View(model);
        }

        [HttpGet]
        public JsonResult UserClickAction(int userId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SetupModuleModels> lstSetupModuleAccess = new List<SetupModuleModels>();
            List<EntryModuleModels> lstEntryModuleAccess = new List<EntryModuleModels>();

            try
            {
                lstSetupModuleAccess = getSetupModule(userId);
                lstEntryModuleAccess = getEntryModule(userId);
                Session["LstSetupModuleAccess"] = lstSetupModuleAccess;
                Session["LstEntryModuleAccess"] = lstEntryModuleAccess;
                resultDefaultData.IsRequestSuccess = true;
            }
            catch(Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }
            
            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                LstSetupModuleAccess = lstSetupModuleAccess,
                LstEntryModuleAccess = lstEntryModuleAccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AccessAllModuleAction(int userId,bool isChecked,short moduleType)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SetupModuleModels> lstSetupModuleAccess = new List<SetupModuleModels>();
            List<EntryModuleModels> lstEntryModuleAccess = new List<EntryModuleModels>();

            if (moduleType == 1)
            {
                if (Session["LstSetupModuleAccess"] != null)
                {
                    lstSetupModuleAccess = Session["LstSetupModuleAccess"] as List<SetupModuleModels>;
                    for(int i = 0; i < lstSetupModuleAccess.Count(); i++)
                    {
                        lstSetupModuleAccess[i].IsAllow = isChecked;
                    }
                    Session["LstSetupModuleAccess"] = lstSetupModuleAccess;
                    resultDefaultData.IsRequestSuccess = true;
                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();                
            }else if(moduleType == 2)
            {
                if (Session["LstEntryModuleAccess"] != null)
                {
                    lstEntryModuleAccess = Session["LstEntryModuleAccess"] as List<EntryModuleModels>;
                    for (int i = 0; i < lstEntryModuleAccess.Count(); i++)
                    {
                        lstEntryModuleAccess[i].IsAllow = isChecked;
                    }
                    Session["LstEntryModuleAccess"] = lstEntryModuleAccess;
                    resultDefaultData.IsRequestSuccess = true;
                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }
            else if(moduleType == 3)
            {

            }

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                LstSetupModuleAccess = lstSetupModuleAccess,
                LstEntryModuleAccess = lstEntryModuleAccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AccessSetupModuleAction(int userId, bool isChecked, int setupModuleId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SetupModuleModels> lstSetupModuleAccess = new List<SetupModuleModels>();

            if (Session["LstSetupModuleAccess"] != null)
            {
                lstSetupModuleAccess = Session["LstSetupModuleAccess"] as List<SetupModuleModels>;
                SetupModuleModels setupModuleModel = lstSetupModuleAccess.Where(x => x.SetupModuleID == setupModuleId).SingleOrDefault();
                setupModuleModel.IsAllow = isChecked;
                int index = lstSetupModuleAccess.FindIndex(x => x.SetupModuleID == setupModuleId);
                lstSetupModuleAccess[index] = setupModuleModel;

                Session["LstSetupModuleAccess"] = lstSetupModuleAccess;
                resultDefaultData.IsRequestSuccess = true;
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                LstSetupModuleAccess = lstSetupModuleAccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AccessEntryModuleAction(int userId, bool isChecked, int entryModuleId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<EntryModuleModels> lstEntryModuleAccess = new List<EntryModuleModels>();

            if (Session["LstEntryModuleAccess"] != null)
            {
                lstEntryModuleAccess = Session["LstEntryModuleAccess"] as List<EntryModuleModels>;
                EntryModuleModels entryModuleModel = lstEntryModuleAccess.Where(x => x.EntryModuleID == entryModuleId).SingleOrDefault();
                entryModuleModel.IsAllow = isChecked;
                int index = lstEntryModuleAccess.FindIndex(x => x.EntryModuleID == entryModuleId);
                lstEntryModuleAccess[index] = entryModuleModel;

                Session["LstEntryModuleAccess"] = lstEntryModuleAccess;
                resultDefaultData.IsRequestSuccess = true;
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                LstEntryModuleAccess = lstEntryModuleAccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaveAction(int userId, short moduleType)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (moduleType == 1)
            {
                if (Session["LstSetupModuleAccess"] != null)
                {
                    List<SetupModuleModels> lstSetupModuleAccess = Session["LstSetupModuleAccess"] as List<SetupModuleModels>;

                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ModuleID", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsAllow", typeof(bool)));

                    for (int i = 0; i < lstSetupModuleAccess.Count; i++)
                    {
                        dt.Rows.Add(lstSetupModuleAccess[i].SetupModuleID, lstSetupModuleAccess[i].IsAllow);
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateSetupModuleUserRight, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Connection = setting.conn;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();

                    resultDefaultData.IsRequestSuccess = true;
                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }
            else if (moduleType == 2)
            {
                if (Session["LstEntryModuleAccess"] != null)
                {
                    List<EntryModuleModels> lstEntryModuleAccess = Session["LstEntryModuleAccess"] as List<EntryModuleModels>;

                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ModuleID", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsAllow", typeof(bool)));

                    for (int i = 0; i < lstEntryModuleAccess.Count; i++)
                    {
                        dt.Rows.Add(lstEntryModuleAccess[i].EntryModuleID, lstEntryModuleAccess[i].IsAllow);
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateEntryModuleUserRight, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Connection = setting.conn;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();

                    resultDefaultData.IsRequestSuccess = true;
                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }
            else if (moduleType == 3)
            {

            }

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        private List<SetupModuleModels> getSetupModule(int selectedUserId)
        {
            List<SetupModuleModels> list = new List<SetupModuleModels>();
            SetupModuleModels item;

            setting.conn.Open();
            SqlCommand cmd;
            if(selectedUserId == 0)cmd = new SqlCommand(TextQuery.setupModuleQuery, setting.conn);
            else cmd = new SqlCommand(textQuery.getSetupModuleQuery(selectedUserId), setting.conn);
            cmd.CommandType = CommandType.Text;                     
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new SetupModuleModels();
                item.SetupModuleID = Convert.ToInt32(reader["SetupModuleID"]);
                item.SetupModuleName = Convert.ToString(reader["SetupModuleName"]);
                if(selectedUserId != 0)item.IsAllow = Convert.ToBoolean(reader["IsAllow"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }

        private List<EntryModuleModels> getEntryModule(int selectedUserId)
        {
            List<EntryModuleModels> list = new List<EntryModuleModels>();
            EntryModuleModels item;

            setting.conn.Open();
            SqlCommand cmd;
            if (selectedUserId == 0) cmd = new SqlCommand(TextQuery.entryModuleQuery, setting.conn);
            else cmd = new SqlCommand(textQuery.getEntryModuleQuery(selectedUserId), setting.conn);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new EntryModuleModels();
                item.EntryModuleID = Convert.ToInt32(reader["EntryModuleID"]);
                item.EntryModuleName = Convert.ToString(reader["EntryModuleName"]);
                if (selectedUserId != 0) item.IsAllow = Convert.ToBoolean(reader["IsAllow"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }

        private void getUser()
        {
            List<UserModels.UserModel> list = appData.selectUser();
            for (int i = 0; i < list.Count; i++)
            {
                model.Users.Add(new SelectListItem { Text = list[i].UserName, Value = Convert.ToString(list[i].UserID) });
            }
        }
    }
}
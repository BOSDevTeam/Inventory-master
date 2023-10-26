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
            ViewData["SetupModuleAccess"] = getSetupModule(0, isTechnician, userId);
            ViewData["EntryModuleAccess"] = getEntryModule(0, isTechnician, userId);
            ViewData["ReportModuleAccess"] = getReportModule(0,isTechnician,userId);
            ViewData["PermissionAccess"] = getPermission(0, isTechnician, userId);
            if (isTechnician == 1)
            {
                model.IsTechnician = 1;
                model.Layout = "~/Views/Shared/_LayoutTechnicianSetting.cshtml";
            }
            else model.Layout = "~/Views/Shared/_LayoutSetting.cshtml";
            return View(model);
        }

        [HttpGet]
        public JsonResult ResetAction()
        {
            if (Session["LstSetupModuleAccess"] != null)
            {
                List<SetupModuleModels> lstSetupModuleAccess = Session["LstSetupModuleAccess"] as List<SetupModuleModels>;
                for (int i = 0; i < lstSetupModuleAccess.Count(); i++)
                {
                    lstSetupModuleAccess[i].IsAllow = false;
                }
                Session["LstSetupModuleAccess"] = lstSetupModuleAccess;               
            }
            if (Session["LstEntryModuleAccess"] != null)
            {
                List<EntryModuleModels> lstEntryModuleAccess = Session["LstEntryModuleAccess"] as List<EntryModuleModels>;
                for (int i = 0; i < lstEntryModuleAccess.Count(); i++)
                {
                    lstEntryModuleAccess[i].IsAllow = false;
                }
                Session["LstEntryModuleAccess"] = lstEntryModuleAccess;
            }
            if (Session["LstReportModuleAccess"] != null)
            {
                List<ReportModuleModels> lstReportModuleAccess = Session["LstReportModuleAccess"] as List<ReportModuleModels>;
                for (int i = 0; i < lstReportModuleAccess.Count(); i++)
                {
                    lstReportModuleAccess[i].IsAllow = false;
                }
                Session["LstReportModuleAccess"] = lstReportModuleAccess;
            }
            if (Session["LstPermissionAccess"] != null)
            {
                List<PermissionModels> lstPermissionAccess = Session["LstPermissionAccess"] as List<PermissionModels>;
                for (int i = 0; i < lstPermissionAccess.Count(); i++)
                {
                    lstPermissionAccess[i].IsAllow = false;
                }
                Session["LstPermissionAccess"] = lstPermissionAccess;
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UserClickAction(int userId, short isTechnician, int loginUserId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SetupModuleModels> lstSetupModuleAccess = new List<SetupModuleModels>();
            List<EntryModuleModels> lstEntryModuleAccess = new List<EntryModuleModels>();
            List<ReportModuleModels> lstReportModuleAccess = new List<ReportModuleModels>();
            List<PermissionModels> lstPermissionAccess = new List<PermissionModels>();

            try
            {
                lstSetupModuleAccess = getSetupModule(userId, isTechnician, loginUserId);
                lstEntryModuleAccess = getEntryModule(userId, isTechnician, loginUserId);
                lstReportModuleAccess = getReportModule(userId,isTechnician,loginUserId);
                lstPermissionAccess = getPermission(userId, isTechnician, loginUserId);
                Session["LstSetupModuleAccess"] = lstSetupModuleAccess;
                Session["LstEntryModuleAccess"] = lstEntryModuleAccess;
                Session["LstReportModuleAccess"] = lstReportModuleAccess;
                Session["LstPermissionAccess"] = lstPermissionAccess;
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
                LstEntryModuleAccess = lstEntryModuleAccess,
                LstReportModuleAccess = lstReportModuleAccess,
                LstPermissionAccess = lstPermissionAccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AccessAllModuleAction(int userId,bool isChecked,short moduleType)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<SetupModuleModels> lstSetupModuleAccess = new List<SetupModuleModels>();
            List<EntryModuleModels> lstEntryModuleAccess = new List<EntryModuleModels>();
            List<ReportModuleModels> lstReportModuleAccess = new List<ReportModuleModels>();
            List<PermissionModels> lstPermissionAccess = new List<PermissionModels>();

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
                if (Session["LstReportModuleAccess"] != null)
                {
                    lstReportModuleAccess = Session["LstReportModuleAccess"] as List<ReportModuleModels>;
                    for (int i = 0; i < lstReportModuleAccess.Count(); i++)
                    {
                        lstReportModuleAccess[i].IsAllow = isChecked;
                    }
                    Session["LstReportModuleAccess"] = lstReportModuleAccess;
                    resultDefaultData.IsRequestSuccess = true;
                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }
            else if (moduleType == 4)
            {
                if (Session["LstPermissionAccess"] != null)
                {
                    lstPermissionAccess = Session["LstPermissionAccess"] as List<PermissionModels>;
                    for (int i = 0; i < lstPermissionAccess.Count(); i++)
                    {
                        lstPermissionAccess[i].IsAllow = isChecked;
                    }
                    Session["LstPermissionAccess"] = lstPermissionAccess;
                    resultDefaultData.IsRequestSuccess = true;
                }
                else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();
            }

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                LstSetupModuleAccess = lstSetupModuleAccess,
                LstEntryModuleAccess = lstEntryModuleAccess,
                LstReportModuleAccess = lstReportModuleAccess,
                LstPermissionAccess = lstPermissionAccess
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
        public JsonResult AccessReportModuleAction(int userId, bool isChecked, int reportModuleId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<ReportModuleModels> lstReportModuleAccess = new List<ReportModuleModels>();

            if (Session["LstReportModuleAccess"] != null)
            {
                lstReportModuleAccess = Session["LstReportModuleAccess"] as List<ReportModuleModels>;
                ReportModuleModels reportModuleModel = lstReportModuleAccess.Where(x => x.ReportModuleID == reportModuleId).SingleOrDefault();
                reportModuleModel.IsAllow = isChecked;
                int index = lstReportModuleAccess.FindIndex(x => x.ReportModuleID == reportModuleId);
                lstReportModuleAccess[index] = reportModuleModel;

                Session["LstReportModuleAccess"] = lstReportModuleAccess;
                resultDefaultData.IsRequestSuccess = true;
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                LstReportModuleAccess = lstReportModuleAccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AccessPermissionAction(int userId, bool isChecked, int permissionId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<PermissionModels> lstPermissionAccess = new List<PermissionModels>();

            if (Session["LstPermissionAccess"] != null)
            {
                lstPermissionAccess = Session["LstPermissionAccess"] as List<PermissionModels>;
                PermissionModels permissionModel = lstPermissionAccess.Where(x => x.PermissionID == permissionId).SingleOrDefault();
                permissionModel.IsAllow = isChecked;
                int index = lstPermissionAccess.FindIndex(x => x.PermissionID == permissionId);
                lstPermissionAccess[index] = permissionModel;

                Session["LstPermissionAccess"] = lstPermissionAccess;
                resultDefaultData.IsRequestSuccess = true;
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData,
                LstPermissionAccess = lstPermissionAccess
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
                if (Session["LstReportModuleAccess"] != null)
                {
                    List<ReportModuleModels> lstReportModuleAccess = Session["LstReportModuleAccess"] as List<ReportModuleModels>;

                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ModuleID", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsAllow", typeof(bool)));

                    for (int i = 0; i < lstReportModuleAccess.Count; i++)
                    {
                        dt.Rows.Add(lstReportModuleAccess[i].ReportModuleID, lstReportModuleAccess[i].IsAllow);
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateReportModuleUserRight, setting.conn);
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
            else if (moduleType == 4)
            {
                if (Session["LstPermissionAccess"] != null)
                {
                    List<PermissionModels> lstPermissionAccess = Session["LstPermissionAccess"] as List<PermissionModels>;

                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ModuleID", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsAllow", typeof(bool)));

                    for (int i = 0; i < lstPermissionAccess.Count; i++)
                    {
                        dt.Rows.Add(lstPermissionAccess[i].PermissionID, lstPermissionAccess[i].IsAllow);
                    }

                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdatePermissionUserRight, setting.conn);
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

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        private List<SetupModuleModels> getSetupModule(int selectedUserId, short isTechnician, int loginUserId)
        {
            List<SetupModuleModels> list = new List<SetupModuleModels>();
            SetupModuleModels item;

            setting.conn.Open();
            SqlCommand cmd;
            if (isTechnician == 1)
            {
                if (selectedUserId == 0)
                {
                    cmd = new SqlCommand(TextQuery.setupModuleQuery, setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd = new SqlCommand(textQuery.getSetupModuleAccessQuery(selectedUserId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
            }
            else
            {
                if (selectedUserId == 0)
                {
                    cmd = new SqlCommand(textQuery.getAllowSetupModuleQuery(loginUserId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd = new SqlCommand(Procedure.PrcGetAccessAllowedSetupModule, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginUserID", loginUserId);
                    cmd.Parameters.AddWithValue("@SelectedUserID", selectedUserId);
                }
            }
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

        private List<EntryModuleModels> getEntryModule(int selectedUserId, short isTechnician, int loginUserId)
        {
            List<EntryModuleModels> list = new List<EntryModuleModels>();
            EntryModuleModels item;

            setting.conn.Open();
            SqlCommand cmd;
            if (isTechnician == 1)
            {
                if (selectedUserId == 0)
                {
                    cmd = new SqlCommand(TextQuery.entryModuleQuery, setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd = new SqlCommand(textQuery.getEntryModuleAccessQuery(selectedUserId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
            }
            else
            {
                if (selectedUserId == 0)
                {
                    cmd = new SqlCommand(textQuery.getAllowEntryModuleQuery(loginUserId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd = new SqlCommand(Procedure.PrcGetAccessAllowedEntryModule, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginUserID", loginUserId);
                    cmd.Parameters.AddWithValue("@SelectedUserID", selectedUserId);
                }
            }
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

        private List<ReportModuleModels> getReportModule(int selectedUserId,short isTechnician,int loginUserId)
        {
            List<ReportModuleModels> list = new List<ReportModuleModels>();
            ReportModuleModels item;

            setting.conn.Open();
            SqlCommand cmd;
            if(isTechnician == 1)
            {
                if (selectedUserId == 0)
                {
                    cmd = new SqlCommand(TextQuery.reportModuleQuery, setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd = new SqlCommand(textQuery.getReportModuleAccessQuery(selectedUserId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
            }else
            {
                if (selectedUserId == 0)
                {
                    cmd = new SqlCommand(textQuery.getAllowReportModuleQuery(loginUserId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd = new SqlCommand(Procedure.PrcGetAccessAllowedReportModule, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginUserID", loginUserId);
                    cmd.Parameters.AddWithValue("@SelectedUserID", selectedUserId);
                }
            }                       
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new ReportModuleModels();
                item.ReportModuleID = Convert.ToInt32(reader["ReportModuleID"]);
                item.ReportModuleName = Convert.ToString(reader["ReportModuleName"]);
                if (selectedUserId != 0) item.IsAllow = Convert.ToBoolean(reader["IsAllow"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }

        private List<PermissionModels> getPermission(int selectedUserId, short isTechnician, int loginUserId)
        {
            List<PermissionModels> list = new List<PermissionModels>();
            PermissionModels item;

            setting.conn.Open();
            SqlCommand cmd;
            if (isTechnician == 1)
            {
                if (selectedUserId == 0)
                {
                    cmd = new SqlCommand(TextQuery.permissionQuery, setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd = new SqlCommand(textQuery.getPermissionAccessQuery(selectedUserId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
            }
            else
            {
                if (selectedUserId == 0)
                {
                    cmd = new SqlCommand(textQuery.getAllowPermissionQuery(loginUserId), setting.conn);
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd = new SqlCommand(Procedure.PrcGetAccessAllowedPermission, setting.conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginUserID", loginUserId);
                    cmd.Parameters.AddWithValue("@SelectedUserID", selectedUserId);
                }
            }
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new PermissionModels();
                item.PermissionID = Convert.ToInt32(reader["PermissionID"]);
                item.PermissionName = Convert.ToString(reader["PermissionName"]);
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
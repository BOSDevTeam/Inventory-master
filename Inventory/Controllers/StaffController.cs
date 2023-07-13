using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using Inventory.Models;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class StaffController : MyController
    {
        InventoryDBEntities entity = new InventoryDBEntities();
        AppData appData = new AppData();
        StaffModels model = new StaffModels();
        AppSetting setting = new AppSetting();

        [SessionTimeoutAttribute]
        public ActionResult StaffEntry(int? staffId)
        {
            getDivision(false);
            if (staffId == null) // new
            {
                if (model.Divisions.Count() != 0) getTownship(Convert.ToInt32(model.Divisions[0].Value));
            }
            else // edit
            {
                ViewBag.IsEdit = true;
                if (Session["StaffList"] != null)
                {
                    List<StaffModels> list = Session["StaffList"] as List<StaffModels>;
                    StaffModels staffModel = list.Where(c => c.StaffID == staffId).SingleOrDefault();
                    if (staffModel != null)
                    {
                        ViewBag.StaffID = staffModel.StaffID;
                        ViewBag.StaffName = staffModel.StaffName;
                        ViewBag.Code = staffModel.Code;
                        ViewBag.Phone = staffModel.Phone;
                        ViewBag.Address = staffModel.Address;
                        ViewBag.TownshipID = staffModel.TownshipID;
                        ViewBag.DivisionID = staffModel.DivisionID;
                        getTownship(staffModel.DivisionID);
                    }
                }
            }
            return View(model);
        }

        public ActionResult StaffList()
        {
            getDivision(true);
            getTownship(true);
            ViewData["StaffList"] = selectStaff(false);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetTownshipByDivision(int divisionId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<TownshipModels.TownshipModel> lstTownship = new List<TownshipModels.TownshipModel>();
            try
            {
                if (divisionId != 0) lstTownship = appData.selectTownship(divisionId);
                else
                {                                    
                    lstTownship = appData.selectTownship(null);
                    TownshipModels.TownshipModel townShip = new TownshipModels.TownshipModel();
                    townShip.TownshipID = 0;
                    townShip.TownshipName = "Township";
                    lstTownship.Insert(0, townShip);
                }
                resultDefaultData.IsRequestSuccess = true;
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }

            var Result = new
            {
                ResultDefaultData = resultDefaultData,
                LstTownship = lstTownship
            };
            return Json(Result,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAction(string staffName, string code, string phone, string address, int townshipId, int divisionId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            try
            {
                var staffObj = (from staff in entity.SStaffs where staff.Code == code select staff).SingleOrDefault();
                if (staffObj == null)
                {
                    SStaff tableRow = new SStaff();
                    tableRow.StaffName = staffName;
                    tableRow.Code = code;
                    tableRow.Phone = phone;
                    tableRow.DivisionID = divisionId;
                    tableRow.TownshipID = townshipId;
                    tableRow.Address = address;
                    entity.SStaffs.Add(tableRow);
                    entity.SaveChanges();
                    resultDefaultData.IsRequestSuccess = true;
                    resultDefaultData.Message = AppConstants.Message.SaveSuccess;
                }
                else
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                    resultDefaultData.Message = AppConstants.Message.CodeDuplicate;
                }
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }

            var Result = new
            {
                ResultDefaultData = resultDefaultData
            };
            return Json(Result);
        }

        [HttpPost]
        public JsonResult EditAction(string staffName, string code, string phone, string address, int townshipId, int divisionId, int editStaffId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            try
            {
                var staffObj = (from staff in entity.SStaffs where staff.Code == code where staff.StaffID != editStaffId select staff).SingleOrDefault();
                if (staffObj == null)
                {
                    var editStaff = entity.SStaffs.SingleOrDefault(c => c.StaffID == editStaffId);
                    if(editStaff != null)
                    {
                        editStaff.StaffName = staffName;
                        editStaff.Code = code;
                        editStaff.Phone = phone;
                        editStaff.DivisionID = divisionId;
                        editStaff.TownshipID = townshipId;
                        editStaff.Address = address;
                        entity.SaveChanges();
                        resultDefaultData.IsRequestSuccess = true;
                        resultDefaultData.Message = AppConstants.Message.EditSuccess;
                    }                   
                }
                else
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                    resultDefaultData.Message = AppConstants.Message.CodeDuplicate;
                }
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }

            var Result = new
            {
                ResultDefaultData = resultDefaultData
            };
            return Json(Result);
        }

        [HttpGet]
        public JsonResult ViewAction(int staffId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            StaffModels staffModel = new StaffModels();

            if (Session["StaffList"] != null)
            {
                try
                {
                    List<StaffModels> list = Session["StaffList"] as List<StaffModels>;
                    staffModel = list.Where(c => c.StaffID == staffId).SingleOrDefault();
                    if (staffModel != null) resultDefaultData.IsRequestSuccess = true;
                    else
                    {
                        resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                        resultDefaultData.Message = AppConstants.Message.NotFoundData;
                    }
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
                StaffModel = staffModel
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(string keyword, int divisionId, int? townshipId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<StaffModels> lstStaff = new List<StaffModels>();

            try
            {
                lstStaff = selectStaff(true, keyword, divisionId, townshipId);
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
                LstStaff = lstStaff
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int staffId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcDeleteStaff, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StaffID", staffId);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                resultDefaultData.IsRequestSuccess = Convert.ToBoolean(reader["IsSuccess"]);
                if (resultDefaultData.IsRequestSuccess) resultDefaultData.Message = AppConstants.Message.DeleteSuccess;
                else
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                    resultDefaultData.Message = Convert.ToString(reader["Message"]);
                }
            }
            reader.Close();
            setting.conn.Close();

            var result = new
            {
                ResultDefaultData = resultDefaultData
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region methods

        public List<StaffModels> selectStaff(bool isSearch, [Optional]string keyword, [Optional]int divisionId, [Optional]int? townshipId)
        {
            List<StaffModels> list = new List<StaffModels>();
            StaffModels item;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetStaff, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IsSearch", isSearch);
            if (isSearch)
            {
                cmd.Parameters.AddWithValue("@Keyword", keyword);
                cmd.Parameters.AddWithValue("@DivisionID",divisionId);
                cmd.Parameters.AddWithValue("@TownshipID", townshipId);              
            }
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new StaffModels();
                item.StaffID = Convert.ToInt32(reader["StaffID"]);
                item.StaffName = Convert.ToString(reader["StaffName"]);
                item.Code = Convert.ToString(reader["Code"]);
                item.Phone = Convert.ToString(reader["Phone"]);
                item.Address = Convert.ToString(reader["Address"]);
                item.TownshipID = Convert.ToInt32(reader["TownshipID"]);
                item.TownshipName = Convert.ToString(reader["TownshipName"]);
                item.DivisionID = Convert.ToInt32(reader["DivisionID"]);
                item.DivisionName = Convert.ToString(reader["DivisionName"]);
                list.Add(item);
            }
            Session["StaffList"] = list;
            reader.Close();
            setting.conn.Close();
            return list;
        }

        private void getDivision(bool isIncludeDefault)
        {
            if(isIncludeDefault) model.Divisions.Add(new SelectListItem { Text = "Division", Value = "0" });
            List<DivisionModels> list = appData.selectDivision();
            for (int i = 0; i < list.Count; i++)
            {
                model.Divisions.Add(new SelectListItem { Text = list[i].DivisionName, Value = Convert.ToString(list[i].DivisionID) });
            }
        }

        private void getTownship(int divisionId)
        {
            List<TownshipModels.TownshipModel> list = appData.selectTownship(divisionId);
            for (int i = 0; i < list.Count; i++)
            {
                model.Townships.Add(new SelectListItem { Text = list[i].TownshipName, Value = Convert.ToString(list[i].TownshipID) });
            }
        }

        private void getTownship(bool isIncludeDefault)
        {
            if (isIncludeDefault) model.Townships.Add(new SelectListItem { Text = "Township", Value = "0" });
            List<TownshipModels.TownshipModel> list = appData.selectTownship(null);
            for (int i = 0; i < list.Count; i++)
            {
                model.Townships.Add(new SelectListItem { Text = list[i].TownshipName, Value = Convert.ToString(list[i].TownshipID) });
            }
        }

        #endregion
    }
}
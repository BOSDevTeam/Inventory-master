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
    public class LocationController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        LocationModels.LocationModel model = new LocationModels.LocationModel();
        static List<LocationModels.LocationModel> lstLocationList = new List<LocationModels.LocationModel>();
        static int editLocationID;
        Procedure procedure = new Procedure();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();

        public ActionResult LocationEntry(int locationId)
        {
            if (locationId != 0)
            {
                Session["IsEdit"] = 1;
                editLocationID = locationId;
                var editLocation = lstLocationList.Where(c => c.LocationID == locationId);
                foreach (var e in editLocation)
                {
                    Session["EditLocationName"] = e.LocationName;
                    Session["EditShortName"] = e.ShortName;
                    Session["EditDescription"] = e.Description;
                    Session["EditCode"] = e.Code;
                    Session["EditPhone"] = e.Phone;
                    Session["EditEmail"] = e.Email;
                    Session["EditAddress"] = e.Address;                  
                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

        public ActionResult LocationList()
        {
            LocationModels.LocationModel locationModel = new LocationModels.LocationModel();
            model.LstLocation = new List<LocationModels.LocationModel>();
            lstLocationList = new List<LocationModels.LocationModel>();

            foreach (var location in Entities.PrcGetLocation())
            {
                locationModel = new LocationModels.LocationModel();
                locationModel.LocationID = location.LocationID;
                locationModel.LocationName = location.LocationName;
                locationModel.ShortName = location.ShortName;
                locationModel.Description = location.Description;
                locationModel.Code = location.Code;
                locationModel.Phone = location.Phone;
                locationModel.Address = location.Address;
                locationModel.Email = location.Email;              

                model.LstLocation.Add(locationModel);
                lstLocationList.Add(locationModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string locationName, string shortName, string description, string code, string phone, string email, string address)
        {
            string message;
            int saveOk;
            var locs = (from loc in Entities.SLocations where loc.Code == code select loc).ToList();
            if (locs.Count() == 0)
            {
                Entities.PrcInsertLocation(locationName, shortName, description, code, phone, email, address);
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
        public JsonResult ViewAction(int locationId)
        {
            string locationName = "", shortName = "", description = "", code = "", phone = "", email = "", address = "";
            var viewLocation = lstLocationList.Where(c => c.LocationID == locationId);
            foreach (var e in viewLocation)
            {
                locationName = e.LocationName;
                shortName = e.ShortName;
                description = e.Description;
                code = e.Code;
                phone = e.Phone;
                email = e.Email;
                address = e.Address;              
                break;
            }

            var myResult = new
            {
                LocationName = locationName,
                ShortName = shortName,
                Description = description,
                Code = code,
                Phone = phone,
                Email = email,
                Address = address              
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string locationName, string shortName, string description, string code, string phone, string email, string address)
        {
            string message;
            int editOk;
            var locs = (from loc in Entities.SLocations where loc.Code == code where loc.LocationID != editLocationID select loc).ToList();
            if (locs.Count() == 0)
            {
                Entities.PrcUpdateLocation(editLocationID, locationName, shortName, description, code, phone, email, address);
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
        public JsonResult SearchAction(string keyword)
        {
            LocationModels.LocationModel locationModel = new LocationModels.LocationModel();
            model.LstLocation = new List<LocationModels.LocationModel>();
            lstLocationList = new List<LocationModels.LocationModel>();

            foreach (var location in Entities.PrcSearchLocation(keyword))
            {
                locationModel = new LocationModels.LocationModel();
                locationModel.LocationID = location.LocationID;
                locationModel.LocationName = location.LocationName;
                locationModel.ShortName = location.ShortName;
                locationModel.Description = location.Description;
                locationModel.Code = location.Code;
                locationModel.Phone = location.Phone;
                locationModel.Address = location.Address;
                locationModel.Email = location.Email;
              
                model.LstLocation.Add(locationModel);
                lstLocationList.Add(locationModel);
            }

            return Json(model.LstLocation, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int locationId)
        {
            string message = "";
            bool IsSuccess = false;
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcDeleteLocation, (SqlConnection) Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LocationID", locationId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                IsSuccess = Convert.ToBoolean(reader["IsSuccess"]);
                message = Convert.ToString(reader["Message"]);
            }
            reader.Close();
            var result = new
            {
                IsSuccess = IsSuccess,
                Message = message
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
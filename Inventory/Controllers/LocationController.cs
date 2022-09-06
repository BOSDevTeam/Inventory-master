using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using System.Data.Entity.Core.Objects;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class LocationController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        LocationModels.LocationModel model = new LocationModels.LocationModel();
        static List<LocationModels.LocationModel> lstLocationList = new List<LocationModels.LocationModel>();
        static int editLocationID;

        public ActionResult LocationEntry(int locationId)
        {
            ViewBag.IsMultiBranch = isMultiBranch();
            if (isMultiBranch()) GetBranch();

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
                    Session["EditBranchID"] = e.BranchID;
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
            ViewBag.IsMultiBranch = isMultiBranch();
            if (isMultiBranch()) GetBranchDefaultInclude();
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
                locationModel.BranchID = location.BranchID;
                locationModel.BranchName = location.BranchName;

                model.LstLocation.Add(locationModel);
                lstLocationList.Add(locationModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string locationName, string shortName, string description, string code, string phone, string email, string address, int? branchId)
        {
            string message;
            int saveOk;
            var locs = (from loc in Entities.S_Location where loc.Code == code select loc).ToList();
            if (locs.Count() == 0)
            {
                Entities.PrcInsertLocation(locationName, shortName, description, code, phone, email, address, branchId);
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
            string locationName = "", shortName = "", description = "", code = "", phone = "", email = "", address = "", branchName = "";
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
                branchName = e.BranchName;
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
                Address = address,
                BranchName = branchName
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string locationName, string shortName, string description, string code, string phone, string email, string address, int? branchId)
        {
            string message;
            int editOk;
            var locs = (from loc in Entities.S_Location where loc.Code == code where loc.LocationID != editLocationID select loc).ToList();
            if (locs.Count() == 0)
            {
                Entities.PrcUpdateLocation(editLocationID, locationName, shortName, description, code, phone, email, address, branchId);
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
        public JsonResult SearchAction(string keyword, int branchId)
        {
            LocationModels.LocationModel locationModel = new LocationModels.LocationModel();
            model.LstLocation = new List<LocationModels.LocationModel>();
            lstLocationList = new List<LocationModels.LocationModel>();

            foreach (var location in Entities.PrcSearchLocation(keyword, branchId))
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
                locationModel.BranchID = location.BranchID;
                locationModel.BranchName = location.BranchName;

                model.LstLocation.Add(locationModel);
                lstLocationList.Add(locationModel);
            }

            return Json(model.LstLocation, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int locationId)
        {
            int delOk;
            var users = (from user in Entities.S_User where user.LocationID == locationId select user).ToList();
            if (users.Count == 0)
            {
                S_Location location = Entities.S_Location.Where(x => x.LocationID == locationId).Single<S_Location>();
                Entities.S_Location.Remove(location);
                Entities.SaveChanges();
                delOk = 1;
            }
            else
            {
                delOk = 0;
            }

            var myResult = new
            {
                DELOK = delOk
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        private void GetBranch()
        {
            foreach (var branch in Entities.S_Branch.OrderBy(m => m.Code))
            {
                model.Branches.Add(new SelectListItem { Text = branch.BranchName, Value = branch.BranchID.ToString() });
            }
        }

        private void GetBranchDefaultInclude()
        {
            model.Branches.Add(new SelectListItem { Text = "Branch", Value = "0" });
            foreach (var branch in Entities.S_Branch.OrderBy(m => m.Code))
            {
                model.Branches.Add(new SelectListItem { Text = branch.BranchName, Value = branch.BranchID.ToString() });
            }
        }

        private bool isMultiBranch()
        {
            CompanySettingModels cModel = new CompanySettingModels();
            var isMultiBranch = Entities.S_CompanySetting.Select(c => c.IsMultiBranch);
            cModel.IsMultiBranch = isMultiBranch.FirstOrDefault();
            return Convert.ToBoolean(cModel.IsMultiBranch);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class VoucherSettingController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        VoucherSettingModels model = new VoucherSettingModels();
        static List<VoucherSettingModels> lstVouSettingList = new List<VoucherSettingModels>();
        static int editVouSettingID;

        public ActionResult VoucherSettingEntry(int Id)
        {           
            ViewBag.IsMultiBranch = isMultiBranch();
            clearSessionPhoto();
            if (Id != 0)
            {
                Session["IsEdit"] = 1;
                editVouSettingID = Id;
                var editVouSetting = lstVouSettingList.Where(c => c.ID == Id);
                foreach (var e in editVouSetting)
                {
                    Session["EditBranchID"] = e.BranchID;
                    Session["EditLocationID"] = e.LocationID;
                    Session["EditHeaderName"] = e.HeaderName;
                    Session["EditHeaderDesp"] = e.HeaderDesp;
                    Session["EditPhone"] = e.HeaderPhone;
                    Session["EditAddress"] = e.HeaderAddress;
                    Session["EditOtherHeader1"] = e.OtherHeader1;
                    Session["EditOtherHeader2"] = e.OtherHeader2;
                    Session["EditFooterMessage1"] = e.FooterMessage1;
                    Session["EditFooterMessage2"] = e.FooterMessage2;
                    Session["EditFooterMessage3"] = e.FooterMessage3;

                    if (e.VoucherLogo != null)
                    {
                        ViewBag.Photo = true;
                        model.VoucherLogo = e.VoucherLogo;
                        model.Base64Photo = Convert.ToBase64String(e.VoucherLogo);
                    }
                    else
                    {
                        ViewBag.Photo = false;
                    }

                    if (e.BranchID != 0) GetLocation(e.BranchID);
                    else GetAllLocation();

                    if (isMultiBranch()) GetBranch();

                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
                if (isMultiBranch())
                {
                    GetBranch();
                    GetLocation();
                }
                else
                {
                    GetAllLocation();
                }
            }
            return View(model);
        }

        public ActionResult VoucherSettingList()
        {           
            ViewBag.IsMultiBranch = isMultiBranch();
            if (isMultiBranch()) GetBranchDefaultInclude();
            GetLocationDefaultInclude();
            VoucherSettingModels vouSettingModel = new VoucherSettingModels();
            model.LstVoucherSetting = new List<VoucherSettingModels>();
            lstVouSettingList = new List<VoucherSettingModels>();

            foreach (var vou in Entities.PrcGetVoucherSetting())
            {
                vouSettingModel = new VoucherSettingModels();
                vouSettingModel.ID = vou.ID;
                vouSettingModel.BranchID = vou.BranchID;
                vouSettingModel.BranchName = vou.BranchName;
                vouSettingModel.LocationID = vou.LocationID;
                vouSettingModel.LocationName = vou.LocationName;
                vouSettingModel.HeaderName = vou.HeaderName;
                vouSettingModel.HeaderDesp = vou.HeaderDesp;
                vouSettingModel.HeaderPhone = vou.HeaderPhone;
                vouSettingModel.HeaderAddress = vou.HeaderAddress;
                vouSettingModel.OtherHeader1 = vou.OtherHeader1;
                vouSettingModel.OtherHeader2 = vou.OtherHeader2;
                vouSettingModel.FooterMessage1 = vou.FooterMessage1;
                vouSettingModel.FooterMessage2 = vou.FooterMessage2;
                vouSettingModel.FooterMessage3 = vou.FooterMessage3;

                if (vou.VoucherLogo != null)
                {
                    vouSettingModel.VoucherLogo = vou.VoucherLogo;
                    vouSettingModel.Base64Photo = Convert.ToBase64String(vou.VoucherLogo);
                }

                model.LstVoucherSetting.Add(vouSettingModel);
                lstVouSettingList.Add(vouSettingModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SearchAction(int branchId)
        {
            VoucherSettingModels vouSettingModel = new VoucherSettingModels();
            model.LstVoucherSetting = new List<VoucherSettingModels>();
            lstVouSettingList = new List<VoucherSettingModels>();

            foreach (var vou in Entities.PrcSearchVoucherSetting(branchId))
            {
                vouSettingModel = new VoucherSettingModels();
                vouSettingModel.ID = vou.ID;
                vouSettingModel.BranchID = vou.BranchID;
                vouSettingModel.BranchName = vou.BranchName;
                vouSettingModel.LocationID = vou.LocationID;
                vouSettingModel.LocationName = vou.LocationName;
                vouSettingModel.HeaderName = vou.HeaderName;
                vouSettingModel.HeaderDesp = vou.HeaderDesp;
                vouSettingModel.HeaderPhone = vou.HeaderPhone;
                vouSettingModel.HeaderAddress = vou.HeaderAddress;
                vouSettingModel.OtherHeader1 = vou.OtherHeader1;
                vouSettingModel.OtherHeader2 = vou.OtherHeader2;
                vouSettingModel.FooterMessage1 = vou.FooterMessage1;
                vouSettingModel.FooterMessage2 = vou.FooterMessage2;
                vouSettingModel.FooterMessage3 = vou.FooterMessage3;

                if (vou.VoucherLogo != null)
                {
                    vouSettingModel.VoucherLogo = vou.VoucherLogo;
                    vouSettingModel.Base64Photo = Convert.ToBase64String(vou.VoucherLogo);
                }

                model.LstVoucherSetting.Add(vouSettingModel);
                lstVouSettingList.Add(vouSettingModel);
            }

            return Json(model.LstVoucherSetting, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaveAction(string headerName, string headerDesp, string headerPhone, string headerAddress, string otherHeader1, string otherHeader2, string footerMessage1, string footerMessage2, string footerMessage3, int? branchId, int locationId)
        {
            string message;
            int saveOk;
            if (!isMultiBranch())
            {
                var settings = (from setting in Entities.S_VoucherSetting where setting.LocationID == locationId select setting).ToList();
                if (settings.Count() == 0)
                {
                    S_VoucherSetting table = new S_VoucherSetting();
                    table.HeaderName = headerName;
                    table.HeaderDesp = headerDesp;
                    table.HeaderPhone = headerPhone;
                    table.HeaderAddress = headerAddress;
                    table.OtherHeader1 = otherHeader1;
                    table.OtherHeader2 = otherHeader2;
                    table.FooterMessage1 = footerMessage1;
                    table.FooterMessage2 = footerMessage2;
                    table.FooterMessage3 = footerMessage3;
                    table.BranchID = branchId;
                    table.LocationID = locationId;
                
                    if (Session["PhotoFile"] != null)
                    {
                        HttpPostedFileBase file = (HttpPostedFileBase)Session["PhotoFile"];
                        if (file != null)
                        {
                            if (file.ContentLength > 0)
                            {
                                table.VoucherLogo = new byte[file.ContentLength];
                                file.InputStream.Read(table.VoucherLogo, 0, file.ContentLength);
                            }
                        }
                        clearSessionPhoto();
                    }

                    Entities.S_VoucherSetting.Add(table);
                    Entities.SaveChanges();
               
                    message = "Saved Successfully!";
                    saveOk = 1;
                }
                else
                {
                    message = "Allow Only One Voucher Setting for One Location!";
                    saveOk = 0;
                }
            }
            else
            {
                var settings = (from setting in Entities.S_VoucherSetting where setting.BranchID == branchId where setting.LocationID == locationId select setting).ToList();
                if (settings.Count() == 0)
                {
                    S_VoucherSetting table = new S_VoucherSetting();
                    table.HeaderName = headerName;
                    table.HeaderDesp = headerDesp;
                    table.HeaderPhone = headerPhone;
                    table.HeaderAddress = headerAddress;
                    table.OtherHeader1 = otherHeader1;
                    table.OtherHeader2 = otherHeader2;
                    table.FooterMessage1 = footerMessage1;
                    table.FooterMessage2 = footerMessage2;
                    table.FooterMessage3 = footerMessage3;
                    table.BranchID = branchId;
                    table.LocationID = locationId;

                    if (Session["PhotoFile"] != null)
                    {
                        HttpPostedFileBase file = (HttpPostedFileBase)Session["PhotoFile"];
                        if (file != null)
                        {
                            if (file.ContentLength > 0)
                            {
                                table.VoucherLogo = new byte[file.ContentLength];
                                file.InputStream.Read(table.VoucherLogo, 0, file.ContentLength);
                            }
                        }
                        clearSessionPhoto();
                    }
              
                    Entities.S_VoucherSetting.Add(table);
                    Entities.SaveChanges();
                 
                    message = "Saved Successfully!";
                    saveOk = 1;
                }
                else
                {
                    message = "Allow Only One Voucher Setting for One Branch and One Location!";
                    saveOk = 0;
                }
            }

            var Result = new
            {
                MESSAGE = message,
                SAVEOK = saveOk
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string headerName, string headerDesp, string headerPhone, string headerAddress, string otherHeader1, string otherHeader2, string footerMessage1, string footerMessage2, string footerMessage3, int? branchId, int locationId)
        {
            string message = "";
            int editOk = 0;
            if (isMultiBranch())
            {
                var settings = (from setting in Entities.S_VoucherSetting where setting.ID != editVouSettingID where setting.BranchID == branchId where setting.LocationID == locationId select setting).ToList();
                if (settings.Count() == 0)
                {
                    var result = Entities.S_VoucherSetting.SingleOrDefault(c => c.ID == editVouSettingID);
                    if (result != null)
                    {
                        result.HeaderName = headerName;
                        result.HeaderDesp = headerDesp;
                        result.HeaderPhone = headerPhone;
                        result.HeaderAddress = headerAddress;
                        result.OtherHeader1 = otherHeader1;
                        result.OtherHeader2 = otherHeader2;
                        result.FooterMessage1 = footerMessage1;
                        result.FooterMessage2 = footerMessage2;
                        result.FooterMessage3 = footerMessage3;
                        result.BranchID = branchId;
                        result.LocationID = locationId;

                        if (Session["PhotoFile"] != null)
                        {
                            HttpPostedFileBase file = (HttpPostedFileBase)Session["PhotoFile"];
                            if (file != null)
                            {
                                if (file.ContentLength > 0)
                                {
                                    result.VoucherLogo = new byte[file.ContentLength];
                                    file.InputStream.Read(result.VoucherLogo, 0, file.ContentLength);
                                }
                            }
                            clearSessionPhoto();
                        }
                      
                        Entities.SaveChanges();                       

                        message = "Edited Successfully!";
                        editOk = 1;
                    }
                }
                else
                {
                    message = "Allow Only One Voucher Setting for One Branch and One Location!";
                    editOk = 0;
                }
            }
            else
            {
                var settings = (from setting in Entities.S_VoucherSetting where setting.ID != editVouSettingID where setting.LocationID == locationId select setting).ToList();
                if (settings.Count() == 0)
                {
                    var result = Entities.S_VoucherSetting.SingleOrDefault(c => c.ID == editVouSettingID);
                    if (result != null)
                    {
                        S_VoucherSetting table = new S_VoucherSetting();
                        table.HeaderName = headerName;
                        table.HeaderDesp = headerDesp;
                        table.HeaderPhone = headerPhone;
                        table.HeaderAddress = headerAddress;
                        table.OtherHeader1 = otherHeader1;
                        table.OtherHeader2 = otherHeader2;
                        table.FooterMessage1 = footerMessage1;
                        table.FooterMessage2 = footerMessage2;
                        table.FooterMessage3 = footerMessage3;
                        table.BranchID = branchId;
                        table.LocationID = locationId;

                        if (Session["PhotoFile"] != null)
                        {
                            HttpPostedFileBase file = (HttpPostedFileBase)Session["PhotoFile"];
                            if (file != null)
                            {
                                if (file.ContentLength > 0)
                                {
                                    table.VoucherLogo = new byte[file.ContentLength];
                                    file.InputStream.Read(table.VoucherLogo, 0, file.ContentLength);
                                }
                            }
                            clearSessionPhoto();
                        }

                        Entities.SaveChanges();                     

                        message = "Edited Successfully!";
                        editOk = 1;
                    }
                }
                else
                {
                    message = "Allow Only One Voucher Setting for One Location!";
                    editOk = 0;
                }
            }

            var Result = new
            {
                MESSAGE = message,
                EDITOK = editOk
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ViewAction(int Id)
        {
            string branchName = "", locationName = "", headerName = "", headerDesp = "", headerPhone = "", headerAddress = "", otherHeader1 = "", otherHeader2 = "", footerMessage1 = "", footerMessage2 = "", footerMessage3 = "", base64Photo = "";
            var viewVouSetting = lstVouSettingList.Where(c => c.ID == Id);
            foreach (var e in viewVouSetting)
            {
                branchName = e.BranchName;
                locationName = e.LocationName;
                headerName = e.HeaderName;
                headerDesp = e.HeaderDesp;
                headerPhone = e.HeaderPhone;
                headerAddress = e.HeaderAddress;
                otherHeader1 = e.OtherHeader1;
                otherHeader2 = e.OtherHeader2;
                footerMessage1 = e.FooterMessage1;
                footerMessage2 = e.FooterMessage2;
                footerMessage3 = e.FooterMessage3;
                if (e.VoucherLogo != null) base64Photo = Convert.ToBase64String(e.VoucherLogo);
                break;
            }

            var myResult = new
            {
                BranchName = branchName,
                LocationName = locationName,
                HeaderName = headerName,
                HeaderDesp = headerDesp,
                HeaderPhone = headerPhone,
                HeaderAddress = headerAddress,
                OtherHeader1 = otherHeader1,
                OtherHeader2 = otherHeader2,
                FooterMessage1 = footerMessage1,
                FooterMessage2 = footerMessage2,
                FooterMessage3 = footerMessage3,
                Base64Photo = base64Photo
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int Id)
        {
            S_VoucherSetting setting = Entities.S_VoucherSetting.Where(x => x.ID == Id).Single<S_VoucherSetting>();
            Entities.S_VoucherSetting.Remove(setting);
            Entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BranchSelectAction(int branchId)
        {
            LocationModels.LocationModel locationModel = new LocationModels.LocationModel();
            List<LocationModels.LocationModel> lstLocation = new List<LocationModels.LocationModel>();

            var locs = (from loc in Entities.S_Location where loc.BranchID == branchId select loc).ToList();
            for (int i = 0; i < locs.Count(); i++)
            {
                locationModel = new LocationModels.LocationModel();
                locationModel.LocationID = locs[i].LocationID;
                locationModel.LocationName = locs[i].LocationName;
                lstLocation.Add(locationModel);
            }

            return Json(lstLocation, JsonRequestBehavior.AllowGet);
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

        private void GetLocationDefaultInclude()
        {
            model.Locations.Add(new SelectListItem { Text = "Location", Value = "0" });
            foreach (var location in Entities.S_Location.OrderBy(m => m.Code))
            {
                model.Locations.Add(new SelectListItem { Text = location.LocationName, Value = location.LocationID.ToString() });
            }
        }

        private void GetLocation()
        {
            if (isMultiBranch())
            {
                var firstBranch = Entities.S_Branch.OrderBy(c => c.Code).Select(c => c.BranchID);
                int firstBranchId = firstBranch.FirstOrDefault();
                var locations = (from location in Entities.S_Location where location.BranchID == firstBranchId orderby location.Code select location).ToList();
                foreach (var location in locations)
                {
                    model.Locations.Add(new SelectListItem { Text = location.LocationName, Value = location.LocationID.ToString() });
                }
            }
            else
            {
                foreach (var location in Entities.S_Location.OrderBy(m => m.Code))
                {
                    model.Locations.Add(new SelectListItem { Text = location.LocationName, Value = location.LocationID.ToString() });
                }
            }
        }

        private void GetLocation(int? branchId)
        {
            var locations = (from location in Entities.S_Location where location.BranchID == branchId orderby location.Code select location).ToList();
            foreach (var location in locations)
            {
                model.Locations.Add(new SelectListItem { Text = location.LocationName, Value = location.LocationID.ToString() });
            }
        }

        private void GetAllLocation()
        {
            var locations = (from location in Entities.S_Location orderby location.Code select location).ToList();
            foreach (var location in locations)
            {
                model.Locations.Add(new SelectListItem { Text = location.LocationName, Value = location.LocationID.ToString() });
            }
        }

        private bool isMultiBranch()
        {
            CompanySettingModels cModel = new CompanySettingModels();
            var isMultiBranch = Entities.S_CompanySetting.Select(c => c.IsMultiBranch);
            cModel.IsMultiBranch = isMultiBranch.FirstOrDefault();
            return Convert.ToBoolean(cModel.IsMultiBranch);
        }

        [HttpPost]
        public JsonResult UploadFile()
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                Session["PhotoFile"] = file;               
            }
            return Json("");
        }

        [HttpGet]
        public JsonResult RefreshAction()
        {
            clearSessionPhoto();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        private void clearSessionPhoto()
        {
            Session["PhotoFile"] = null;
        }
    }
}
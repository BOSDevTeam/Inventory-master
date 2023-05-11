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
            clearSessionPhoto();
            if (Id != 0)
            {
                Session["IsEdit"] = 1;
                editVouSettingID = Id;
                var editVouSetting = lstVouSettingList.Where(c => c.ID == Id);
                foreach (var e in editVouSetting)
                {                  
                    Session["EditLocationID"] = e.LocationID;
                    Session["EditHeaderName"] = e.HeaderName;
                    Session["EditHeaderDesp"] = e.HeaderDesp;
                    Session["EditHeaderName2"] = e.HeaderName2;
                    Session["EditHeaderDesp2"] = e.HeaderDesp2;
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

                    GetAllLocation();

                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
                GetAllLocation();
            }
            return View(model);
        }

        public ActionResult VoucherSettingList()
        {           
            GetLocationDefaultInclude();
            VoucherSettingModels vouSettingModel = new VoucherSettingModels();
            model.LstVoucherSetting = new List<VoucherSettingModels>();
            lstVouSettingList = new List<VoucherSettingModels>();

            foreach (var vou in Entities.PrcGetVoucherSetting())
            {
                vouSettingModel = new VoucherSettingModels();
                vouSettingModel.ID = vou.ID;               
                vouSettingModel.LocationID = vou.LocationID;
                vouSettingModel.LocationName = vou.LocationName;
                vouSettingModel.HeaderName = vou.HeaderName;
                vouSettingModel.HeaderDesp = vou.HeaderDesp;
                vouSettingModel.HeaderName2 = vou.HeaderName2;
                vouSettingModel.HeaderDesp2 = vou.HeaderDesp2;
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
        public JsonResult SearchAction()
        {
            VoucherSettingModels vouSettingModel = new VoucherSettingModels();
            model.LstVoucherSetting = new List<VoucherSettingModels>();
            lstVouSettingList = new List<VoucherSettingModels>();

            foreach (var vou in Entities.PrcGetVoucherSetting())
            {
                vouSettingModel = new VoucherSettingModels();
                vouSettingModel.ID = vou.ID;             
                vouSettingModel.LocationID = vou.LocationID;
                vouSettingModel.LocationName = vou.LocationName;
                vouSettingModel.HeaderName = vou.HeaderName;
                vouSettingModel.HeaderDesp = vou.HeaderDesp;
                vouSettingModel.HeaderName2 = vou.HeaderName2;
                vouSettingModel.HeaderDesp2 = vou.HeaderDesp2;
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
        public JsonResult SaveAction(string headerName, string headerDesp, string headerPhone, string headerAddress, string otherHeader1, string otherHeader2, string footerMessage1, string footerMessage2, string footerMessage3, int locationId, string headerName2, string headerDesp2)
        {
            string message;
            int saveOk;

            var settings = (from setting in Entities.SVoucherSettings where setting.LocationID == locationId select setting).ToList();
            if (settings.Count() == 0)
            {
                SVoucherSetting table = new SVoucherSetting();
                table.HeaderName = headerName;
                table.HeaderDesp = headerDesp;
                table.HeaderName2 = headerName2;
                table.HeaderDesp2 = headerDesp2;
                table.HeaderPhone = headerPhone;
                table.HeaderAddress = headerAddress;
                table.OtherHeader1 = otherHeader1;
                table.OtherHeader2 = otherHeader2;
                table.FooterMessage1 = footerMessage1;
                table.FooterMessage2 = footerMessage2;
                table.FooterMessage3 = footerMessage3;              
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

                Entities.SVoucherSettings.Add(table);
                Entities.SaveChanges();

                message = "Saved Successfully!";
                saveOk = 1;
            }
            else
            {
                message = "Allow Only One Voucher Setting for One Location!";
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
        public JsonResult EditAction(string headerName, string headerDesp, string headerPhone, string headerAddress, string otherHeader1, string otherHeader2, string footerMessage1, string footerMessage2, string footerMessage3, int locationId, string headerName2, string headerDesp2)
        {
            string message = "";
            int editOk = 0;

            var settings = (from setting in Entities.SVoucherSettings where setting.ID != editVouSettingID where setting.LocationID == locationId select setting).ToList();
            if (settings.Count() == 0)
            {
                var result = Entities.SVoucherSettings.SingleOrDefault(c => c.ID == editVouSettingID);
                if (result != null)
                {
                    result.HeaderName = headerName;
                    result.HeaderDesp = headerDesp;
                    result.HeaderName2 = headerName2;
                    result.HeaderDesp2 = headerDesp2;
                    result.HeaderPhone = headerPhone;
                    result.HeaderAddress = headerAddress;
                    result.OtherHeader1 = otherHeader1;
                    result.OtherHeader2 = otherHeader2;
                    result.FooterMessage1 = footerMessage1;
                    result.FooterMessage2 = footerMessage2;
                    result.FooterMessage3 = footerMessage3;
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
                message = "Allow Only One Voucher Setting for One Location!";
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
        public JsonResult ViewAction(int Id)
        {
            string locationName = "", headerName = "", headerDesp = "", headerPhone = "", headerAddress = "", otherHeader1 = "", otherHeader2 = "", footerMessage1 = "", footerMessage2 = "", footerMessage3 = "", base64Photo = "", headerName2 = "", headerDesp2 = "";
            var viewVouSetting = lstVouSettingList.Where(c => c.ID == Id);
            foreach (var e in viewVouSetting)
            {              
                locationName = e.LocationName;
                headerName = e.HeaderName;
                headerDesp = e.HeaderDesp;
                headerName2 = e.HeaderName2;
                headerDesp2 = e.HeaderDesp2;
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
                LocationName = locationName,
                HeaderName = headerName,
                HeaderDesp = headerDesp,
                HeaderName2 = headerName2,
                HeaderDesp2 = headerDesp2,
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
            SVoucherSetting setting = Entities.SVoucherSettings.Where(x => x.ID == Id).Single<SVoucherSetting>();
            Entities.SVoucherSettings.Remove(setting);
            Entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        private void GetLocationDefaultInclude()
        {
            model.Locations.Add(new SelectListItem { Text = "Location", Value = "0" });
            foreach (var location in Entities.SLocations.OrderBy(m => m.Code))
            {
                model.Locations.Add(new SelectListItem { Text = location.LocationName, Value = location.LocationID.ToString() });
            }
        }

        private void GetLocation()
        {
            foreach (var location in Entities.SLocations.OrderBy(m => m.Code))
            {
                model.Locations.Add(new SelectListItem { Text = location.LocationName, Value = location.LocationID.ToString() });
            }
        }       

        private void GetAllLocation()
        {
            var locations = (from location in Entities.SLocations orderby location.Code select location).ToList();
            foreach (var location in locations)
            {
                model.Locations.Add(new SelectListItem { Text = location.LocationName, Value = location.LocationID.ToString() });
            }
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
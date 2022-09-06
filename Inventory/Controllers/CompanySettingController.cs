using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class CompanySettingController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        CompanySettingModels model = new CompanySettingModels();
        static int editID;

        public ActionResult CompanySettingEntry()
        {
            var cst = (from cs in Entities.S_CompanySetting select cs).ToList();
            clearSessionPhoto();
            if (cst.Count != 0)
            {
                Session["IsEdit"] = 1;
                foreach (var e in Entities.S_CompanySetting)
                {
                    editID = e.CompanyID;
                    Session["EditCompanyName"] = e.CompanyName;
                    Session["EditDescription"] = e.Description;
                    Session["EditPhone"] = e.Phone;
                    Session["EditAddress"] = e.Address;
                    Session["EditEmail"] = e.Email;
                    Session["EditWebsite"] = e.Website;
                    Session["EditTax"] = e.Tax;
                    Session["EditServiceCharges"] = e.ServiceCharges;

                    if ((e.Tax != null && e.Tax != "0") || (e.ServiceCharges != null && e.ServiceCharges != "0")) Session["EditIsTaxChargesVal"] = 1;
                    else Session["EditIsTaxChargesVal"] = 0;

                    if (e.IsMultiCurrency == true) Session["EditIsMultiCurrencyVal"] = 1;
                    else Session["EditIsMultiCurrencyVal"] = 0;

                    if (e.IsMultiUnit == true) Session["EditIsMultiUnitVal"] = 1;
                    else Session["EditIsMultiUnitVal"] = 0;

                    if (e.IsMultiBranch == true) Session["EditIsMultiBranchVal"] = 1;
                    else Session["EditIsMultiBranchVal"] = 0;

                    if (e.IsDifProductByBranch == true) Session["EditIsDifProductByBranchVal"] = 1;
                    else Session["EditIsDifProductByBranchVal"] = 0;

                    if (e.IsBarcode == true) Session["EditIsBarcodeVal"] = 1;
                    else Session["EditIsBarcodeVal"] = 0;

                    if (e.IsProductPhoto == true) Session["EditIsProductPhotoVal"] = 1;
                    else Session["EditIsProductPhotoVal"] = 0;

                    if (e.IsQRcode == true) Session["EditIsQRcodeVal"] = 1;
                    else Session["EditIsQRcodeVal"] = 0;

                    if (e.IsProductVariant == true) Session["EditIsProductVariantVal"] = 1;
                    else Session["EditIsProductVariantVal"] = 0;

                    if (e.Logo != null)
                    {
                        ViewBag.Photo = true;
                        model.Logo = e.Logo;
                        model.Base64Photo = Convert.ToBase64String(e.Logo);
                    }
                    else
                    {
                        ViewBag.Photo = false;
                    }
                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string companyName, string description, string phone, string email, string website, string address, string tax, string serviceCharges, bool isMultiBranch, bool isMultiUnit, bool isMultiCurrency, bool isBarcode, bool isQRcode, bool isProductVariant, bool isProductPhoto, bool isDifProductByBranch)
        {
            S_CompanySetting table = new S_CompanySetting();
            table.CompanyName = companyName;
            table.Description = description;
            table.Phone = phone;
            table.Email = email;
            table.Website = website;
            table.Address = address;
            if (tax.Length != 0) table.Tax = tax;
            else table.Tax = "0";
            if (serviceCharges.Length != 0) table.ServiceCharges = serviceCharges;
            else table.ServiceCharges = "0";
            table.IsMultiBranch = isMultiBranch;
            table.IsMultiUnit = isMultiUnit;
            table.IsMultiCurrency = isMultiCurrency;
            table.IsBarcode = isBarcode;
            table.IsQRcode = isQRcode;
            table.IsProductVariant = isProductVariant;
            table.IsProductPhoto = isProductPhoto;
            table.IsDifProductByBranch = isDifProductByBranch;           

            if (Session["PhotoFile"] != null)
            {
                HttpPostedFileBase file = (HttpPostedFileBase)Session["PhotoFile"];
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        table.Logo = new byte[file.ContentLength];
                        file.InputStream.Read(table.Logo, 0, file.ContentLength);
                    }
                }
                clearSessionPhoto();
            }

            Entities.S_CompanySetting.Add(table);
            Entities.SaveChanges();          

            var Result = new
            {
                MESSAGE = "Saved Successfully!"
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string companyName, string description, string phone, string email, string website, string address, string tax, string serviceCharges, bool isMultiBranch, bool isMultiUnit, bool isMultiCurrency, bool isBarcode, bool isQRcode, bool isProductVariant, bool isProductPhoto, bool isDifProductByBranch)
        {
            var result = Entities.S_CompanySetting.SingleOrDefault(c => c.CompanyID == editID);
            if (result != null)
            {
                result.CompanyName = companyName;
                result.Description = description;
                result.Phone = phone;
                result.Email = email;
                result.Website = website;
                result.Address = address;
                result.Tax = tax;
                result.ServiceCharges = serviceCharges;
                result.IsMultiBranch = isMultiBranch;
                result.IsMultiUnit = isMultiUnit;
                result.IsMultiCurrency = isMultiCurrency;
                result.IsBarcode = isBarcode;
                result.IsQRcode = isQRcode;
                result.IsProductVariant = isProductVariant;
                result.IsProductPhoto = isProductPhoto;
                result.IsDifProductByBranch = isDifProductByBranch;

                if (Session["PhotoFile"] != null)
                {
                    HttpPostedFileBase file = (HttpPostedFileBase)Session["PhotoFile"];
                    if (file != null)
                    {
                        if (file.ContentLength > 0)
                        {
                            result.Logo = new byte[file.ContentLength];
                            file.InputStream.Read(result.Logo, 0, file.ContentLength);
                        }
                    }
                    clearSessionPhoto();
                }             
                Entities.SaveChanges();              
            }

            var Result = new
            {
                MESSAGE = "Edited Successfully!"
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
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

        private void clearSessionPhoto()
        {
            Session["PhotoFile"] = null;
        }
    }
}
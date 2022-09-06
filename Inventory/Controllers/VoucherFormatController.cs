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
    public class VoucherFormatController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        VoucherFormatModels model = new VoucherFormatModels();
        static List<VoucherFormatModels> lstVouFormatList = new List<VoucherFormatModels>();
        static int editVouFormatID;

        public ActionResult VoucherFormatEntry(int Id)
        {          
            ViewBag.IsMultiBranch = isMultiBranch();
            if (isMultiBranch()) GetBranch();

            if (Id != 0)
            {
                Session["IsEdit"] = 1;
                editVouFormatID = Id;
                var editVouFormat = lstVouFormatList.Where(c => c.ID == Id);
                foreach (var e in editVouFormat)
                {
                    Session["EditBranchID"] = e.BranchID;
                    Session["EditBranchName"] = e.BranchName;
                    Session["EditPreFormat"] = e.PreFormat;
                    Session["EditMidFormat"] = e.MidFormat;
                    Session["EditPostFormat"] = e.PostFormat;
                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

        public ActionResult VoucherFormatList()
        {         
            ViewBag.IsMultiBranch = isMultiBranch();
            if (isMultiBranch()) GetBranchDefaultInclude();
            VoucherFormatModels vouFormatModel = new VoucherFormatModels();
            model.LstVoucherFormat = new List<VoucherFormatModels>();
            lstVouFormatList = new List<VoucherFormatModels>();

            foreach (var format in Entities.PrcGetVoucherFormat())
            {
                vouFormatModel = new VoucherFormatModels();
                vouFormatModel.ID = format.ID;
                vouFormatModel.BranchID = format.BranchID;
                vouFormatModel.BranchName = format.BranchName;
                vouFormatModel.PreFormat = format.PreFormat;
                vouFormatModel.MidFormat = format.MidFormat;
                vouFormatModel.PostFormat = format.PostFormat;

                model.LstVoucherFormat.Add(vouFormatModel);
                lstVouFormatList.Add(vouFormatModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(int? branchId, string preFormat, string midFormat, int postFormat)
        {
            string message;
            int saveOk;
            if (!isMultiBranch())
            {
                var formats = (from format in Entities.S_VoucherFormat select format).ToList();
                if (formats.Count() == 0)
                {
                    Entities.PrcInsertVoucherFormat(branchId, preFormat, midFormat, postFormat);
                    message = "Saved Successfully!";
                    saveOk = 1;
                }
                else
                {
                    message = "Allow Only One Voucher Format!";
                    saveOk = 0;
                }
            }
            else
            {
                var formats = (from format in Entities.S_VoucherFormat where format.BranchID == branchId select format).ToList();
                if (formats.Count() == 0)
                {
                    Entities.PrcInsertVoucherFormat(branchId, preFormat, midFormat, postFormat);
                    message = "Saved Successfully!";
                    saveOk = 1;
                }
                else
                {
                    message = "Allow Only One Voucher Format for One Branch!";
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
        public JsonResult EditAction(int? branchId, string preFormat, string midFormat, int postFormat)
        {
            string message;
            int editOk;
            if (isMultiBranch())
            {
                var formats = (from format in Entities.S_VoucherFormat where format.BranchID == branchId where format.ID != editVouFormatID select format).ToList();
                if (formats.Count() == 0)
                {
                    Entities.PrcUpdateVoucherFormat(editVouFormatID, branchId, preFormat, midFormat, postFormat);
                    message = "Edited Successfully!";
                    editOk = 1;
                }
                else
                {
                    message = "Allow Only One Voucher Format for One Branch!";
                    editOk = 0;
                }
            }
            else
            {
                Entities.PrcUpdateVoucherFormat(editVouFormatID, branchId, preFormat, midFormat, postFormat);
                message = "Edited Successfully!";
                editOk = 1;
            }

            var Result = new
            {
                MESSAGE = message,
                EDITOK = editOk
            };

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(int branchId)
        {
            VoucherFormatModels vouFormatModel = new VoucherFormatModels();
            model.LstVoucherFormat = new List<VoucherFormatModels>();
            lstVouFormatList = new List<VoucherFormatModels>();

            foreach (var format in Entities.PrcSearchVoucherFormat(branchId))
            {
                vouFormatModel = new VoucherFormatModels();
                vouFormatModel.ID = format.ID;
                vouFormatModel.BranchID = format.BranchID;
                vouFormatModel.BranchName = format.BranchName;
                vouFormatModel.PreFormat = format.PreFormat;
                vouFormatModel.MidFormat = format.MidFormat;
                vouFormatModel.PostFormat = format.PostFormat;

                model.LstVoucherFormat.Add(vouFormatModel);
                lstVouFormatList.Add(vouFormatModel);
            }

            return Json(model.LstVoucherFormat, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int Id)
        {
            S_VoucherFormat format = Entities.S_VoucherFormat.Where(x => x.ID == Id).Single<S_VoucherFormat>();
            Entities.S_VoucherFormat.Remove(format);
            Entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
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
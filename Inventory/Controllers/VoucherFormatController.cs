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
            if (Id != 0)
            {
                Session["IsEdit"] = 1;
                editVouFormatID = Id;
                var editVouFormat = lstVouFormatList.Where(c => c.ID == Id);
                foreach (var e in editVouFormat)
                {                  
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
            VoucherFormatModels vouFormatModel = new VoucherFormatModels();
            model.LstVoucherFormat = new List<VoucherFormatModels>();
            lstVouFormatList = new List<VoucherFormatModels>();

            foreach (var format in Entities.PrcGetVoucherFormat())
            {
                vouFormatModel = new VoucherFormatModels();
                vouFormatModel.ID = format.ID;              
                vouFormatModel.PreFormat = format.PreFormat;
                vouFormatModel.MidFormat = format.MidFormat;
                vouFormatModel.PostFormat = format.PostFormat;

                model.LstVoucherFormat.Add(vouFormatModel);
                lstVouFormatList.Add(vouFormatModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string preFormat, string midFormat, int postFormat)
        {
            string message;
            int saveOk;

            var formats = (from format in Entities.S_VoucherFormat select format).ToList();
            if (formats.Count() == 0)
            {
                Entities.PrcInsertVoucherFormat(preFormat, midFormat, postFormat);
                message = "Saved Successfully!";
                saveOk = 1;
            }
            else
            {
                message = "Allow Only One Voucher Format!";
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
        public JsonResult EditAction(string preFormat, string midFormat, int postFormat)
        {
            string message;
            int editOk;

            Entities.PrcUpdateVoucherFormat(editVouFormatID, preFormat, midFormat, postFormat);
            message = "Edited Successfully!";
            editOk = 1;

            var Result = new
            {
                MESSAGE = message,
                EDITOK = editOk
            };

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction()
        {
            VoucherFormatModels vouFormatModel = new VoucherFormatModels();
            model.LstVoucherFormat = new List<VoucherFormatModels>();
            lstVouFormatList = new List<VoucherFormatModels>();

            foreach (var format in Entities.PrcGetVoucherFormat())
            {
                vouFormatModel = new VoucherFormatModels();
                vouFormatModel.ID = format.ID;              
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
    }
}
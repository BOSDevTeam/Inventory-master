using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class AdjustTypeController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        AdjustTypeModels.AdjustTypeModel model = new AdjustTypeModels.AdjustTypeModel();
        static List<AdjustTypeModels.AdjustTypeModel> lstAdjustTypeList = new List<AdjustTypeModels.AdjustTypeModel>();
        static int editAdjustTypeID;

        public ActionResult AdjustTypeEntry(int adjustTypeId)
        {
            if (adjustTypeId != 0)
            {
                Session["IsEdit"] = 1;
                editAdjustTypeID = adjustTypeId;
                var editAdjustType = lstAdjustTypeList.Where(c => c.AdjustTypeID == adjustTypeId);
                foreach (var e in editAdjustType)
                {
                    Session["EditAdjustTypeName"] = e.AdjustTypeName;
                    Session["EditShortName"] = e.ShortName;
                    if (e.IsIncrease) Session["EditIsIncreaseVal"] = 1;
                    else Session["EditIsIncreaseVal"] = 0;
                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

        public ActionResult AdjustTypeList()
        {
            AdjustTypeModels.AdjustTypeModel adjustTypeModel = new AdjustTypeModels.AdjustTypeModel();
            model.LstAdjustType = new List<AdjustTypeModels.AdjustTypeModel>();
            lstAdjustTypeList = new List<AdjustTypeModels.AdjustTypeModel>();

            foreach (var adjust in Entities.S_AdjustType)
            {
                adjustTypeModel = new AdjustTypeModels.AdjustTypeModel();
                adjustTypeModel.AdjustTypeID = adjust.AdjustTypeID;
                adjustTypeModel.AdjustTypeName = adjust.AdjustTypeName;
                adjustTypeModel.ShortName = adjust.ShortName;
                adjustTypeModel.IsIncrease = adjust.IsIncrease;
                if (adjust.IsIncrease) adjustTypeModel.IncreaseDecrease = "Increase";
                else adjustTypeModel.IncreaseDecrease = "Decrease";

                model.LstAdjustType.Add(adjustTypeModel);
                lstAdjustTypeList.Add(adjustTypeModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string adjustTypeName, string shortName, bool isIncrease,string sign)
        {
            Entities.PrcInsertAdjustType(adjustTypeName, shortName, isIncrease,sign);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string adjustTypeName, string shortName, bool isIncrease, string sign)
        {
            Entities.PrcUpdateAdjustType(editAdjustTypeID, adjustTypeName, shortName, isIncrease,sign);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(string keyword)
        {
            AdjustTypeModels.AdjustTypeModel adjustTypeModel = new AdjustTypeModels.AdjustTypeModel();
            model.LstAdjustType = new List<AdjustTypeModels.AdjustTypeModel>();
            lstAdjustTypeList = new List<AdjustTypeModels.AdjustTypeModel>();

            foreach (var adjust in Entities.PrcSearchAdjustType(keyword))
            {
                adjustTypeModel = new AdjustTypeModels.AdjustTypeModel();
                adjustTypeModel.AdjustTypeID = adjust.AdjustTypeID;
                adjustTypeModel.AdjustTypeName = adjust.AdjustTypeName;
                adjustTypeModel.ShortName = adjust.ShortName;
                adjustTypeModel.IsIncrease = adjust.IsIncrease;
                if (adjust.IsIncrease) adjustTypeModel.IncreaseDecrease = "Increase";
                else adjustTypeModel.IncreaseDecrease = "Decrease";

                model.LstAdjustType.Add(adjustTypeModel);
                lstAdjustTypeList.Add(adjustTypeModel);
            }

            return Json(model.LstAdjustType, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int adjustTypeId)
        {
            S_AdjustType adjust = Entities.S_AdjustType.Where(x => x.AdjustTypeID == adjustTypeId).Single<S_AdjustType>();
            Entities.S_AdjustType.Remove(adjust);
            Entities.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
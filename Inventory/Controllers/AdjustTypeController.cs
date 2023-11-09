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
        AdjustTypeModels model = new AdjustTypeModels();
        static List<AdjustTypeModels> lstAdjustTypeList = new List<AdjustTypeModels>();
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
            AdjustTypeModels adjustTypeModel = new AdjustTypeModels();           
            lstAdjustTypeList = new List<AdjustTypeModels>();

            foreach (var adjust in Entities.SAdjustTypes)
            {
                adjustTypeModel = new AdjustTypeModels();
                adjustTypeModel.AdjustTypeID = adjust.AdjustTypeID;
                adjustTypeModel.AdjustTypeName = adjust.AdjustTypeName;
                adjustTypeModel.ShortName = adjust.ShortName;
                adjustTypeModel.IsIncrease = adjust.IsIncrease;
                if (adjust.IsIncrease) adjustTypeModel.IncreaseDecrease = "Increase";
                else adjustTypeModel.IncreaseDecrease = "Decrease";              
                lstAdjustTypeList.Add(adjustTypeModel);
            }
            ViewData["LstAdjustType"] = lstAdjustTypeList;

            return View();
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
            AdjustTypeModels adjustTypeModel = new AdjustTypeModels();
            model.LstAdjustType = new List<AdjustTypeModels>();
            lstAdjustTypeList = new List<AdjustTypeModels>();

            foreach (var adjust in Entities.PrcSearchAdjustType(keyword))
            {
                adjustTypeModel = new AdjustTypeModels();
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
            SAdjustType adjust = Entities.SAdjustTypes.Where(x => x.AdjustTypeID == adjustTypeId).Single<SAdjustType>();
            Entities.SAdjustTypes.Remove(adjust);
            Entities.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
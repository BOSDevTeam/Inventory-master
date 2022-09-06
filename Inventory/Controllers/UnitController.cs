using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class UnitController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        UnitModels model = new UnitModels();
        static List<UnitModels> lstUnitList = new List<UnitModels>();
        static int editUnitID;     

        public ActionResult UnitEntry(int unitId)
        {          
            GetUnitLevel();
            if (unitId != 0)
            {
                Session["IsEdit"] = 1;
                editUnitID = unitId;
                var editUnit = lstUnitList.Where(c => c.UnitID == unitId);
                foreach (var e in editUnit)
                {
                    Session["EditUnitName"] = e.UnitName;
                    Session["EditKeyword"] = e.Keyword;
                    Session["EditULID"] = e.ULID;

                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;

            }
            return View(model);
        }

        public ActionResult UnitList()
        {           
            UnitModels unitModel = new UnitModels();
            model.LstUnit = new List<UnitModels>();
            lstUnitList = new List<UnitModels>();

            foreach (var unit in Entities.PrcGetUnit())
            {
                unitModel = new UnitModels();
                unitModel.UnitID = unit.UnitID;
                unitModel.UnitName = unit.UnitName;
                unitModel.Keyword = unit.Keyword;
                unitModel.ULID = unit.ULID;
                unitModel.ULName = unit.ULName;
                unitModel.ULOrder = unit.ULOrder;

                model.LstUnit.Add(unitModel);
                lstUnitList.Add(unitModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string unitName, string keyword, short ulId)
        {
            string message;
            int saveOk;

            var units = (from unit in Entities.S_Unit where unit.ULID == ulId select unit).ToList();
            if (units.Count() == 0)
            {
                S_Unit table = new S_Unit();
                table.UnitName = unitName;
                table.Keyword = keyword;
                table.ULID = ulId;
                Entities.S_Unit.Add(table);
                Entities.SaveChanges();
                message = "Saved Successfully!";
                saveOk = 1;
            }
            else
            {
                message = "Allow Only One Unit Level for One Unit!";
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
        public JsonResult EditAction(string unitName, string keyword, short ulId)
        {
            string message = "";
            int editOk = 0;

            var units = (from unit in Entities.S_Unit where unit.UnitID != editUnitID where unit.ULID == ulId select unit).ToList();
            if (units.Count() == 0)
            {
                var result = Entities.S_Unit.SingleOrDefault(c => c.UnitID == editUnitID);
                if (result != null)
                {
                    result.UnitName = unitName;
                    result.Keyword = keyword;
                    result.ULID = ulId;

                    Entities.SaveChanges();
                    message = "Edited Successfully!";
                    editOk = 1;
                }
            }
            else
            {
                message = "Allow Only One Unit Level for One Unit!";
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
        public JsonResult DeleteAction(int unitId)
        {
            int delOk;
            var pros = (from pro in Entities.S_ProductUnit where pro.UnitID == unitId select pro).ToList();
            if (pros.Count == 0)
            {
                S_Unit unit = Entities.S_Unit.Where(x => x.UnitID == unitId).Single<S_Unit>();
                Entities.S_Unit.Remove(unit);
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

        private void GetUnitLevel()
        {
            var levels = (from level in Entities.Sys_UnitLevel select level).OrderBy(c => c.ULOrder).ToList();
            foreach (var level in levels)
            {
                model.UnitLevels.Add(new SelectListItem { Text = level.ULName, Value = level.ULID.ToString() });
            }
        }
    }
}
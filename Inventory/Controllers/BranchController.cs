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
    public class BranchController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        BranchModels.BranchModel model = new BranchModels.BranchModel();
        static List<BranchModels.BranchModel> lstBranchList = new List<BranchModels.BranchModel>();
        static int editBranchID;

        public ActionResult BranchEntry(int branchId)
        {
            if (branchId != 0)
            {
                Session["IsEdit"] = 1;
                editBranchID = branchId;
                var editBranch = lstBranchList.Where(c => c.BranchID == branchId);
                foreach (var e in editBranch)
                {
                    Session["EditBranchName"] = e.BranchName;
                    Session["EditShortName"] = e.ShortName;
                    Session["EditDescription"] = e.Description;
                    Session["EditCode"] = e.Code;
                    Session["EditPhone"] = e.Phone;
                    Session["EditEmail"] = e.Email;
                    Session["EditAddress"] = e.Address;
                    Session["EditTax"] = e.Tax;
                    Session["EditServiceCharges"] = e.ServiceCharges;
                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

        public ActionResult BranchList()
        {
            BranchModels.BranchModel branchModel = new BranchModels.BranchModel();
            model.LstBranch = new List<BranchModels.BranchModel>();
            lstBranchList = new List<BranchModels.BranchModel>();

            foreach (var branch in Entities.S_Branch.OrderBy(m => m.Code))
            {
                branchModel = new BranchModels.BranchModel();
                branchModel.BranchID = branch.BranchID;
                branchModel.BranchName = branch.BranchName;
                branchModel.ShortName = branch.ShortName;
                branchModel.Description = branch.Description;
                branchModel.Code = branch.Code;
                branchModel.Phone = branch.Phone;
                branchModel.Address = branch.Address;
                branchModel.Email = branch.Email;
                branchModel.Tax = branch.Tax;
                branchModel.ServiceCharges = branch.ServiceCharges;

                model.LstBranch.Add(branchModel);
                lstBranchList.Add(branchModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string branchName, string shortName, string description, string code, string phone, string email, string address, string tax, string serviceCharges)
        {
            string message;
            int saveOk;
            var branches = (from branch in Entities.S_Branch where branch.Code == code select branch).ToList();
            if (branches.Count() == 0)
            {
                Entities.PrcInsertBranch(branchName, shortName, description, code, phone, email, address, tax, serviceCharges);
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
        public JsonResult ViewAction(int branchId)
        {
            string branchName = "", shortName = "", description = "", code = "", phone = "", email = "", address = "", tax = "", serviceCharges = "";
            var viewBranch = lstBranchList.Where(c => c.BranchID == branchId);
            foreach (var e in viewBranch)
            {
                branchName = e.BranchName;
                shortName = e.ShortName;
                description = e.Description;
                code = e.Code;
                phone = e.Phone;
                email = e.Email;
                address = e.Address;
                tax = e.Tax;
                serviceCharges = e.ServiceCharges;
                break;
            }

            var myResult = new
            {
                BranchName = branchName,
                ShortName = shortName,
                Description = description,
                Code = code,
                Phone = phone,
                Email = email,
                Address = address,
                Tax = tax,
                ServiceCharges = serviceCharges
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string branchName, string shortName, string description, string code, string phone, string email, string address, string tax, string serviceCharges)
        {
            string message;
            int editOk;
            var branches = (from branch in Entities.S_Branch where branch.Code == code where branch.BranchID != editBranchID select branch).ToList();
            if (branches.Count() == 0)
            {
                Entities.PrcUpdateBranch(editBranchID, branchName, shortName, description, code, phone, email, address, tax, serviceCharges);
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
            BranchModels.BranchModel branchModel = new BranchModels.BranchModel();
            model.LstBranch = new List<BranchModels.BranchModel>();
            lstBranchList = new List<BranchModels.BranchModel>();

            foreach (var branch in Entities.PrcSearchBranch(keyword))
            {
                branchModel = new BranchModels.BranchModel();
                branchModel.BranchID = branch.BranchID;
                branchModel.BranchName = branch.BranchName;
                branchModel.ShortName = branch.ShortName;
                branchModel.Description = branch.Description;
                branchModel.Code = branch.Code;
                branchModel.Phone = branch.Phone;
                branchModel.Address = branch.Address;
                branchModel.Email = branch.Email;
                branchModel.Tax = branch.Tax;
                branchModel.ServiceCharges = branch.ServiceCharges;

                model.LstBranch.Add(branchModel);
                lstBranchList.Add(branchModel);
            }

            return Json(model.LstBranch, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int branchId)
        {
            int delOk;
            var users = (from user in Entities.S_User where user.BranchID == branchId select user).ToList();
            var locs = (from loc in Entities.S_Location where loc.BranchID == branchId select loc).ToList();
            if (users.Count == 0 && locs.Count == 0)
            {
                S_Branch branch = Entities.S_Branch.Where(x => x.BranchID == branchId).Single<S_Branch>();
                Entities.S_Branch.Remove(branch);
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
    }
}
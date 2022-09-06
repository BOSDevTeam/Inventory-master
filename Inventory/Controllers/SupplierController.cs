using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class SupplierController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        SupplierModels.SupplierModel model = new SupplierModels.SupplierModel();
        static List<SupplierModels.SupplierModel> lstSupplierList = new List<SupplierModels.SupplierModel>();
        static int editSupplierID;

        public ActionResult SupplierEntry(int supplierId)
        {         
            ViewBag.IsMultiBranch = isMultiBranch();
            if (isMultiBranch()) GetBranch();
            GetDivision();
           
            


            if (supplierId != 0)
            {
                
                Session["IsEdit"] = 1;
                editSupplierID = supplierId;
                var editSupplier = lstSupplierList.Where(c => c.SupplierID == supplierId);
                foreach (var e in editSupplier)
                {
                    Session["EditSupplierName"] = e.SupplierName;
                    Session["EditCode"] = e.Code;
                    Session["EditPhone"] = e.Phone;
                    Session["EditEmail"] = e.Email;
                    Session["EditContact"] = e.Contact;
                    Session["EditAddress"] = e.Address;
                    Session["EditBranchID"] = e.BranchID;
                    Session["EditTownshipID"] = e.TownshipID;
                    Session["EditDivisionID"] = e.DivisionID;
                    GetTownship(e.DivisionID);
                    if (e.IsCredit) Session["EditIsCreditVal"] = 1;
                    else Session["EditIsCreditVal"] = 0;
                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

        public ActionResult SupplierList()
        {           
            ViewBag.IsMultiBranch = isMultiBranch();
            if (isMultiBranch()) GetBranchDefaultInclude();
            GetTownshipDefaultInclude();
            SupplierModels.SupplierModel supplierModel = new SupplierModels.SupplierModel();
            model.LstSupplier = new List<SupplierModels.SupplierModel>();
            lstSupplierList = new List<SupplierModels.SupplierModel>();

            foreach (var supplier in Entities.PrcGetSupplier())
            {
                supplierModel = new SupplierModels.SupplierModel();
                supplierModel.SupplierID = supplier.SupplierID;
                supplierModel.SupplierName = supplier.SupplierName;
                supplierModel.Code = supplier.Code;
                supplierModel.Phone = supplier.Phone;
                supplierModel.Contact = supplier.Contact;
                supplierModel.IsCredit = supplier.IsCredit;
                if (supplier.IsCredit) supplierModel.Credit = "Allow Credit";
                else supplierModel.Credit = "Not Allow Credit";
                supplierModel.IsDefault = supplier.IsDefault;
                supplierModel.Address = supplier.Address;
                supplierModel.Email = supplier.Email;
                supplierModel.BranchID = supplier.BranchID;
                supplierModel.BranchName = supplier.BranchName;
                supplierModel.TownshipID = supplier.TownshipID;
                supplierModel.TownshipName = supplier.TownshipName;
                supplierModel.DivisionID = supplier.DivisionID;
                supplierModel.DivisionName = supplier.DivisionName;

                model.LstSupplier.Add(supplierModel);
                lstSupplierList.Add(supplierModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string supplierName, string code, string phone, string email, string contact, string address, int townshipId, int? branchId, bool isCredit, int divisionId)
        {
            string message;
            int saveOk;
            var supt = (from sup in Entities.S_Supplier where sup.Code == code select sup).ToList();
            if (supt.Count() == 0)
            {
                Entities.PrcInsertSupplier(supplierName, code, phone, email, address, contact, townshipId, branchId, isCredit, divisionId);
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
        public JsonResult ViewAction(int supplierId)
        {
            string supplierName = "", code = "", phone = "", email = "", address = "", branchName = "", contact = "", townshipName = "", credit = "", divisionName ="";
            var viewSupplier = lstSupplierList.Where(c => c.SupplierID == supplierId);
            foreach (var e in viewSupplier)
            {
                supplierName = e.SupplierName;
                code = e.Code;
                phone = e.Phone;
                email = e.Email;
                address = e.Address;
                branchName = e.BranchName;
                contact = e.Contact;
                townshipName = e.TownshipName;
                divisionName = e.DivisionName;
                credit = e.Credit;
                break;
            }

            var myResult = new
            {
                SupplierName = supplierName,
                Code = code,
                Phone = phone,
                Email = email,
                Address = address,
                BranchName = branchName,
                Contact = contact,
                TownshipName = townshipName,
                DivisionName = divisionName,
                Credit = credit
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string supplierName, string code, string phone, string email, string contact, string address, int townshipId, int? branchId, bool isCredit, int divisionId)
        {
            string message;
            int editOk;
            var supt = (from sup in Entities.S_Supplier where sup.Code == code where sup.SupplierID != editSupplierID select sup).ToList();
            if (supt.Count() == 0)
            {
                Entities.PrcUpdateSupplier(editSupplierID, supplierName, code, phone, email, address, contact, townshipId, branchId, isCredit, divisionId);
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
        public JsonResult GetDivsionSelectTownship(int divisionId)
        {
            TownshipModels.TownshipModel town = new TownshipModels.TownshipModel();
            List<TownshipModels.TownshipModel> lstwon = new List<TownshipModels.TownshipModel>();
            foreach (var township in Entities.PrcGetDivisionSelectTownship(divisionId))
            {
                town = new TownshipModels.TownshipModel();
                town.TownshipID = township.TownshipID;
                town.TownshipName = township.TownshipName;
                lstwon.Add(town);
            }

            return Json(lstwon, JsonRequestBehavior.AllowGet);
        }

        /*
        [HttpGet]
        private void getTownshipByDivision(int divisionId)
        {
            foreach (var town in Entities.PrcGetDivisionSelectTownship(divisionId))
            {
                model.Townships.Add(new SelectListItem { Text = town.TownshipName, Value = town.TownshipID.ToString() });
            }
        }
        */

        [HttpGet]
        public JsonResult SearchAction(string keyword, int? branchId, int? townshipId)
        {
            SupplierModels.SupplierModel supplierModel = new SupplierModels.SupplierModel();
            model.LstSupplier = new List<SupplierModels.SupplierModel>();
            lstSupplierList = new List<SupplierModels.SupplierModel>();

            foreach (var supplier in Entities.PrcSearchSupplier(keyword, branchId, townshipId))
            {
                supplierModel = new SupplierModels.SupplierModel();
                supplierModel.SupplierID = supplier.SupplierID;
                supplierModel.SupplierName = supplier.SupplierName;
                supplierModel.Code = supplier.Code;
                supplierModel.Phone = supplier.Phone;
                supplierModel.Contact = supplier.Contact;
                supplierModel.IsCredit = supplier.IsCredit;
                if (supplier.IsCredit) supplierModel.Credit = "Allow Credit";
                else supplierModel.Credit = "Not Allow Credit";
                supplierModel.IsDefault = supplier.IsDefault;
                supplierModel.Address = supplier.Address;
                supplierModel.Email = supplier.Email;
                supplierModel.BranchID = supplier.BranchID;
                supplierModel.BranchName = supplier.BranchName;
                supplierModel.TownshipID = supplier.TownshipID;
                supplierModel.TownshipName = supplier.TownshipName;
                supplierModel.DivisionID = supplier.DivisionID;
                supplierModel.DivisionName = supplier.DivisionName;

                model.LstSupplier.Add(supplierModel);
                lstSupplierList.Add(supplierModel);
            }

            return Json(model.LstSupplier, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int supplierId)
        {
            int delOk;
            //var mssale = (from ms in Entities.T_MasterSale where ms.CustomerID == customerId select ms).ToList();
            //if (mssale.Count == 0)
            //{
            S_Supplier supplier = Entities.S_Supplier.Where(x => x.SupplierID == supplierId).Single<S_Supplier>();
            Entities.S_Supplier.Remove(supplier);
            Entities.SaveChanges();
            delOk = 1;
            //}
            //else
            //{
            //    delOk = 0;
            //}

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

        private void GetDivision()
        {
            foreach (var div in Entities.S_Division.OrderBy(m=>m.Code))
            {
                model.Divisions.Add(new SelectListItem {Text = div.DivisionName, Value = div.DivisionID.ToString() });
            }
        }

        private void GetTownship(int editdivisionId)
        {
            foreach (var town in Entities.S_Township.Where(m=>m.DivisionID == editdivisionId).OrderBy(m => m.Code))
            {
                model.Townships.Add(new SelectListItem { Text = town.TownshipName, Value = town.TownshipID.ToString() });
            }
        }

        private void GetTownshipDefaultInclude()
        {
            model.Townships.Add(new SelectListItem { Text = "Township", Value = "0" });
            foreach (var town in Entities.S_Township.OrderBy(m => m.Code))
            {
                model.Townships.Add(new SelectListItem { Text = town.TownshipName, Value = town.TownshipID.ToString() });
            }
        }
    }
}
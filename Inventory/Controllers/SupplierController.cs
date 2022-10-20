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
            GetDivision();
            if (model.Divisions.Count != 0) GetTownship(Convert.ToInt32(model.Divisions[0].Value));
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
                    Session["EditTownshipID"] = e.TownshipID;
                    Session["EditDivisionID"] = e.DivisionID;
                    GetTownshipByDivision(e.DivisionID);
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
            GetDivisionDefaultInclude();
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
        public JsonResult SaveAction(string supplierName, string code, string phone, string email, string contact, string address, int townshipId, bool isCredit, int divisionId)
        {
            string message;
            int saveOk;
            var supt = (from sup in Entities.SSuppliers where sup.Code == code select sup).ToList();
            if (supt.Count() == 0)
            {
                var supPhone = (from sup in Entities.SSuppliers where sup.Phone == phone where sup.Phone != null select sup).ToList();
                if (supPhone.Count == 0 )
                {
                    Entities.PrcInsertSupplier(supplierName, code, phone, email, address, contact, townshipId, isCredit, divisionId);
                    message = "Saved Successfully!";
                    saveOk = 1;
                }
                else
                {
                    message = "Phone Duplicate!";
                    saveOk = 0;
                }
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
            string supplierName = "", code = "", phone = "", email = "", address = "", contact = "", townshipName = "", credit = "", divisionName ="";
            var viewSupplier = lstSupplierList.Where(c => c.SupplierID == supplierId);
            foreach (var e in viewSupplier)
            {
                supplierName = e.SupplierName;
                code = e.Code;
                phone = e.Phone;
                email = e.Email;
                address = e.Address;           
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
                Contact = contact,
                TownshipName = townshipName,
                DivisionName = divisionName,
                Credit = credit
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string supplierName, string code, string phone, string email, string contact, string address, int townshipId, bool isCredit, int divisionId)
        {
            string message;
            int editOk;
            var supt = (from sup in Entities.SSuppliers where sup.Code == code where sup.SupplierID != editSupplierID select sup).ToList();
            if (supt.Count() == 0)
            {
                var suptPhone = (from sup in Entities.SSuppliers where sup.Phone == phone where sup.Phone != null where sup.SupplierID != editSupplierID select sup).ToList();
                if (suptPhone.Count == 0)
                {
                    Entities.PrcUpdateSupplier(editSupplierID, supplierName, code, phone, email, address, contact, townshipId, isCredit, divisionId);
                    message = "Edited Successfully!";
                    editOk = 1;
                }
                else
                {
                    message = "Phone Duplicate!";
                    editOk = 0;
                }
                

               
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
            foreach (var township in Entities.PrcGetTownshipByDivision(divisionId))
            {
                town = new TownshipModels.TownshipModel();
                town.TownshipID = township.TownshipID;
                town.TownshipName = township.TownshipName;
                lstwon.Add(town);
            }

            return Json(lstwon, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(string keyword,int divisionId, int? townshipId)
        {
            SupplierModels.SupplierModel supplierModel = new SupplierModels.SupplierModel();
            model.LstSupplier = new List<SupplierModels.SupplierModel>();
            lstSupplierList = new List<SupplierModels.SupplierModel>();

            foreach (var supplier in Entities.PrcSearchSupplier(keyword, divisionId, townshipId))
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
            SSupplier supplier = Entities.SSuppliers.Where(x => x.SupplierID == supplierId).Single<SSupplier>();
            Entities.SSuppliers.Remove(supplier);
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

        private void GetDivision()
        {
            foreach (var div in Entities.SDivisions.OrderBy(m=>m.Code))
            {
                model.Divisions.Add(new SelectListItem {Text = div.DivisionName, Value = div.DivisionID.ToString() });
            }
        }

        private void GetTownshipByDivision(int editdivisionId)
        {
            foreach (var town in Entities.STownships.Where(m=>m.DivisionID == editdivisionId).OrderBy(m => m.Code))
            {
                model.Townships.Add(new SelectListItem { Text = town.TownshipName, Value = town.TownshipID.ToString() });
            }
        }

        private void GetTownship(int divisionId)
        {
            foreach (var town in Entities.STownships.Where(m => m.DivisionID == divisionId).OrderBy(m => m.Code))
            {
                model.Townships.Add(new SelectListItem { Text = town.TownshipName, Value = town.TownshipID.ToString() });
            }
        }


        private void GetDivisionDefaultInclude()
        {
            model.Divisions.Add(new SelectListItem { Text = "Division", Value = "0" });
            foreach (var div in Entities.SDivisions.OrderBy(m => m.Code))
            {
                model.Divisions.Add(new SelectListItem { Text = div.DivisionName, Value = div.DivisionID.ToString() });
            }
        }

        private void GetTownshipDefaultInclude()
        {
            model.Townships.Add(new SelectListItem { Text = "Township", Value = "0" });
            foreach (var town in Entities.STownships.OrderBy(m => m.Code))
            {
                model.Townships.Add(new SelectListItem { Text = town.TownshipName, Value = town.TownshipID.ToString() });
            }
        }
    }
}
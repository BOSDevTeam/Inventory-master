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
    public class CustomerController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        CustomerModels.CustomerModel model = new CustomerModels.CustomerModel();
        static List<CustomerModels.CustomerModel> lstCustomerList = new List<CustomerModels.CustomerModel>();
        static int editCustomerID;

        public ActionResult CustomerEntry(int customerId)
        {           
            GetDivision();
           
            if (customerId != 0)
            {
                Session["IsEdit"] = 1;
                editCustomerID = customerId;
                var editCustomer = lstCustomerList.Where(c => c.CustomerID == customerId);
                foreach (var e in editCustomer)
                {
                    Session["EditCustomerName"] = e.CustomerName;
                    Session["EditCode"] = e.Code;
                    Session["EditPhone"] = e.Phone;
                    Session["EditEmail"] = e.Email;
                    Session["EditContact"] = e.Contact;
                    Session["EditAddress"] = e.Address;
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

        public ActionResult CustomerList()
        {
            GetTownshipDefaultInclude();

            CustomerModels.CustomerModel customerModel = new CustomerModels.CustomerModel();
            model.LstCustomer = new List<CustomerModels.CustomerModel>();
            lstCustomerList = new List<CustomerModels.CustomerModel>();

            foreach (var customer in Entities.PrcGetCustomer())
            {
                customerModel = new CustomerModels.CustomerModel();
                customerModel.CustomerID = customer.CustomerID;
                customerModel.CustomerName = customer.CustomerName;
                customerModel.Code = customer.Code;
                customerModel.Phone = customer.Phone;
                customerModel.Contact = customer.Contact;
                customerModel.IsCredit = customer.IsCredit;
                if (customer.IsCredit) customerModel.Credit = "Allow Credit";
                else customerModel.Credit = "Not Allow Credit";
                customerModel.IsDefault = customer.IsDefault;
                customerModel.Address = customer.Address;
                customerModel.Email = customer.Email;             
                customerModel.TownshipID = customer.TownshipID;
                customerModel.TownshipName = customer.TownshipName;
                customerModel.DivisionID = Convert.ToInt32(customer.DivisionID);
                customerModel.DivisionName = customer.DivisionName;

                model.LstCustomer.Add(customerModel);
                lstCustomerList.Add(customerModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string customerName, string code, string phone, string email, string contact, string address, int townshipId, bool isCredit,int divisionId)
        {
            string message;
            int saveOk;
            var cust = (from cus in Entities.S_Customer where cus.Code == code select cus).ToList();
            if (cust.Count() == 0)
            {
                Entities.PrcInsertCustomer(customerName, code, phone, email, address, contact, townshipId, isCredit, divisionId);
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
        public JsonResult ViewAction(int customerId)
        {
            string customerName = "", code = "", phone = "", email = "", address = "", contact = "", townshipName = "", credit = "" , divisionName = "";
            var viewCustomer = lstCustomerList.Where(c => c.CustomerID == customerId);
            foreach (var e in viewCustomer)
            {
                customerName = e.CustomerName;
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
                CustomerName = customerName,
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
        public JsonResult EditAction(string customerName, string code, string phone, string email, string contact, string address, int townshipId, bool isCredit,int divisionId)
        {
            string message;
            int editOk;
            var cust = (from cus in Entities.S_Customer where cus.Code == code where cus.CustomerID != editCustomerID select cus).ToList();
            if (cust.Count() == 0)
            {
                Entities.PrcUpdateCustomer(editCustomerID, customerName, code, phone, email, address, contact, townshipId, isCredit, divisionId);
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
        public JsonResult SearchAction(string keyword, int? townshipId)
        {
            CustomerModels.CustomerModel customerModel = new CustomerModels.CustomerModel();
            model.LstCustomer = new List<CustomerModels.CustomerModel>();
            lstCustomerList = new List<CustomerModels.CustomerModel>();

            foreach (var customer in Entities.PrcSearchCustomer(keyword, townshipId))
            {
                customerModel = new CustomerModels.CustomerModel();
                customerModel.CustomerID = customer.CustomerID;
                customerModel.CustomerName = customer.CustomerName;
                customerModel.Code = customer.Code;
                customerModel.Phone = customer.Phone;
                customerModel.Contact = customer.Contact;
                customerModel.IsCredit = customer.IsCredit;
                if (customer.IsCredit) customerModel.Credit = "Allow Credit";
                else customerModel.Credit = "Not Allow Credit";
                customerModel.IsDefault = customer.IsDefault;
                customerModel.Address = customer.Address;
                customerModel.Email = customer.Email;              
                customerModel.TownshipID = customer.TownshipID;
                customerModel.TownshipName = customer.TownshipName;
                customerModel.DivisionID = customer.DivisionID;
                customerModel.DivisionName = customer.DivisionName;


                model.LstCustomer.Add(customerModel);
                lstCustomerList.Add(customerModel);
            }

            return Json(model.LstCustomer, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int customerId)
        {
            int delOk;
            var mssale = (from ms in Entities.T_MasterSale where ms.CustomerID == customerId select ms).ToList();
            if (mssale.Count == 0)
            {
                S_Customer customer = Entities.S_Customer.Where(x => x.CustomerID == customerId).Single<S_Customer>();
                Entities.S_Customer.Remove(customer);
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

        private void GetDivision()
        {
            foreach (var div in Entities.S_Division.OrderBy(m => m.Code))
            {
                model.Divisions.Add(new SelectListItem { Text = div.DivisionName, Value = div.DivisionID.ToString() });
            }
        }

        /*
        private void getTownshipByDivision(int divisionId)
        {
            foreach (var township in Entities.PrcGetDivisionSelectTownship(divisionId))
            {
                model.Townships.Add(new SelectListItem { Text = township.TownshipName, Value = township.TownshipID.ToString() });
            }
        }
        */

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
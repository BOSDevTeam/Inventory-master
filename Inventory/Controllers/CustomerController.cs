using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using System.Data.Entity.Core.Objects;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class CustomerController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        CustomerModels.CustomerModel model = new CustomerModels.CustomerModel();
        static List<CustomerModels.CustomerModel> lstCustomerList = new List<CustomerModels.CustomerModel>();
        Procedure procedure = new Procedure();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        static int editCustomerID;

        public ActionResult CustomerEntry(int customerId)
        {           
            GetDivision();
            if (model.Divisions.Count() != 0) GetTownship(Convert.ToInt32(model.Divisions[0].Value));

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

        public ActionResult CustomerList()
        {
            GetDivisionDefaultInclude();
            GetTownshipDefaultInclude();

            CustomerModels.CustomerModel customerModel = new CustomerModels.CustomerModel();            
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
                lstCustomerList.Add(customerModel);
            }
            ViewData["LstCustomer"] = lstCustomerList;

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string customerName, string code, string phone, string email, string contact, string address, int townshipId, bool isCredit,int divisionId)
        {
            string message;
            int saveOk;
            var cust = (from cus in Entities.SCustomers where cus.Code == code select cus).ToList();
            if (cust.Count() == 0)
            {
                var custphone = (from cus in Entities.SCustomers where cus.Phone == phone where cus.Phone != "" select cus).ToList();
                if (custphone.Count == 0)
                {
                    Entities.PrcInsertCustomer(customerName, code, phone, email, address, contact, townshipId, isCredit, divisionId);
                    message = "Saved Successfully!";
                    saveOk = 1;
                }
                else
                {
                    message = "Phone Duplicate";
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
            var cust = (from cus in Entities.SCustomers where cus.Code == code where cus.CustomerID != editCustomerID select cus).ToList();
            if (cust.Count() == 0)
            {
                var editcustphone = (from cus in Entities.SCustomers where cus.Phone == phone where cus.Phone != "" where cus.CustomerID != editCustomerID select cus).ToList();
                if (editcustphone.Count == 0)
                {
                    Entities.PrcUpdateCustomer(editCustomerID, customerName, code, phone, email, address, contact, townshipId, isCredit, divisionId);
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
        public JsonResult SearchAction(string keyword,int divisionId, int? townshipId)
        {
            CustomerModels.CustomerModel customerModel = new CustomerModels.CustomerModel();
            model.LstCustomer = new List<CustomerModels.CustomerModel>();
            lstCustomerList = new List<CustomerModels.CustomerModel>();

            foreach (var customer in Entities.PrcSearchCustomer(keyword, divisionId, townshipId))
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
            string message = "";
            bool IsSuccess = false;
            List<CustomerModels> CusList = new List<CustomerModels>();
            CustomerModels customerModels = new CustomerModels();
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcDeleteCustomer, (SqlConnection) Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            SqlDataReader reder = cmd.ExecuteReader();
            if (reder.Read())
            {
                message = Convert.ToString(reder["Message"]);
                IsSuccess = Convert.ToBoolean(reder["IsSuccess"]);
            }   
            reder.Close();
            dataConnectorSQL.Close();
            var result = new {
                Message = message,
                IsSuccess = IsSuccess
            };

            return Json(result, JsonRequestBehavior.AllowGet);
            
        }

        private void GetDivision()
        {
            foreach (var div in Entities.SDivisions.OrderBy(m => m.Code))
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

        public JsonResult GetDivsionSelectTownship(int? divisionId)
        {
            TownshipModels.TownshipModel town = new TownshipModels.TownshipModel();
            List<TownshipModels.TownshipModel> lstwon = new List<TownshipModels.TownshipModel>();
            if (divisionId > 0)
            {
                foreach (var township in Entities.PrcGetTownshipByDivision(divisionId))
                {
                    town = new TownshipModels.TownshipModel();
                    town.TownshipID = township.TownshipID;
                    town.TownshipName = township.TownshipName;
                    lstwon.Add(town);
                }
            }
            else
            {
                town = new TownshipModels.TownshipModel();
                town.TownshipID = 0;
                town.TownshipName = "Township";
                lstwon.Add(town);
                foreach(var township in Entities.PrcGetTownship())
                {
                    town = new TownshipModels.TownshipModel();
                    town.TownshipID = township.TownshipID;
                    town.TownshipName = township.TownshipName;
                    lstwon.Add(town);
                }
            }

            if (lstwon.Count == 0)
            {
                Session["TownshipID"] = 0;
            }

            return Json(lstwon, JsonRequestBehavior.AllowGet);
        }

        private void GetTownshipByDivision(int? editdivisionId)
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class CLSaleOrderController : Controller
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        CLMasterSaleOrderModels model = new CLMasterSaleOrderModels();
        CLTranSaleOrderModels tranmodel = new CLTranSaleOrderModels();
        List<CLMasterSaleOrderModels> Lstmso = new List<CLMasterSaleOrderModels>();
        List<CLTranSaleOrderModels> TsoList = new List<CLTranSaleOrderModels>();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        Procedure procedure = new Procedure();

        [HttpGet]
        public JsonResult SaveAction(int saleOrderID)
        {
            if (Session["TranSaleList"] != null)
            {
                List<CLTranSaleOrderModels> list = Session["TranSaleList"] as List<CLTranSaleOrderModels>;
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("ID", typeof(int)));
                dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                dt.Columns.Add(new DataColumn("Amount", typeof(int)));
                for (int i = 0; i < list.Count(); i++)
                {
                    dt.Rows.Add(list[i].ID, list[i].Quantity, list[i].Amount);
                }
                if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateCLTranSaleOrder, (SqlConnection)Session["SQLConnection"]);
                cmd.Parameters.AddWithValue("@SaleOrderID", saleOrderID);
                cmd.Parameters.AddWithValue("@CurrentDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@Subtotal", Convert.ToInt32(Session["TotalAmt"]));
                cmd.Parameters.AddWithValue("@TaxAmt", Convert.ToInt32(Session["TaxAmt"]));
                cmd.Parameters.AddWithValue("@ChargesAmt", Convert.ToInt32(Session["ChargeAmt"]));
                cmd.Parameters.AddWithValue("@Total", Convert.ToInt32(Session["Total"]));
                cmd.Parameters.AddWithValue("@temptbl", dt);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                dataConnectorSQL.Close();              
            }

            List<CLMasterSaleOrderModels> List = Session["LstMSale"] as List<CLMasterSaleOrderModels>;
            if (List.Count != 0)
            {
                int index = List.FindIndex(item => item.SaleOrderID == saleOrderID);             
                var result = List.Where(c => c.SaleOrderID == saleOrderID).SingleOrDefault();
                CLMasterSaleOrderModels newItem = new CLMasterSaleOrderModels();
                newItem.SaleOrderID = saleOrderID;
                newItem.OrderDateTime = result.OrderDateTime;
                newItem.OrderNumber = result.OrderNumber;
                newItem.ClientName = result.ClientName;
                newItem.ClientPhone = result.ClientPhone;
                newItem.CustomerName = result.CustomerName;
                newItem.CustomerPhone = result.CustomerPhone;
                newItem.DefaultCurrency = result.DefaultCurrency;
                newItem.Subtotal = Convert.ToInt32(Session["TotalAmt"]);
                newItem.TaxAmt = Convert.ToInt32(Session["TaxAmt"]);
                newItem.ChargesAmt = Convert.ToInt32(Session["ChargeAmt"]);
                newItem.Total = Convert.ToInt32(Session["Total"]);
                newItem.Remark = result.Remark;
                newItem.Counts = List.Count();
                List[index] = newItem;
                clearSession();
            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CLMasterSaleOrderList()
        {           
            CLMasterSaleOrderModels mastersalemodel = new CLMasterSaleOrderModels();
            model.Lstmso = new List<CLMasterSaleOrderModels>();
            Lstmso = new List<CLMasterSaleOrderModels>();
            foreach (var mastersale in Entities.PrcGetCLMasterSaleOrder())
            {
                mastersalemodel = new CLMasterSaleOrderModels();
                mastersalemodel.SaleOrderID = Convert.ToInt32(mastersale.SaleOrderID);
                mastersalemodel.OrderDateTime = mastersale.Date;
                mastersalemodel.OrderNumber = mastersale.OrderNumber;
                mastersalemodel.ClientID = mastersale.ClientID;
                mastersalemodel.ClientName = mastersale.ClientName;
                mastersalemodel.ClientPhone = mastersale.ClientPhone;
                mastersalemodel.CustomerID = mastersale.CustomerID;
                mastersalemodel.CustomerName = mastersale.CustomerName;
                mastersalemodel.CustomerPhone = mastersale.CustomerPhone;
                mastersalemodel.Subtotal = Convert.ToInt32(mastersale.Subtotal);
                mastersalemodel.Tax = Convert.ToInt32(mastersale.Tax);
                mastersalemodel.TaxAmt = Convert.ToInt32(mastersale.TaxAmt);
                mastersalemodel.Charges = Convert.ToInt32(mastersale.Charges);
                mastersalemodel.ChargesAmt = Convert.ToInt32(mastersale.ChargesAmt);
                mastersalemodel.Total = Convert.ToInt32(mastersale.Total);
                mastersalemodel.DefaultCurrency = mastersale.Currency;
                mastersalemodel.Remark = mastersale.Remark;
                model.Lstmso.Add(mastersalemodel);
                Lstmso.Add(mastersalemodel);
            }
            var lstmsale = Lstmso;
            Session["LstMSale"] = lstmsale;
            int count = Lstmso.Count();
            ViewBag.Count = count;
            return View(model);
        }

        [HttpGet]
        public JsonResult QuantityAction(int tranSaleId, int quantity)
        {
            int totalAmt = 0, tax = 0, charge = 0, total = 0; int qtyPriceAmount = 0;
            var jsonResult = new JsonResult();
            List<CLTranSaleOrderModels> List = new List<CLTranSaleOrderModels>();
            List = Session["TranSaleList"] as List<CLTranSaleOrderModels>;
            var result = List.Where(c => c.ID == tranSaleId).SingleOrDefault();
            if (result != null)
            {
                int amount = quantity * result.SalePrice;

                for (int i = 0; i < List.Count(); i++)
                {
                    if (List[i].ID == tranSaleId)
                    {
                        List[i].Quantity = quantity;
                        List[i].Amount = amount;
                        break;
                    }
                }

                
                qtyPriceAmount = amount;
                totalAmt = calculateSubtotal();
                tax = (totalAmt * result.Tax) / 100;
                charge = (totalAmt * result.Charges) / 100;
                total = totalAmt + tax + charge;
                Session["TotalAmt"] = totalAmt;
                Session["TaxAmt"] = tax;
                Session["ChargeAmt"] = charge;
                Session["Total"] = total;
            }

            var myResult = new
            {
                Subtotal = totalAmt,
                Total = total,
                TaxAmt = tax,
                ChargesAmt = charge,
                Amount = qtyPriceAmount,
                TranSaleOrderList = List,
            };

            jsonResult = Json(myResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public void clearSession()
        {
            Session["TotalAmt"] = null;
            Session["TaxAmt"] = null;
            Session["ChargeAmt"] = null;
            Session["Total"] = null;
        }

        private int calculateSubtotal()
        {
            int subtotal = 0;
            List<CLTranSaleOrderModels> list = new List<CLTranSaleOrderModels>();
            if (Session["TranSaleList"] != null)
            {
                list = Session["TranSaleList"] as List<CLTranSaleOrderModels>;
                for (int i = 0; i < list.Count(); i++)
                {
                    subtotal += list[i].Amount;
                }
            }

            return subtotal;
        }

        [HttpGet]
        public JsonResult SaleOrderDetail(int saleorderId)
        {
            clearSession();
            CLTranSaleOrderModels transaleModel = new CLTranSaleOrderModels();
            tranmodel.TsoList = new List<CLTranSaleOrderModels>();
            TsoList = new List<CLTranSaleOrderModels>();
            string saleordernumber = "", clientname = "", clientphone = "", customername = "", customerphone = "", datetime = "", remark = "";
            int subtotal = 0, taxAmt = 0, tax = 0, charges = 0, chargesAmt = 0, total = 0;
            List<CLMasterSaleOrderModels> lstMsale = Session["LstMSale"] as List<CLMasterSaleOrderModels>;
            var viewclsaleorder = lstMsale.Where(c => c.SaleOrderID == saleorderId);
            foreach (var e in viewclsaleorder)
            {
                saleorderId = e.SaleOrderID;
                saleordernumber = e.OrderNumber;
                datetime = e.OrderDateTime.ToString();
                clientname = e.ClientName;
                clientphone = e.ClientPhone;
                customername = e.CustomerName;
                customerphone = e.CustomerPhone;
                remark = e.Remark;
                subtotal = e.Subtotal;
                tax = e.Tax;
                taxAmt = e.TaxAmt;
                charges = e.Charges;
                chargesAmt = e.ChargesAmt;
                total = e.Total;
            }

            Session["Tax"] = tax;
            Session["Charges"] = charges;
            foreach (var transale in Entities.PrcGetCLTranSaleOrderBySaleOrderID(saleorderId))
            {
                transaleModel = new CLTranSaleOrderModels();
                transaleModel.ID = transale.ID;
                transaleModel.ProductName = transale.ProductName;
                transaleModel.Quantity = transale.Quantity;
                transaleModel.SalePrice = transale.Price;
                transaleModel.DefaultCurrency = transale.Currency;
                transaleModel.Amount = transale.Amount;
                transaleModel.Tax = Convert.ToInt32(transale.Tax);
                transaleModel.Charges = Convert.ToInt32(transale.Charges);
                tranmodel.TsoList.Add(transaleModel);
                TsoList.Add(transaleModel);
                Session["TranSaleList"] = TsoList;
            }

            var myResult = new
            {
                TranSaleOrderList = TsoList,
                SaleOrderID = saleorderId,
                OrderNumber = saleordernumber,
                OrderDateTime = datetime,
                ClientName = clientname,
                ClientPhone = clientphone,
                CustomerName = customername,
                CustomerPhone = customerphone,
                Remark = remark,
                Subtotal = subtotal,
                TaxAmt = taxAmt,
                ChargesAmt = chargesAmt,
                Total = total

            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchCLMasterSaleOrder(string keyword)
        {
            CLMasterSaleOrderModels mastersalemodel = new CLMasterSaleOrderModels();
            model.Lstmso = new List<CLMasterSaleOrderModels>();
            Lstmso = new List<CLMasterSaleOrderModels>();
            foreach (var s in Entities.PrcSearchCLMasterSaleOrder(keyword))
            {
                mastersalemodel = new CLMasterSaleOrderModels();
                mastersalemodel.SaleOrderID = Convert.ToInt32(s.SaleOrderID);
                mastersalemodel.OrderNumber = s.OrderNumber;
                mastersalemodel.ClientName = s.ClientName;
                mastersalemodel.ClientPhone = s.ClientPhone;
                mastersalemodel.CustomerName = s.CustomerName;
                mastersalemodel.CustomerPhone = s.CustomerPhne;
                mastersalemodel.OrderDateTime = s.Date;
                mastersalemodel.Subtotal = Convert.ToInt32(s.Subtotal);
                mastersalemodel.ChargesAmt = Convert.ToInt32(s.ChargesAmt);
                mastersalemodel.TaxAmt = Convert.ToInt32(s.TaxAmt);
                mastersalemodel.Total = Convert.ToInt32(s.Total);
                mastersalemodel.DefaultCurrency = s.Currency;
                mastersalemodel.Remark = s.Remark;
                int Countmastersale = Entities.PrcSearchCLMasterSaleOrder(keyword).Count();
                mastersalemodel.Counts = Countmastersale;
                model.Lstmso.Add(mastersalemodel);
                Lstmso.Add(mastersalemodel);
                Session["LstMSale"] = Lstmso;
            }

            return Json(model.Lstmso, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int transaleId)
        {
            int totalAmt = 0, tax = 0, charge = 0, total = 0, status;
            var jsonResult = new JsonResult();
            List<CLTranSaleOrderModels> List = new List<CLTranSaleOrderModels>();
            List = Session["TranSaleList"] as List<CLTranSaleOrderModels>;
            var result = List.Where(c => c.ID == transaleId).SingleOrDefault();
            for (int i = 0; i < List.Count(); i++)
            {
                if (List[i].ID == transaleId)
                {
                    List.Remove(List[i]);

                    break;
                }
            }

            if (result != null)
            {
                totalAmt = calculateSubtotal();
                tax = (totalAmt * result.Tax) / 100;
                charge = (totalAmt * result.Charges) / 100;
                total = totalAmt + tax + charge;

                Session["DeleteTotalAmt"] = totalAmt;
                Session["DeleteTaxAmt"] = tax;
                Session["DeleteChargeAmt"] = charge;
                Session["DeleteTotal"] = total;
            }
            if (List.Count() == 0) status = 0;  // transalelist empty
            else status = 1;

            var myResult = new
            {
                Subtotal = totalAmt,
                Total = total,
                TaxAmt = tax,
                Status = status,
                ChargesAmt = charge,
                TranSaleOrderList = List,
            };

            jsonResult = Json(myResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult DeleteActionCont(int saleOrderID, int transaleId)
        {
            if (Session["SQLConnection"] != null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateCLMasterSaleOrder, (SqlConnection)Session["SQLConnection"]);
            cmd.Parameters.AddWithValue("@ID", transaleId);
            cmd.Parameters.AddWithValue("@SaleOrderID", saleOrderID);
            cmd.Parameters.AddWithValue("@Subtotal", Convert.ToInt32(Session["DeleteTotalAmt"]));
            cmd.Parameters.AddWithValue("@TaxAmt", Convert.ToInt32(Session["DeleteTaxAmt"]));
            cmd.Parameters.AddWithValue("@ChargesAmt", Convert.ToInt32(Session["DeleteChargeAmt"]));
            cmd.Parameters.AddWithValue("@Total", Convert.ToInt32(Session["DeleteTotal"]));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            dataConnectorSQL.Close();

            List<CLMasterSaleOrderModels> List = Session["LstMSale"] as List<CLMasterSaleOrderModels>;

            if (List.Count != 0)
            {
                int index = List.FindIndex(item => item.SaleOrderID == saleOrderID);
                var result = List.Where(c => c.SaleOrderID == saleOrderID).SingleOrDefault();
                CLMasterSaleOrderModels newItem = new CLMasterSaleOrderModels();
                newItem.SaleOrderID = saleOrderID;
                newItem.OrderDateTime = result.OrderDateTime;
                newItem.OrderNumber = result.OrderNumber;
                newItem.ClientName = result.ClientName;
                newItem.ClientPhone = result.ClientPhone;
                newItem.CustomerName = result.CustomerName;
                newItem.CustomerPhone = result.CustomerPhone;
                newItem.DefaultCurrency = result.DefaultCurrency;
                newItem.Subtotal = Convert.ToInt32(Session["DeleteTotalAmt"]);
                newItem.TaxAmt = Convert.ToInt32(Session["DeleteTaxAmt"]);
                newItem.ChargesAmt = Convert.ToInt32(Session["DeleteChargeAmt"]);
                newItem.Total = Convert.ToInt32(Session["DeleteTotal"]);
                newItem.Remark = result.Remark;
                newItem.Counts = List.Count();
                List[index] = newItem;

            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }
    }
}
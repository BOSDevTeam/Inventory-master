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

        static List<CLMasterSaleOrderModels> Lstmso = new List<CLMasterSaleOrderModels>();
        List<CLTranSaleOrderModels> transaleList = new List<CLTranSaleOrderModels>();
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
                SqlCommand cmd = new SqlCommand(procedure.PrcCLUpdateTranSaleOrder, (SqlConnection)Session["SQLConnection"]);
                cmd.Parameters.AddWithValue("@SaleOrderID", saleOrderID);
                cmd.Parameters.AddWithValue("@Subtotal", Convert.ToInt32(Session["TotalAmt"]));
                cmd.Parameters.AddWithValue("@TaxAmt", Convert.ToInt32(Session["TaxAmt"]));
                cmd.Parameters.AddWithValue("@ChargesAmt", Convert.ToInt32(Session["ChargeAmt"]));
                cmd.Parameters.AddWithValue("@Total", Convert.ToInt32(Session["Total"]));
                cmd.Parameters.AddWithValue("@temptbl", dt);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                dataConnectorSQL.Close();
            }

            return Json("", JsonRequestBehavior.AllowGet);
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
                mastersalemodel.Total = Convert.ToInt32(mastersale.Total);
                mastersalemodel.DefaultCurrency = mastersale.Currency;
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

                Session["TranSaleList"] = List;
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
                Tax = tax,
                Charge = charge,
                Amount = qtyPriceAmount,
                TranSaleOrderList = List,
            };

            jsonResult = Json(myResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
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
            CLTranSaleOrderModels transaleModel = new CLTranSaleOrderModels();
            tranmodel.TsoList = new List<CLTranSaleOrderModels>();
            transaleList = new List<CLTranSaleOrderModels>();
            string saleordernumber = "", clientname = "", clientphone = "", customername = "", customerphone = "", datetime = "", remark = "";
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
            }

            foreach (var transale in Entities.PrcGetCLTranSaleOrderBySaleOrderID(saleorderId))
            {
                transaleModel = new CLTranSaleOrderModels();
                transaleModel.ID = transale.ID;
                transaleModel.ProductName = transale.ProductName;
                transaleModel.Quantity = transale.Quantity;
                transaleModel.SalePrice = transale.Price;
                transaleModel.Subtotal = transale.Subtotal;
                transaleModel.TaxAmt = transale.TaxAmt;
                transaleModel.Tax = transale.Tax;
                transaleModel.ChargesAmt = transale.ChargesAmt;
                transaleModel.Charges = transale.Charges;
                transaleModel.DefaultCurrency = transale.Currency;
                transaleModel.Total = transale.Total;
                transaleModel.Amount = transale.Amount;
                tranmodel.TsoList.Add(transaleModel);
                transaleList.Add(transaleModel);
            }

            var myResult = new
            {
                TranSaleOrderList = transaleList,
                SaleOrderID = saleorderId,
                OrderNumber = saleordernumber,
                OrderDateTime = datetime,
                ClientName = clientname,
                ClientPhone = clientphone,
                CustomerName = customername,
                CustomerPhone = customerphone,
                Remark = remark
            };

            var lstTSale = transaleList;
            Session["TranSaleList"] = lstTSale;
            return Json(myResult, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult SearchCLMasterSaleOrder(string keyword)
        {
            CLMasterSaleOrderModels mastersalemodel = new CLMasterSaleOrderModels();
            model.Lstmso = new List<CLMasterSaleOrderModels>();
            Lstmso = new List<CLMasterSaleOrderModels>();
            if (keyword != null)
            {
                foreach (var s in Entities.PrcSearchCLMasterSaleOrder(keyword))
                {
                    mastersalemodel.SaleOrderID = Convert.ToInt32(s.SaleOrderID);
                    mastersalemodel.OrderNumber = s.OrderNumber;
                    mastersalemodel.ClientName = s.ClientName;
                    mastersalemodel.CustomerName = s.CustomerName;
                    mastersalemodel.OrderDateTime = s.Date;
                    mastersalemodel.Total = Convert.ToInt32(s.Total);
                    mastersalemodel.DefaultCurrency = s.Currency;
                    int Countmastersale = Entities.PrcSearchCLMasterSaleOrder(keyword).Count();
                    mastersalemodel.Counts = Countmastersale;
                    model.Lstmso.Add(mastersalemodel);
                    Lstmso.Add(mastersalemodel);
                }
            }
            else
            {
            }

            return Json(Lstmso, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult DeleteAction(int transaleId)
        {
            int totalAmt = 0, tax = 0, charge = 0, total = 0;
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
            }
            var myResult = new
            {
                Subtotal = totalAmt,
                Total = total,
                Tax = tax,
                Charge = charge,
                TranSaleOrderList = List,
            };

            jsonResult = Json(myResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }


        [HttpGet]
        public JsonResult DeleteRealAction(int transaleId)
        {
            T_CLTranSaleOrder cltransale = Entities.T_CLTranSaleOrder.Where(x => x.ID == transaleId).Single<T_CLTranSaleOrder>();
            Entities.T_CLTranSaleOrder.Remove(cltransale);
            Entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}
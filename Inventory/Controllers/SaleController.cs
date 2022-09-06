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
    public class SaleController : MyController
    {
        InventoryDBEntities entity = new InventoryDBEntities();
        SaleModels.SaleModel model = new SaleModels.SaleModel();
        static List<SaleModels.CurrentSaleModel> lstCurrentSale = new List<SaleModels.CurrentSaleModel>();
        static double totalAmount, taxAmount, chargesAmount, netAmount, voucherDis, paidAmount, curDisAmount;
        static int editTranID;
        static int curDisPercent;
        static bool isDelivery;
        List<SaleModels.LocationModel> lstLocation = new List<SaleModels.LocationModel>();
        List<SaleModels.CurrencyModel> lstCurrency = new List<SaleModels.CurrencyModel>();
        SaleModels.VoucherSettingModel vouSettingModel = new SaleModels.VoucherSettingModel();
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        Procedure procedure = new Procedure();

        public ActionResult Sale(int tranId, bool isMultiBranch, bool isMultiCurrency, bool isMultiUnit, bool isBankPayment, int branchId, int userId)
        {
            int sortId = 1;
            SaleModels.SaleListModel saleListModel = new SaleModels.SaleListModel();
            SaleModels.CurrentSaleModel tranModel = new SaleModels.CurrentSaleModel();
            model.CurrentSaleList = new List<SaleModels.CurrentSaleModel>();
            lstCurrentSale = new List<SaleModels.CurrentSaleModel>();

            getSaleInfo(isMultiBranch, isMultiCurrency, isMultiUnit, branchId, userId, isBankPayment);

            if (tranId == 0)
            {
                Session["IsEdit"] = 0;
                netAmount = 0;
                totalAmount = 0;
                taxAmount = 0;
                chargesAmount = 0;
                paidAmount = 0;
                voucherDis = 0;
                curDisPercent = 0;
                curDisAmount = 0;
            }
            else
            {
                Session["IsEdit"] = 1;
                editTranID = tranId;

                if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();

                SqlCommand cmd = new SqlCommand(procedure.PrcGetMasterSaleByTranID, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TranID", tranId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    saleListModel = new SaleModels.SaleListModel();
                    Session["EditDate"] = reader["Date"];
                    Session["EditVoucher"] = reader["Voucher"];
                    Session["EditCustomer"] = reader["CustomerID"];
                    Session["EditLocation"] = reader["LocationID"];
                    Session["EditPayment"] = reader["PaymentID"];
                    Session["EditCurrency"] = reader["CurrencyID"];
                    Session["EditCreditLimitDay"] = reader["CreditLimitDay"];
                    Session["EditNetAmt"] = Convert.ToDouble(reader["NetAmt"]);
                    Session["EditTotalAmt"] = Convert.ToDouble(reader["TotalAmt"]);
                    Session["EditTaxAmt"] = Convert.ToDouble(reader["TaxAmt"]);
                    Session["EditChargesAmt"] = Convert.ToDouble(reader["ChargesAmt"]);
                    Session["EditFOCAmt"] = Convert.ToDouble(reader["FOCAmt"]);
                    Session["EditAdvancedPayAmt"] = Convert.ToDouble(reader["AdvancedPayAmt"]);
                    Session["EditVoucherDis"] = Convert.ToDouble(reader["VoucherDis"]);
                    Session["IsDelivery"] = reader["IsDelivery"];
                    Session["EditVouDisPercent"] = reader["VouDisPercent"];
                    Session["EditVouDisAmount"] = reader["VouDisAmount"];
                    Session["EditPaymentPercent"] = reader["PaymentPercent"];

                    netAmount = Convert.ToDouble(reader["NetAmt"]);
                    totalAmount = Convert.ToDouble(reader["TotalAmt"]);
                    taxAmount = Convert.ToDouble(reader["TaxAmt"]);
                    chargesAmount = Convert.ToDouble(reader["ChargesAmt"]);
                    paidAmount = Convert.ToDouble(reader["AdvancedPayAmt"]);
                    voucherDis = Convert.ToDouble(reader["VoucherDis"]);
                    curDisPercent = (int)reader["VouDisPercent"];
                    curDisAmount = Convert.ToDouble(reader["VouDisAmount"]);

                    reader.Close();

                    if (saleListModel.IsDelivery == true)
                    {
                        cmd = new SqlCommand("Select Date,Recipient,Phone,Address From T_Delivery Where TranID=" + tranId, (SqlConnection)Session["SQLConnection"]);
                        cmd.CommandType = CommandType.Text;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            Session["DeliDate"] = reader["Date"];
                            Session["DeliRecipient"] = reader["Recipient"];
                            Session["DeliPhone"] = reader["Phone"];
                            Session["DeliAddress"] = reader["Address"];
                        }
                        reader.Close();
                    }
                }

                cmd = new SqlCommand(procedure.PrcGetTranSaleByTranID, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TranID", tranId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tranModel = new SaleModels.CurrentSaleModel();
                    tranModel.ProductID = Convert.ToInt32(reader["ProductID"]);
                    tranModel.Code = Convert.ToString(reader["Code"]);
                    tranModel.ProductName = Convert.ToString(reader["ProductName"]);
                    if (reader["Keyword"] != null) tranModel.UnitVariant = Convert.ToString(reader["Keyword"]);
                    else if (reader["Variant"] != null) tranModel.UnitVariant = Convert.ToString(reader["Variant"]);
                    else tranModel.UnitVariant = "-";
                    tranModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                    tranModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
                    tranModel.DiscountAmt = Convert.ToDouble(reader["DiscountAmt"]);
                    tranModel.Amount = Convert.ToDouble(reader["TotalAmount"]);
                    tranModel.SortID = sortId;
                    tranModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                    tranModel.Variant = Convert.ToString(reader["Variant"]);
                    sortId++;
                    lstCurrentSale.Add(tranModel);
                }
                reader.Close();
                dataConnectorSQL.Close();

                var trans = lstCurrentSale;
                ViewData["EditLstTran"] = trans;
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult MainMenuAction(int mainMenuId)
        {
            List<SaleModels.SubMenuModel> lstSubMenu = new List<SaleModels.SubMenuModel>();
            SaleModels.SubMenuModel data = new SaleModels.SubMenuModel();
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select SubMenuID,SubMenuName From S_SubMenu Where MainMenuID=" + mainMenuId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data = new SaleModels.SubMenuModel();
                data.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                data.SubMenuName = Convert.ToString(reader["SubMenuName"]);

                lstSubMenu.Add(data);
            }
            reader.Close();
            dataConnectorSQL.Close();         

            return Json(lstSubMenu, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SubMenuAction(int subMenuId, string subMenuName, bool isProductPhoto)
        {
            var result = new JsonResult();

            List<SaleModels.ProductModel> lstProduct = new List<SaleModels.ProductModel>();
            SaleModels.ProductModel data = new SaleModels.ProductModel();
            Session["SubMenuName"] = subMenuName;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select ProductID,ProductName From S_Product Where SubMenuID=" + subMenuId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data = new SaleModels.ProductModel();
                data.ProductID = Convert.ToInt32(reader["ProductID"]);
                data.ProductName = Convert.ToString(reader["ProductName"]);
                data.IsPhoto = isProductPhoto;

                lstProduct.Add(data);
            }
            reader.Close();
            dataConnectorSQL.Close();

            result = Json(lstProduct, JsonRequestBehavior.AllowGet);

            return result;
        }

        [HttpGet]
        public JsonResult LoadProductPhotoAction(int subMenuId)
        {
            var result = new JsonResult();

            List<SaleModels.ProductModel> lstProduct = new List<SaleModels.ProductModel>();
            SaleModels.ProductModel data = new SaleModels.ProductModel();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select ProductID,ProductName,isnull(Photo,'') AS Photo From S_Product Where SubMenuID=" + subMenuId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data = new SaleModels.ProductModel();
                data.ProductID = Convert.ToInt32(reader["ProductID"]);
                data.ProductName = Convert.ToString(reader["ProductName"]);
                if (reader["Photo"].ToString().Length != 0) data.Base64Photo = Convert.ToBase64String((byte[])reader["Photo"]);

                lstProduct.Add(data);
            }
            reader.Close();
            dataConnectorSQL.Close();

            result = Json(lstProduct, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;

            return result;
        }

        [HttpGet]
        public JsonResult ProductAction(int productId)
        {
            List<SaleModels.ProductModel> lstProductByID = new List<SaleModels.ProductModel>();
            SaleModels.ProductUnitModel pUnitModel = new SaleModels.ProductUnitModel();
            SaleModels.ProductVariantModel pVariantModel = new SaleModels.ProductVariantModel();
            SaleModels.ProductModel model = new SaleModels.ProductModel();          

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select ProductID,Code,ProductName,SalePrice,isnull(DisPercent,0) AS DisPercent,IsUnit,IsVariant From S_Product Where ProductID=" + productId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model = new SaleModels.ProductModel();
                model.ProductID = Convert.ToInt32(reader["ProductID"]);
                model.Code = Convert.ToString(reader["Code"]);
                model.ProductName = Convert.ToString(reader["ProductName"]);
                model.SalePrice = Convert.ToDouble(reader["SalePrice"]);
                model.DisPercent = Convert.ToInt32(reader["DisPercent"]);
                model.IsUnit = Convert.ToBoolean(reader["IsUnit"]);
                model.IsVariant = Convert.ToBoolean(reader["IsVariant"]);
            }
            reader.Close();
            dataConnectorSQL.Close();

            if (model.IsUnit != null)
            {
                if (model.IsUnit == true) getUnitByProduct(productId, model, pUnitModel);                                  
            }

            if (model.IsVariant != null)
            {
                if (model.IsVariant == true) getVariantByProduct(productId, model, pVariantModel);                                   
            }

            lstProductByID.Add(model);

            return Json(lstProductByID, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ProductAddAction(int productId, string productName, double salePrice, int quantity, Int16 disPercent, string code, int unitId, string unitName, int variantId, string variantName,bool isMultiBranch,int branchId)
        {
            int? totalQuantity = 0;
            totalAmount = 0;
            netAmount = 0;
            
            SaleModels.CurrentSaleModel currentSaleModel = new SaleModels.CurrentSaleModel();

            currentSaleModel.SortID = lstCurrentSale.Count() + 1;
            currentSaleModel.Code = code;
            currentSaleModel.ProductID = productId;
            currentSaleModel.ProductName = productName;
            currentSaleModel.SalePrice = salePrice;
            currentSaleModel.Quantity = quantity;
            currentSaleModel.DisPercent = disPercent;
            if (unitId != 0)
            {
                currentSaleModel.UnitVariant = unitName;
                currentSaleModel.UnitID = unitId;
            }
            else if (variantId != 0)
            {
                currentSaleModel.UnitVariant = variantName;
                currentSaleModel.VariantID = variantId;
            }
            if (currentSaleModel.UnitVariant == null) currentSaleModel.UnitVariant = "-";
            currentSaleModel.DiscountAmt = Convert.ToDouble((salePrice * disPercent) / 100);
            currentSaleModel.Amount = (salePrice - currentSaleModel.DiscountAmt) * quantity;

            lstCurrentSale.Add(currentSaleModel);

            for (int i = 0; i < lstCurrentSale.Count(); i++)
            {
                totalAmount += lstCurrentSale[i].Amount;
                totalQuantity += lstCurrentSale[i].Quantity;
            }

            calcTaxAndChargesAmt(totalAmount,isMultiBranch,branchId);            

            voucherDis = 0;
            double disVouPercent = (totalAmount * curDisPercent) / 100;
            voucherDis = disVouPercent + curDisAmount;

            netAmount = (totalAmount + taxAmount + chargesAmount - voucherDis) - paidAmount;

            var myResult = new
            {
                CurrentSaleList = lstCurrentSale,
                TotalAmount = totalAmount,
                TaxAmount = taxAmount,
                ChargesAmount = chargesAmount,
                NetAmount = netAmount,
                VoucherDis = voucherDis,
                CurrentSaleCount = "Sale (" + totalQuantity + ")"
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ProductEditAction(int productId, string productName, double salePrice, int quantity, Int16 disPercent, string code, int unitId, string unitName, int variantId, string variantName, int sortId,bool isMultiBranch,int branchId)
        {
            int? totalQuantity = 0;
            totalAmount = 0;
            netAmount = 0;
           
            SaleModels.CurrentSaleModel currentSaleModel = new SaleModels.CurrentSaleModel();

            int index = lstCurrentSale.FindIndex(x => x.SortID == sortId);
            lstCurrentSale[index].SortID = sortId;
            lstCurrentSale[index].Code = code;
            lstCurrentSale[index].ProductID = productId;
            lstCurrentSale[index].ProductName = productName;
            lstCurrentSale[index].SalePrice = salePrice;
            lstCurrentSale[index].Quantity = quantity;
            lstCurrentSale[index].DisPercent = disPercent;
            if (unitId != 0)
            {
                lstCurrentSale[index].UnitVariant = unitName;
                lstCurrentSale[index].UnitID = unitId;
            }
            else if (variantId != 0)
            {
                lstCurrentSale[index].UnitVariant = variantName;
                lstCurrentSale[index].VariantID = variantId;
            }
            if (lstCurrentSale[index].UnitVariant == null) lstCurrentSale[index].UnitVariant = "-";
            lstCurrentSale[index].DiscountAmt = Convert.ToDouble((salePrice * disPercent) / 100);
            lstCurrentSale[index].Amount = (salePrice - lstCurrentSale[index].DiscountAmt) * quantity;

            for (int i = 0; i < lstCurrentSale.Count(); i++)
            {
                totalAmount += lstCurrentSale[i].Amount;
                totalQuantity += lstCurrentSale[i].Quantity;
            }

            calcTaxAndChargesAmt(totalAmount,isMultiBranch,branchId);

            voucherDis = 0;
            double disVouPercent = (totalAmount * curDisPercent) / 100;
            voucherDis = disVouPercent + curDisAmount;

            netAmount = (totalAmount + taxAmount + chargesAmount - voucherDis) - paidAmount;

            var myResult = new
            {
                CurrentSaleList = lstCurrentSale,
                TotalAmount = totalAmount,
                TaxAmount = taxAmount,
                ChargesAmount = chargesAmount,
                NetAmount = netAmount,
                VoucherDis = voucherDis,
                CurrentSaleCount = "Sale (" + totalQuantity + ")"
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaleEditAction(int sortId)
        {
            SaleModels.ProductUnitModel pUnitModel = new SaleModels.ProductUnitModel();
            SaleModels.ProductVariantModel pVariantModel = new SaleModels.ProductVariantModel();
            SaleModels.ProductModel pModel = new SaleModels.ProductModel();
            string code = "", name = "";
            int? quantity = 0;
            double salePrice = 0;
            short disPercent = 0;
            int? productId = 0, unitId = 0, variantId = 0;

            var editSale = lstCurrentSale.Where(c => c.SortID == sortId);
            foreach (var e in editSale)
            {
                productId = e.ProductID;
                code = e.Code;
                name = e.ProductName;
                quantity = e.Quantity;
                salePrice = e.SalePrice;
                disPercent = e.DisPercent;
                unitId = e.UnitID;
                variantId = e.VariantID;
                break;
            }

            if (isUnitByProduct(productId))getUnitByProduct(productId, pModel, pUnitModel);             
            
            if (isVariantByProduct(productId)) getVariantByProduct(productId, pModel, pVariantModel);
              
            var myResult = new
            {
                ProductID = productId,
                Code = code,
                Name = name,
                Quantity = quantity,
                SalePrice = salePrice,
                DisPercent = disPercent,
                ProductUnitList = pModel.ProductUnitList,
                SelectedUnitID = unitId,
                ProductVariantList = pModel.ProductVariantList,
                SelectedVariantID = variantId
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ProductDeleteAction(int sortId,bool isMultiBranch,int branchId)
        {
            int? totalQuantity = 0;
            totalAmount = 0;
            netAmount = 0;
          
            SaleModels.CurrentSaleModel currentSaleModel = new SaleModels.CurrentSaleModel();

            int index = lstCurrentSale.FindIndex(x => x.SortID == sortId);
            lstCurrentSale.RemoveAt(index);

            for (int i = 0; i < lstCurrentSale.Count(); i++)
            {
                totalAmount += lstCurrentSale[i].Amount;
                totalQuantity += lstCurrentSale[i].Quantity;
            }

            calcTaxAndChargesAmt(totalAmount,isMultiBranch,branchId);          

            voucherDis = 0;
            double disVouPercent = (totalAmount * curDisPercent) / 100;
            voucherDis = disVouPercent + curDisAmount;

            netAmount = (totalAmount + taxAmount + chargesAmount - voucherDis) - paidAmount;

            var myResult = new
            {
                CurrentSaleList = lstCurrentSale,
                TotalAmount = totalAmount,
                TaxAmount = taxAmount,
                ChargesAmount = chargesAmount,
                NetAmount = netAmount,
                VoucherDis = voucherDis,
                CurrentSaleCount = "Sale (" + totalQuantity + ")"
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AllProductDeleteAction()
        {
            lstCurrentSale.Clear();
            totalAmount = 0;
            taxAmount = 0;
            chargesAmount = 0;
            netAmount = 0;
            voucherDis = 0;
            paidAmount = 0;
            curDisAmount = 0;
            curDisPercent = 0;

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PayAction(int customerId, int locationId, int paymentId, int currencyId, DateTime dateTime, int creditLimitDay, int payMethodId, int bankPaymentId, int netAmount, int paymentPercent, int userId, int branchId, string voucherNumber)
        {
            int tranId = 0;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();

            DataTable dt = new DataTable();
            //Add columns  
            dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
            dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
            dt.Columns.Add(new DataColumn("SalePrice", typeof(double)));
            dt.Columns.Add(new DataColumn("DiscountAmt", typeof(double)));
            dt.Columns.Add(new DataColumn("TotalAmount", typeof(double)));
            dt.Columns.Add(new DataColumn("Variant", typeof(string)));
            dt.Columns.Add(new DataColumn("UnitID", typeof(int)));

            for (int i = 0; i < lstCurrentSale.Count(); i++)
            {
                if (lstCurrentSale[i].UnitID == null) lstCurrentSale[i].UnitID = 0;
                if (lstCurrentSale[i].UnitVariant == "-") lstCurrentSale[i].UnitVariant = "";
                //Add rows  
                dt.Rows.Add(lstCurrentSale[i].ProductID, lstCurrentSale[i].Quantity, lstCurrentSale[i].SalePrice, lstCurrentSale[i].DiscountAmt, lstCurrentSale[i].Amount, lstCurrentSale[i].UnitVariant, lstCurrentSale[i].UnitID);               
            }

            lstCurrentSale.Clear();

            SqlCommand cmd = new SqlCommand(procedure.PrcInsertSale, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Date", dateTime);
            cmd.Parameters.AddWithValue("@Voucher", voucherNumber);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            cmd.Parameters.AddWithValue("@LocationID", locationId);
            cmd.Parameters.AddWithValue("@PaymentID", paymentId);
            cmd.Parameters.AddWithValue("@CurrencyID", currencyId);
            cmd.Parameters.AddWithValue("@IsDelivery", isDelivery);
            cmd.Parameters.AddWithValue("@VoucherDis", Convert.ToDecimal(voucherDis));
            cmd.Parameters.AddWithValue("@AdvancedPayAmt", Convert.ToDecimal(paidAmount));
            cmd.Parameters.AddWithValue("@FOCAmt", 0);
            cmd.Parameters.AddWithValue("@TaxAmt", Convert.ToDecimal(taxAmount));
            cmd.Parameters.AddWithValue("@ChargesAmt", Convert.ToDecimal(chargesAmount));
            cmd.Parameters.AddWithValue("@TotalAmt", Convert.ToDecimal(totalAmount));
            cmd.Parameters.AddWithValue("@NetAmt", Convert.ToDecimal(netAmount));
            cmd.Parameters.AddWithValue("@CreditLimitDay", creditLimitDay);
            cmd.Parameters.AddWithValue("@BranchID", branchId);
            cmd.Parameters.AddWithValue("@VouDisPercent", curDisPercent);
            cmd.Parameters.AddWithValue("@VouDisAmount", Convert.ToDecimal(curDisAmount));
            cmd.Parameters.AddWithValue("@PayMethodID", payMethodId);
            cmd.Parameters.AddWithValue("@BankPaymentID", bankPaymentId);
            cmd.Parameters.AddWithValue("@PaymentPercent", paymentPercent);
            cmd.Parameters.AddWithValue("@temptbl", dt);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) tranId = Convert.ToInt32(reader["TranID"]);
            reader.Close();
            dataConnectorSQL.Close();

            curDisPercent = 0;
            curDisAmount = 0;

            var myResult = new
            {              
                TranID = tranId
            };
            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PreparePrintBillAction(int tranId, int locationId, DateTime dateTime, bool isMultiBranch, int branchId, string userName, string customerName)
        {           
            string vouNumAndSlipID = getVoucherData(locationId, isMultiBranch, branchId);
            string[] arr = vouNumAndSlipID.Split(',');
            string voucherNumber = arr[0];
            string slipId = arr[1];

            var myResult = new
            {
                SaleDetail = getSaleDetailByTranID(tranId),
                VouSettingModel = vouSettingModel,               
                VoucherNumber = voucherNumber,
                CustomerName = customerName,
                UserName = userName,
                SaleDateTime = dateTime,               
                SlipID = slipId
            };
            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PayEditAction(int customerId, int locationId, int paymentId, int currencyId, DateTime dateTime, int creditLimitDay, int netAmount,int userId)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();         

            DataTable dt = new DataTable();
            //Add columns  
            dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
            dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
            dt.Columns.Add(new DataColumn("SalePrice", typeof(double)));
            dt.Columns.Add(new DataColumn("DiscountAmt", typeof(double)));
            dt.Columns.Add(new DataColumn("TotalAmount", typeof(double)));
            dt.Columns.Add(new DataColumn("Variant", typeof(string)));
            dt.Columns.Add(new DataColumn("UnitID", typeof(int)));

            for (int i = 0; i < lstCurrentSale.Count(); i++)
            {
                if (lstCurrentSale[i].UnitID == null) lstCurrentSale[i].UnitID = 0;
                if (lstCurrentSale[i].UnitVariant == "-") lstCurrentSale[i].UnitVariant = "";
                //Add rows  
                dt.Rows.Add(lstCurrentSale[i].ProductID, lstCurrentSale[i].Quantity, lstCurrentSale[i].SalePrice, lstCurrentSale[i].DiscountAmt, lstCurrentSale[i].Amount, lstCurrentSale[i].UnitVariant, lstCurrentSale[i].UnitID);
            }

            //SqlCommand cmd = new SqlCommand(procedure.PrcInsertTempTranSale, (SqlConnection)Session["SQLConnection"]);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@temptbl", dt);
            //cmd.ExecuteNonQuery();

            lstCurrentSale.Clear();

            SqlCommand cmd = new SqlCommand(procedure.PrcUpdateSaleByTranID, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;          
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            cmd.Parameters.AddWithValue("@LocationID", locationId);
            cmd.Parameters.AddWithValue("@PaymentID", paymentId);
            cmd.Parameters.AddWithValue("@CurrencyID", currencyId);
            cmd.Parameters.AddWithValue("@IsDelivery", isDelivery);
            cmd.Parameters.AddWithValue("@VoucherDis", Convert.ToDecimal(voucherDis));
            cmd.Parameters.AddWithValue("@AdvancedPayAmt", Convert.ToDecimal(paidAmount));
            cmd.Parameters.AddWithValue("@FOCAmt", 0);
            cmd.Parameters.AddWithValue("@TaxAmt", Convert.ToDecimal(taxAmount));
            cmd.Parameters.AddWithValue("@ChargesAmt", Convert.ToDecimal(chargesAmount));
            cmd.Parameters.AddWithValue("@TotalAmt", Convert.ToDecimal(totalAmount));
            cmd.Parameters.AddWithValue("@NetAmt", Convert.ToDecimal(netAmount));
            cmd.Parameters.AddWithValue("@CreditLimitDay", creditLimitDay);
            cmd.Parameters.AddWithValue("@TranID", editTranID);
            cmd.Parameters.AddWithValue("@VouDisPercent", curDisPercent);
            cmd.Parameters.AddWithValue("@VouDisAmount", Convert.ToDecimal(curDisAmount));
            cmd.Parameters.AddWithValue("@temptbl", dt);
            cmd.ExecuteNonQuery();
           
            dataConnectorSQL.Close();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult VoucherDisAction(int disPercent, double disAmount)
        {
            curDisAmount = disAmount;
            curDisPercent = disPercent;
            bool disRangeOk = true;
            if (disPercent > 100)
            {
                disRangeOk = false;
                var result = new
                {
                    DisRangeOk = disRangeOk
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            voucherDis = 0;
            double disVouPercent = (totalAmount * disPercent) / 100;
            voucherDis = disVouPercent + disAmount;

            netAmount = (totalAmount + taxAmount + chargesAmount - voucherDis) - paidAmount;

            var myResult = new
            {
                VoucherDis = voucherDis,
                NetAmount = netAmount,
                DisRangeOk = disRangeOk
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AdvancedPayAction(double paidAmt)
        {
            paidAmount = 0;
            paidAmount = paidAmt;

            netAmount = (totalAmount + taxAmount + chargesAmount - voucherDis) - paidAmount;

            var myResult = new
            {
                NetAmount = netAmount
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeliveryAction(DateTime dateTime, string recipient, string phone, string address)
        {
            isDelivery = true;

            entity.Database.ExecuteSqlCommand("TRUNCATE TABLE Temp_Delivery");

            Temp_Delivery tempDelivery = new Temp_Delivery();
            tempDelivery.Date = dateTime;
            tempDelivery.Recipient = recipient;
            tempDelivery.Phone = phone;
            tempDelivery.Address = address;
            entity.Temp_Delivery.Add(tempDelivery);
            entity.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaleDetailAction(int tranId)
        {
            return Json(getSaleDetailByTranID(tranId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaleDeleteAction(int tranId)
        {
            using (var context = new InventoryDBEntities())
            {
                T_MasterSale ms = context.T_MasterSale.Where(x => x.TranID == tranId).Single<T_MasterSale>();
                context.T_MasterSale.Remove(ms);
                context.SaveChanges();

                context.T_TranSale.RemoveRange(context.T_TranSale.Where(x => x.TranID == tranId));
                context.SaveChanges();

                var delivery = context.T_Delivery.Where(x => x.TranID == tranId).FirstOrDefault();
                if (delivery != null)
                {
                    T_Delivery deli = context.T_Delivery.Where(x => x.TranID == tranId).Single<T_Delivery>();
                    context.T_Delivery.Remove(deli);
                    context.SaveChanges();
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaleList()
        {          
            getAllSaleList();
            return View(model);
        }

        [HttpGet]
        public JsonResult SearchAction(DateTime fromDate, DateTime toDate, string voucherNo)
        {
            model.SaleList = new List<SaleModels.SaleListModel>();
            SaleModels.SaleListModel saleListModel;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcSearchSaleByDateVoucher, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            cmd.Parameters.AddWithValue("@voucherNo", voucherNo);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                saleListModel = new SaleModels.SaleListModel();
                saleListModel.TranID = Convert.ToInt32(reader["TranID"]);
                saleListModel.Date = Convert.ToDateTime(reader["Date"]);
                saleListModel.Voucher = Convert.ToString(reader["Voucher"]);
                saleListModel.UserName = Convert.ToString(reader["UserName"]); 
                saleListModel.CustomerName = Convert.ToString(reader["CustomerName"]);
                saleListModel.LocShortName = Convert.ToString(reader["LocShortName"]);
                saleListModel.PayKeyword = Convert.ToString(reader["PayKeyword"]);
                saleListModel.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                saleListModel.NetAmt = Convert.ToDouble(reader["NetAmt"]); 
                model.SaleList.Add(saleListModel);             
            }
            reader.Close();
            dataConnectorSQL.Close();        

            return Json(model.SaleList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AllSearchAction(DateTime fromDate, DateTime toDate, string voucherNo, int customerId, int locationId, int paymentId, int currencyId)
        {
            model.SaleList = new List<SaleModels.SaleListModel>();
            SaleModels.SaleListModel saleListModel;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcSearchSaleByAllFilter, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fromDate", fromDate);
            cmd.Parameters.AddWithValue("@toDate", toDate);
            cmd.Parameters.AddWithValue("@voucherNo", voucherNo);
            cmd.Parameters.AddWithValue("@customerId", customerId);
            cmd.Parameters.AddWithValue("@locationId", locationId);
            cmd.Parameters.AddWithValue("@paymentId", paymentId);
            cmd.Parameters.AddWithValue("@currencyId", currencyId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                saleListModel = new SaleModels.SaleListModel();
                saleListModel.TranID = Convert.ToInt32(reader["TranID"]);
                saleListModel.Date = Convert.ToDateTime(reader["Date"]);
                saleListModel.Voucher = Convert.ToString(reader["Voucher"]);
                saleListModel.UserName = Convert.ToString(reader["UserName"]);
                saleListModel.CustomerName = Convert.ToString(reader["CustomerName"]);
                saleListModel.LocShortName = Convert.ToString(reader["LocShortName"]);
                saleListModel.PayKeyword = Convert.ToString(reader["PayKeyword"]);
                saleListModel.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                saleListModel.NetAmt = Convert.ToDouble(reader["NetAmt"]);
                model.SaleList.Add(saleListModel);
            }
            reader.Close();
            dataConnectorSQL.Close();        

            return Json(model.SaleList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FilterClickAction(bool isMultiBranch,int branchId,bool isMultiCurrency)
        {
            SaleModels.MainMenuModel mainMenuModel = new SaleModels.MainMenuModel();
            SaleModels.SubMenuModel subMenuModel = new SaleModels.SubMenuModel();
            SaleModels.CustomerModel customerModel = new SaleModels.CustomerModel();
            SaleModels.PaymentModel paymentModel = new SaleModels.PaymentModel();
            SaleModels.LocationModel locationModel = new SaleModels.LocationModel();
            SaleModels.CurrencyModel curModel = new SaleModels.CurrencyModel();
            List<SaleModels.MainMenuModel> lstMainMenu = new List<SaleModels.MainMenuModel>();
            List<SaleModels.SubMenuModel> lstSubMenu = new List<SaleModels.SubMenuModel>();
            List<SaleModels.CustomerModel> lstCustomer = new List<SaleModels.CustomerModel>();
            List<SaleModels.PaymentModel> lstPayment = new List<SaleModels.PaymentModel>();

            //foreach (var main in entity.S_MainMenu)
            //{
            //    mainMenuModel = new SaleModels.MainMenuModel();
            //    mainMenuModel.MainMenuID = main.MainMenuID;
            //    mainMenuModel.MainMenuName = main.MainMenuName;
            //    lstMainMenu.Add(mainMenuModel);
            //}
            //foreach (var sub in entity.S_SubMenu)
            //{
            //    subMenuModel = new SaleModels.SubMenuModel();
            //    subMenuModel.SubMenuID = sub.SubMenuID;
            //    subMenuModel.SubMenuName = sub.SubMenuName;
            //    lstSubMenu.Add(subMenuModel);
            //}
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();

            customerModel.CustomerID = 0;
            customerModel.CustomerName = "Choose Customer";
            lstCustomer.Add(customerModel);            
            SqlCommand cmd = new SqlCommand("Select CustomerID,CustomerName From S_Customer", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                customerModel = new SaleModels.CustomerModel();
                customerModel.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                customerModel.CustomerName = Convert.ToString(reader["CustomerName"]);
                lstCustomer.Add(customerModel);
            }
            reader.Close();                      

            paymentModel.PaymentID = 0;
            paymentModel.PayKeyword = "Choose Payment";
            lstPayment.Add(paymentModel);
            cmd = new SqlCommand("Select PaymentID,Keyword From Sys_Payment", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                paymentModel = new SaleModels.PaymentModel();
                paymentModel.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                paymentModel.PayKeyword = Convert.ToString(reader["Keyword"]);
                lstPayment.Add(paymentModel);
            }
            reader.Close();           

            dataConnectorSQL.Close();

            getLocation(isMultiBranch,branchId);
            locationModel.LocationID = 0;
            locationModel.LocShortName = "Choose Location";
            lstLocation.Insert(0, locationModel);

            if (isMultiCurrency)
            {
                getCurrency(isMultiCurrency);
                curModel.CurrencyID = 0;
                curModel.CurKeyword = "Choose Currency";
                lstCurrency.Insert(0, curModel);
            }

            var myResult = new
            {
                //LstMainMenu = lstMainMenu,
                //LstSubMenu = lstSubMenu,
                LstCustomer = lstCustomer,
                LstPayment = lstPayment,
                LstLocation = lstLocation,
                LstCurrency = lstCurrency
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult MainMenuSelectAction(int mainMenuId)
        {
            SaleModels.SubMenuModel subMenuModel = new SaleModels.SubMenuModel();
            List<SaleModels.SubMenuModel> lstSubMenu = new List<SaleModels.SubMenuModel>();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select SubMenuID,SubMenuName From S_SubMenu Where MainMenuID=" + mainMenuId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                subMenuModel = new SaleModels.SubMenuModel();
                subMenuModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                subMenuModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);
                lstSubMenu.Add(subMenuModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            return Json(lstSubMenu, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnitByProductId(int productId)
        {
            SaleModels.ProductModel model = new SaleModels.ProductModel();
            SaleModels.ProductUnitModel pUnitModel = new SaleModels.ProductUnitModel();

            getUnitByProduct(productId, model, pUnitModel);       
            
            return Json(model.ProductUnitList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetVariantByProductId(int productId)
        {
            SaleModels.ProductModel model = new SaleModels.ProductModel();
            SaleModels.ProductVariantModel pVariantModel = new SaleModels.ProductVariantModel();

            getVariantByProduct(productId, model, pVariantModel);
         
            return Json(model.ProductVariantList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ProductUnitSelectAction(int productId, int unitId)
        {
            double salePrice = 0; short? disPercent = 0;        

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select SalePrice,isnull(DisPercent,0) AS DisPercent From S_ProductUnit Where ProductID=" + productId +" And UnitID="+unitId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                salePrice = Convert.ToDouble(reader["SalePrice"]);
                disPercent = Convert.ToInt16(reader["DisPercent"]);
            }
            reader.Close();
            dataConnectorSQL.Close();

            var myResult = new
            {
                SalePrice = salePrice,
                DisPercent = disPercent
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchByCodeAction(string searchCode)
        {
            List<ProductModels.ProductModel> lstProduct = new List<ProductModels.ProductModel>();
            int productId = 0, searchOk;
            string productName = "", code = "";
            double salePrice = 0;
            short? disPercent = 0;
            bool isUnit = false, isVariant = false;
            var pros = (from pro in entity.S_Product where pro.Code == searchCode || pro.ProductName == searchCode select pro).ToList();
            if (pros.Count() == 1)
            {
                var product = entity.S_Product.Where(d => d.Code == searchCode || d.ProductName == searchCode).Single();
                productId = product.ProductID;
                productName = product.ProductName;
                code = product.Code;
                salePrice = Convert.ToDouble(product.SalePrice);
                disPercent = product.DisPercent;
                isUnit = product.IsUnit;
                isVariant = product.IsVariant;
                searchOk = 1;
            }
            else
            {
                searchOk = 0;
                ProductModels.ProductModel productModel = new ProductModels.ProductModel();
                if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand(procedure.PrcSearchProductByCodeName, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@keyword", searchCode);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    productModel = new ProductModels.ProductModel();
                    productModel.ProductID = Convert.ToInt32(reader["ProductID"]);
                    productModel.ProductName = Convert.ToString(reader["ProductName"]);
                    productModel.Code = Convert.ToString(reader["Code"]);
                    productModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                    productModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
                    productModel.IsUnit = Convert.ToBoolean(reader["IsUnit"]);
                    productModel.DisPercent = Convert.ToInt16(reader["DisPercent"]);
                    productModel.IsVariant = Convert.ToBoolean(reader["IsVariant"]);
                    productModel.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                    productModel.MainMenuName = Convert.ToString(reader["MainMenuName"]);
                    productModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);

                    lstProduct.Add(productModel);
                }
                reader.Close();
                dataConnectorSQL.Close();
            }

            var result = new
            {
                ProductID = productId,
                ProductName = productName,
                Code = code,
                SalePrice = salePrice,
                DisPercent = disPercent,
                IsUnit = isUnit,
                IsVariant = isVariant,
                SearchOk = searchOk,
                LstProduct = lstProduct
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchByFirstKeywordAction(string searchCode)
        {
            List<ProductModels.ProductModel> lstProduct = new List<ProductModels.ProductModel>();
            ProductModels.ProductModel productModel = new ProductModels.ProductModel();
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcSearchByFirstKeyword, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", searchCode);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productModel = new ProductModels.ProductModel();
                productModel.ProductID = Convert.ToInt32(reader["ProductID"]);
                productModel.ProductName = Convert.ToString(reader["ProductName"]);
                productModel.Code = Convert.ToString(reader["Code"]);
                productModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                productModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
                productModel.IsUnit = Convert.ToBoolean(reader["IsUnit"]);
                productModel.DisPercent = Convert.ToInt16(reader["DisPercent"]);
                productModel.IsVariant = Convert.ToBoolean(reader["IsVariant"]);
                productModel.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                productModel.MainMenuName = Convert.ToString(reader["MainMenuName"]);
                productModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);

                lstProduct.Add(productModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            var result = new
            {
                LstProduct = lstProduct
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchByLastKeywordAction(string searchCode)
        {
            List<ProductModels.ProductModel> lstProduct = new List<ProductModels.ProductModel>();
            ProductModels.ProductModel productModel = new ProductModels.ProductModel();
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcSearchByLastKeyword, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", searchCode);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productModel = new ProductModels.ProductModel();
                productModel.ProductID = Convert.ToInt32(reader["ProductID"]);
                productModel.ProductName = Convert.ToString(reader["ProductName"]);
                productModel.Code = Convert.ToString(reader["Code"]);
                productModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                productModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
                productModel.IsUnit = Convert.ToBoolean(reader["IsUnit"]);
                productModel.DisPercent = Convert.ToInt16(reader["DisPercent"]);
                productModel.IsVariant = Convert.ToBoolean(reader["IsVariant"]);
                productModel.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                productModel.MainMenuName = Convert.ToString(reader["MainMenuName"]);
                productModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);

                lstProduct.Add(productModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            var result = new
            {
                LstProduct = lstProduct
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchProductClickAction(int productId)
        {
            string productName = "", code = "";
            double salePrice = 0;
            short? disPercent = 0;
            bool isUnit = false, isVariant = false;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select ProductID,ProductName,Code,SalePrice,isnull(DisPercent,0) AS DisPercent,IsUnit,IsVariant From S_Product Where ProductID=" + productId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                productId = Convert.ToInt32(reader["ProductID"]);
                productName = Convert.ToString(reader["ProductName"]);
                code = Convert.ToString(reader["Code"]);
                salePrice = Convert.ToDouble(reader["SalePrice"]);
                disPercent = Convert.ToInt16(reader["DisPercent"]);
                isUnit = Convert.ToBoolean(reader["IsUnit"]);
                isVariant = Convert.ToBoolean(reader["IsVariant"]);
            }
            reader.Close();
            dataConnectorSQL.Close();

            var result = new
            {
                ProductID = productId,
                ProductName = productName,
                Code = code,
                SalePrice = salePrice,
                DisPercent = disPercent,
                IsUnit = isUnit,
                IsVariant = isVariant
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PaymentAction(bool isBankPayment)
        {
            bool isPayMethod = false;
            if (isBankPayment) isPayMethod = true;

            var result = new
            {
                IsPayMethod = isPayMethod
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void getSaleInfo(bool isMultiBranch, bool isMultiCurrency, bool isMultiUnit, int branchId, int userId, bool isBankPayment)
        {
            int? firstMainMenuId = 0;

            ViewBag.IsMultiCurrency = isMultiCurrency;
            ViewBag.IsMultiUnit = isMultiUnit;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();

            SqlCommand cmd = new SqlCommand("Select CustomerID,CustomerName From S_Customer", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.Customers.Add(new SelectListItem { Text = Convert.ToString(reader["CustomerName"]), Value = Convert.ToString(reader["CustomerID"]) });
            }
            reader.Close();

            ViewBag.DefaultLocationID = checkDefaultLocation(userId);
            if (isMultiBranch)
            {                
                cmd = new SqlCommand("Select LocationID,ShortName From S_Location Where BranchID=" + branchId, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Locations.Add(new SelectListItem { Text = Convert.ToString(reader["ShortName"]), Value = Convert.ToString(reader["LocationID"]) });
                }
                reader.Close();

                cmd = new SqlCommand("Select PreFormat,MidFormat,PostFormat From S_VoucherFormat Where BranchID=" + branchId, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                if (reader.Read()) ViewBag.VoucherNumber = Convert.ToString(reader["PreFormat"]) + Convert.ToString(reader["MidFormat"]) + Convert.ToString(reader["PostFormat"]);
                reader.Close();
            }
            else
            {             
                cmd = new SqlCommand("Select LocationID,ShortName From S_Location", (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Locations.Add(new SelectListItem { Text = Convert.ToString(reader["ShortName"]), Value = Convert.ToString(reader["LocationID"]) });
                }
                reader.Close();

                cmd = new SqlCommand("Select PreFormat,MidFormat,PostFormat From S_VoucherFormat", (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                if (reader.Read()) ViewBag.VoucherNumber = Convert.ToString(reader["PreFormat"]) + Convert.ToString(reader["MidFormat"]) + Convert.ToString(reader["PostFormat"]);
                reader.Close();
            }

            cmd = new SqlCommand("Select PaymentID,Keyword From Sys_Payment", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.Payments.Add(new SelectListItem { Text = Convert.ToString(reader["Keyword"]), Value = Convert.ToString(reader["PaymentID"]) });
            }
            reader.Close();

            if (isMultiCurrency)
            {
                cmd = new SqlCommand("Select CurrencyID,Keyword From Sys_Currency Order By IsDefault DESC", (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Currencies.Add(new SelectListItem { Text = Convert.ToString(reader["Keyword"]), Value = Convert.ToString(reader["CurrencyID"]) });
                }
                reader.Close();
            }

            cmd = new SqlCommand("Select MainMenuID,MainMenuName From S_MainMenu", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.MainMenus.Add(new SelectListItem { Text = Convert.ToString(reader["MainMenuName"]), Value = Convert.ToString(reader["MainMenuID"]) });
            }
            reader.Close();

            if (model.MainMenus.Count() != 0) firstMainMenuId = Convert.ToInt32(model.MainMenus[0].Value);

            if (firstMainMenuId.HasValue)
            {
                cmd = new SqlCommand("Select SubMenuID,SubMenuName From S_SubMenu Where MainMenuID=" + firstMainMenuId, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.SubMenus.Add(new SelectListItem { Text = Convert.ToString(reader["SubMenuName"]), Value = Convert.ToString(reader["SubMenuID"]) });
                }
                reader.Close();
            }
            dataConnectorSQL.Close();

            if (isBankPayment)
            {
                getPayMethod();
                getBankPayment();
            }
        }

        private void getAllSaleList()
        {
            model.SaleList = new List<SaleModels.SaleListModel>();
            SaleModels.SaleListModel saleListModel;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcGetMasterSaleList, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                saleListModel = new SaleModels.SaleListModel();
                saleListModel.TranID = Convert.ToInt32(reader["TranID"]);
                DateTime dateTime = Convert.ToDateTime(reader["Date"]);
                saleListModel.StrDate = dateTime.ToString("dd'/'MM'/'yyyy");
                saleListModel.Voucher = Convert.ToString(reader["Voucher"]);
                saleListModel.UserName = Convert.ToString(reader["UserName"]);
                saleListModel.CustomerName = Convert.ToString(reader["CustomerName"]);
                saleListModel.LocShortName = Convert.ToString(reader["LocShortName"]);
                saleListModel.PayKeyword = Convert.ToString(reader["PayKeyword"]);
                saleListModel.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                saleListModel.NetAmt = Convert.ToDouble(reader["NetAmt"]);
                model.SaleList.Add(saleListModel);
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private Object getSaleDetailByTranID(int tranId)
        {
            SaleModels.SaleListModel saleListModel = new SaleModels.SaleListModel();
            SaleModels.CurrentSaleModel tranModel = new SaleModels.CurrentSaleModel();
            SaleModels.DeliveryModel deliModel = new SaleModels.DeliveryModel();
            List<SaleModels.CurrentSaleModel> lstTran = new List<SaleModels.CurrentSaleModel>();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();

            SqlCommand cmd = new SqlCommand(procedure.PrcGetMasterSaleByTranID, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TranID", tranId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                saleListModel = new SaleModels.SaleListModel();
                saleListModel.Date = Convert.ToDateTime(reader["Date"]);
                saleListModel.Voucher = Convert.ToString(reader["Voucher"]);
                saleListModel.UserName = Convert.ToString(reader["UserName"]);
                saleListModel.CustomerName = Convert.ToString(reader["CustomerName"]);
                saleListModel.LocShortName = Convert.ToString(reader["LocShortName"]);
                saleListModel.PayKeyword = Convert.ToString(reader["PayKeyword"]);
                saleListModel.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                saleListModel.CreditLimitDay = Convert.ToInt32(reader["CreditLimitDay"]);
                saleListModel.NetAmt = Convert.ToDouble(reader["NetAmt"]);
                saleListModel.TotalAmt = Convert.ToDouble(reader["TotalAmt"]);
                saleListModel.TaxAmt = Convert.ToDouble(reader["TaxAmt"]);
                saleListModel.ChargesAmt = Convert.ToDouble(reader["ChargesAmt"]);
                saleListModel.FOCAmt = Convert.ToDouble(reader["FOCAmt"]);
                saleListModel.AdvancedPayAmt = Convert.ToDouble(reader["AdvancedPayAmt"]);
                saleListModel.VoucherDis = Convert.ToDouble(reader["VoucherDis"]);
                saleListModel.IsDelivery = Convert.ToBoolean(reader["IsDelivery"]);
                saleListModel.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);            

                reader.Close();

                if (saleListModel.IsDelivery == true)
                {
                    cmd = new SqlCommand("Select Date,Recipient,Phone,Address From T_Delivery Where TranID=" + tranId, (SqlConnection)Session["SQLConnection"]);
                    cmd.CommandType = CommandType.Text;
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        deliModel = new SaleModels.DeliveryModel();
                        deliModel.Date = Convert.ToDateTime(reader["Date"]);
                        deliModel.Recipient = Convert.ToString(reader["Recipient"]);
                        deliModel.Phone = Convert.ToString(reader["Phone"]);
                        deliModel.Address = Convert.ToString(reader["Address"]);
                    }
                    reader.Close();
                }
            }

            //foreach (var sale in entity.PrcGetTranSaleByTranID(tranId))
            //{
            //    tranModel = new SaleModels.CurrentSaleModel();
            //    tranModel.ProductName = sale.ProductName;
            //    if (sale.Keyword != null) tranModel.UnitVariant = sale.Keyword;
            //    else if (sale.Variant != null) tranModel.UnitVariant = sale.Variant;
            //    else tranModel.UnitVariant = "-";
            //    tranModel.Quantity = sale.Quantity;
            //    tranModel.SalePrice = Convert.ToDouble(sale.SalePrice);
            //    tranModel.DiscountAmt = Convert.ToDouble(sale.DiscountAmt);
            //    tranModel.Amount = Convert.ToDouble(sale.TotalAmount);
            //    lstTran.Add(tranModel);
            //}

            cmd = new SqlCommand(procedure.PrcGetTranSaleByTranID, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TranID", tranId);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tranModel = new SaleModels.CurrentSaleModel();             
                tranModel.ProductName = Convert.ToString(reader["ProductName"]);
                if (reader["Keyword"] != null) tranModel.UnitVariant = Convert.ToString(reader["Keyword"]);
                else if (reader["Variant"] != null) tranModel.UnitVariant = Convert.ToString(reader["Variant"]);
                else tranModel.UnitVariant = "-";
                tranModel.Quantity = Convert.ToInt32(reader["Quantity"]);
                tranModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
                tranModel.DiscountAmt = Convert.ToDouble(reader["DiscountAmt"]);
                tranModel.Amount = Convert.ToDouble(reader["TotalAmount"]);
                lstTran.Add(tranModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            var myResult = new
            {
                SaleListModel = saleListModel,
                DeliveryModel = deliModel,
                LstTran = lstTran
            };

            return myResult;
        }

        private void calcTaxAndChargesAmt(double totalAmount, bool isMultiBranch,int branchId)
        {
            int tax, charges;
            taxAmount = 0;
            chargesAmount = 0;

            if (isMultiBranch)
            {
                if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand("Select Tax,ServiceCharges From S_Branch Where BranchID=" + branchId, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    tax = Convert.ToInt32(reader["Tax"]);
                    charges = Convert.ToInt32(reader["ServiceCharges"]);
                    taxAmount = (totalAmount * tax) / 100;
                    chargesAmount = (totalAmount * charges) / 100;
                }
                reader.Close();
                dataConnectorSQL.Close();
            }
            else
            {
                if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand("Select Tax,ServiceCharges From S_CompanySetting", (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    tax = Convert.ToInt32(reader["Tax"]);
                    charges = Convert.ToInt32(reader["ServiceCharges"]);
                    taxAmount = (totalAmount * tax) / 100;
                    chargesAmount = (totalAmount * charges) / 100;
                }
                reader.Close();
                dataConnectorSQL.Close();
            }
        }

        private bool isUnitByProduct(int? productId)
        {
            bool result = false;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select IsUnit From S_Product Where ProductID=" + productId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                SaleModels.ProductModel model = new SaleModels.ProductModel();
                model.IsUnit = Convert.ToBoolean(reader["IsUnit"]);
                if (model.IsUnit == true) result = true;
                else result = false;
            }
            reader.Close();
            dataConnectorSQL.Close();

            return result;
        }

        private bool isVariantByProduct(int? productId)
        {
            bool result = false;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select IsVariant From S_Product Where ProductID=" + productId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                SaleModels.ProductModel model = new SaleModels.ProductModel();
                model.IsVariant = Convert.ToBoolean(reader["IsVariant"]);
                if (model.IsVariant != null)
                {
                    if (model.IsVariant == true) result = true;
                    else result = false;
                }
            }
            reader.Close();
            dataConnectorSQL.Close();

            return result;
        }

        private void getLocation(bool isMultiBranch,int branchId)
        {         
            SaleModels.LocationModel locationModel = new SaleModels.LocationModel();
            lstLocation = new List<SaleModels.LocationModel>();

            if (isMultiBranch)
            {
                if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand("Select LocationID,ShortName From S_Location Where BranchID=" + branchId, (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    locationModel = new SaleModels.LocationModel();
                    locationModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                    locationModel.LocShortName = Convert.ToString(reader["ShortName"]);
                    lstLocation.Add(locationModel);                   
                }
                reader.Close();
                dataConnectorSQL.Close();
            }
            else
            {
                if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
                SqlCommand cmd = new SqlCommand("Select LocationID,ShortName From S_Location", (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    locationModel = new SaleModels.LocationModel();
                    locationModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                    locationModel.LocShortName = Convert.ToString(reader["ShortName"]);
                    lstLocation.Add(locationModel);
                }
                reader.Close();
                dataConnectorSQL.Close();
            }
        }

        private void getCurrency(bool isMultiCurrency)
        {
            SaleModels.CurrencyModel curModel = new SaleModels.CurrencyModel();
            lstCurrency = new List<SaleModels.CurrencyModel>();

            if (isMultiCurrency)
            {
                SqlCommand cmd = new SqlCommand("Select CurrencyID,Keyword From Sys_Currency Order By IsDefault DESC", (SqlConnection)Session["SQLConnection"]);
                cmd.CommandType = CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    curModel = new SaleModels.CurrencyModel();
                    curModel.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                    curModel.CurKeyword = Convert.ToString(reader["Keyword"]);
                    lstCurrency.Add(curModel);
                }
                reader.Close();
                dataConnectorSQL.Close();
            }
        }

        private int checkDefaultLocation(int userId)
        {
            int defaultLocationId = 0;
            UserModels.UserModel userModel = new UserModels.UserModel();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select isnull(LocationID,0) AS LocationID From S_User WHERE UserID="+userId, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) defaultLocationId = Convert.ToInt32(reader["LocationID"]);
            reader.Close();
            dataConnectorSQL.Close();
         
            return defaultLocationId;
        }

        private string getVoucherData(int locationId, bool isMultiBranch, int branchId)
        {
            string voucherNumber = "";
            int slipId = 0;
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcGetVoucherData, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IsMultiBranch", isMultiBranch);
            cmd.Parameters.AddWithValue("@BranchID", branchId);
            cmd.Parameters.AddWithValue("@LocationID", locationId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                vouSettingModel = new SaleModels.VoucherSettingModel();
                if (reader["VoucherLogo"] != null)
                {
                    vouSettingModel.VoucherLogo = (byte[])(reader["VoucherLogo"]);
                    vouSettingModel.Base64Photo = Convert.ToBase64String((byte[])(reader["VoucherLogo"]));
                }
                vouSettingModel.HeaderName = Convert.ToString(reader["HeaderName"]);
                vouSettingModel.HeaderDesp = Convert.ToString(reader["HeaderDesp"]);
                vouSettingModel.HeaderPhone = Convert.ToString(reader["HeaderPhone"]);
                vouSettingModel.HeaderAddress = Convert.ToString(reader["HeaderAddress"]);
                vouSettingModel.OtherHeader1 = Convert.ToString(reader["OtherHeader1"]);
                vouSettingModel.OtherHeader2 = Convert.ToString(reader["OtherHeader2"]);
                vouSettingModel.FooterMessage1 = Convert.ToString(reader["FooterMessage1"]);
                vouSettingModel.FooterMessage2 = Convert.ToString(reader["FooterMessage2"]);
                vouSettingModel.FooterMessage3 = Convert.ToString(reader["FooterMessage3"]);
                voucherNumber = Convert.ToString(reader["VoucherNumber"]);
                slipId = Convert.ToInt32(reader["SlipID"]);
            }
            reader.Close();
            dataConnectorSQL.Close();
            return voucherNumber + "," + slipId;                
        }

        private void getPayMethod()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select PayMethodID,PayMethodName From Sys_PayMethod", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.PayMethods.Add(new SelectListItem { Text = Convert.ToString(reader["PayMethodName"]), Value = Convert.ToString(reader["PayMethodID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void getBankPayment()
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select BankPaymentID,BankPaymentName From S_BankPayment", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.BankPayments.Add(new SelectListItem { Text = Convert.ToString(reader["BankPaymentName"]), Value = Convert.ToString(reader["BankPaymentID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void getUnitByProduct(int? productId,SaleModels.ProductModel model,SaleModels.ProductUnitModel pUnitModel)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcGetUnitByProductID, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pUnitModel = new SaleModels.ProductUnitModel();
                pUnitModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                pUnitModel.Keyword = Convert.ToString(reader["Keyword"]);
                pUnitModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
                if (reader["DisPercent"] != null) pUnitModel.DisPercent = Convert.ToInt16(reader["DisPercent"]);
                else pUnitModel.DisPercent = 0;

                model.ProductUnitList.Add(pUnitModel);
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void getVariantByProduct(int? productId, SaleModels.ProductModel model, SaleModels.ProductVariantModel pVariantModel)
        {
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcGetVariantByProductID, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pVariantModel = new SaleModels.ProductVariantModel();
                pVariantModel.ID = Convert.ToInt32(reader["ID"]);
                pVariantModel.Variant = Convert.ToString(reader["Variant"]);

                model.ProductVariantList.Add(pVariantModel);
            }
            reader.Close();
            dataConnectorSQL.Close();
        }
    }
}
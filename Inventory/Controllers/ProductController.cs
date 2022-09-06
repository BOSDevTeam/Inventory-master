using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using System.Data.Entity.Core.Objects;
using System.IO;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class ProductController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        Procedure procedure = new Procedure();
        InventoryDBEntities Entities = new InventoryDBEntities();
        ProductModels.ProductModel model = new ProductModels.ProductModel();
        static List<ProductModels.ProductModel> lstProductList = new List<ProductModels.ProductModel>();
        static int editProductID;
        static List<ProductModels.ProductUnit> lstProductUnitList = new List<ProductModels.ProductUnit>();
        static List<ProductModels.ProductVariant> lstProductVariantList = new List<ProductModels.ProductVariant>();
        DataTable dtUnit = new DataTable();
        Setting setting = new Setting();

        public ActionResult ProductEntry(int productId,bool isMultiUnit, bool isDifProductByBranch, bool isProductVariant, bool isBarcode, bool isQRcode, bool isProductPhoto)
        {                     
            Entities.Database.ExecuteSqlCommand("TRUNCATE TABLE Temp_ProductVariant");
            ViewBag.IsMultiUnit = isMultiUnit;
            ViewBag.IsDifProductByBranch = isDifProductByBranch;
            ViewBag.IsBarcode = isBarcode;
            ViewBag.IsQRcode = isQRcode;
            ViewBag.IsProductVariant = isProductVariant;
            ViewBag.IsProductPhoto = isProductPhoto;

            getMainMenu();
            if (model.MainMenus.Count() != 0) getSubMenuByMainMenu(Convert.ToInt32(model.MainMenus[0].Value));
            if (isMultiUnit) getUnit();
            clearSessionPhoto();
            createUnitDataTable();

            if (productId != 0)
            {
                Session["IsEdit"] = 1;
                editProductID = productId;
                var editProduct = lstProductList.Where(c => c.ProductID == productId);
                foreach (var e in editProduct)
                {
                    Session["EditProductName"] = e.ProductName;
                    Session["EditSortCode"] = e.SortCode;
                    Session["EditDescription"] = e.Description;
                    Session["EditCode"] = e.Code;
                    Session["EditPurchasePrice"] = e.PurchasePrice;
                    Session["EditSalePrice"] = e.SalePrice;
                    Session["EditWholeSalePrice"] = e.WholeSalePrice;
                    Session["EditAlertQty"] = e.AlertQty;
                    Session["EditDisPercent"] = e.DisPercent;
                    Session["EditMainMenuID"] = e.MainMenuID;
                    Session["EditSubMenuID"] = e.SubMenuID;
                    if (e.IsStock == true) Session["EditIsStockVal"] = 1;
                    else Session["EditIsStockVal"] = 0;
                    if (e.Photo != null)
                    {
                        ViewBag.Photo = true;
                        model.Photo = e.Photo;
                        model.Base64Photo = Convert.ToBase64String(e.Photo);
                    }
                    else
                    {
                        ViewBag.Photo = false;
                    }
                    
                    ViewData["EditLstProductUnit"] = getProductUnitByProductID(productId);                 
                    ViewData["EditLstProductVariant"] = getProductVariantByProductID(productId);

                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

        public ActionResult ProductList(bool isMultiUnit, bool isProductPhoto, bool isProductVariant, int p)
        {
            ViewBag.IsMultiUnit = isMultiUnit;
            ViewBag.IsProductVariant = isProductVariant;
            ViewBag.IsProductPhoto = isProductPhoto;
            getMainMenuDefaultInclude();
            getSubMenuDefaultInclude();

            getProductList(p, "", 0, 0, isProductPhoto);
            //ProductModels.ProductModel productModel = new ProductModels.ProductModel();
            //model.LstProduct = new List<ProductModels.ProductModel>();
            //lstProductList = new List<ProductModels.ProductModel>();
            //List<ProductModels.ProductModel> tempList = new List<ProductModels.ProductModel>();

            //if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            //SqlCommand cmd = new SqlCommand(procedure.PrcGetProduct, (SqlConnection)Session["SQLConnection"]);
            //cmd.CommandType = CommandType.StoredProcedure;

            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    productModel = new ProductModels.ProductModel();
            //    productModel.ProductID = Convert.ToInt32(reader["ProductID"]);
            //    productModel.ProductName = Convert.ToString(reader["ProductName"]);
            //    productModel.Code = Convert.ToString(reader["Code"]);
            //    productModel.SortCode = Convert.ToInt32(reader["SortCode"]);
            //    productModel.Description = Convert.ToString(reader["Description"]);
            //    productModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
            //    productModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);
            //    productModel.PurchasePrice = Convert.ToDouble(reader["PurPrice"]);
            //    productModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
            //    productModel.WholeSalePrice = Convert.ToDouble(reader["WholeSalePrice"]);
            //    productModel.IsUnit = Convert.ToBoolean(reader["IsUnit"]);
            //    productModel.IsStock = Convert.ToBoolean(reader["IsStock"]);
            //    productModel.AlertQty = Convert.ToInt32(reader["AlertQty"]);
            //    productModel.DisPercent = Convert.ToInt32(reader["DisPercent"]);
            //    productModel.IsVariant = Convert.ToBoolean(reader["IsVariant"]);
            //    productModel.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
            //    productModel.MainMenuName = Convert.ToString(reader["MainMenuName"]);
            //    if (isProductPhoto)
            //    {
            //        if (reader["Photo"].ToString().Length != 0)
            //        {
            //            productModel.Photo = (byte[])(reader["Photo"]);
            //            productModel.Base64Photo = Convert.ToBase64String(productModel.Photo);
            //        }
            //    }
            //    tempList.Add(productModel);
            //}
            //reader.Close();
            //dataConnectorSQL.Close();

            //if (tempList.Count > setting.pageSize)
            //{
            //    model.TotalPageNum = tempList.Count / setting.pageSize;
            //    setting.left = tempList.Count % setting.pageSize;
            //    if (setting.left != 0) model.TotalPageNum += 1;

            //    int i = p * setting.pageSize;
            //    int j = (i - setting.pageSize) + 1;
            //    int start = j;
            //    int end = i;
            //    setting.startRowIndex = start - 1;
            //    setting.endRowIndex = end - 1;
            //    Session["PageNumber"] = "Page : " + p;
            //}
            //else
            //{
            //    setting.startRowIndex = 0;
            //    setting.endRowIndex = tempList.Count - 1;
            //    Session["PageNumber"] = "";
            //}

            //for (int page = setting.startRowIndex; page < tempList.Count; page ++)
            //{
            //    if (page > setting.endRowIndex) break;

            //    productModel = new ProductModels.ProductModel();
            //    productModel.ProductID = tempList[page].ProductID;
            //    productModel.ProductName = tempList[page].ProductName;
            //    productModel.Code = tempList[page].Code;
            //    productModel.SortCode = tempList[page].SortCode;
            //    productModel.Description = tempList[page].Description;
            //    productModel.SubMenuID = tempList[page].SubMenuID;
            //    productModel.SubMenuName = tempList[page].SubMenuName;
            //    productModel.PurchasePrice = tempList[page].PurchasePrice;
            //    productModel.SalePrice = tempList[page].SalePrice;
            //    productModel.WholeSalePrice = tempList[page].WholeSalePrice;
            //    productModel.IsUnit = tempList[page].IsUnit;
            //    productModel.IsStock = tempList[page].IsStock;
            //    productModel.AlertQty = tempList[page].AlertQty;
            //    productModel.DisPercent = tempList[page].DisPercent;
            //    productModel.IsVariant = tempList[page].IsVariant;
            //    productModel.MainMenuID = tempList[page].MainMenuID;
            //    productModel.MainMenuName = tempList[page].MainMenuName;
            //    if (isProductPhoto)
            //    {
            //        if (tempList[page].Photo.ToString().Length != 0)
            //        {
            //            productModel.Photo = tempList[page].Photo;
            //            productModel.Base64Photo = Convert.ToBase64String(productModel.Photo);
            //        }
            //    }

            //    model.LstProduct.Add(productModel);
            //    lstProductList.Add(productModel);
            //}

            return View(model);
        }

        [HttpGet]
        public JsonResult MainMenuSelectAction(int mainMenuId)
        {
            SubMenuModels.SubMenuModel subMenuModel = new SubMenuModels.SubMenuModel();
            List<SubMenuModels.SubMenuModel> lstSubMenu = new List<SubMenuModels.SubMenuModel>();            

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select SubMenuID,SubMenuName From S_SubMenu Where MainMenuID=" + mainMenuId + " Order By SortCode", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                subMenuModel = new SubMenuModels.SubMenuModel();
                subMenuModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                subMenuModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);
                lstSubMenu.Add(subMenuModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            return Json(lstSubMenu, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SaveAction(string productName, string code, int sortCode, string description, double purchasePrice, double salePrice, double wholeSalePrice, int alertQty, short disPercent, bool isStock, int subMenuID, bool isProductPhoto)
        {
            string message = "";
            bool isSuccess = false;
            int productId = 0;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcInsertProduct, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.Parameters.AddWithValue("@SortCode", sortCode);
            cmd.Parameters.AddWithValue("@ProductName", productName);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@PurPrice", purchasePrice);
            cmd.Parameters.AddWithValue("@SalePrice", salePrice);
            cmd.Parameters.AddWithValue("@WholeSalePrice", wholeSalePrice);
            cmd.Parameters.AddWithValue("@AlertQty", alertQty);
            cmd.Parameters.AddWithValue("@DisPercent", disPercent);
            cmd.Parameters.AddWithValue("@IsStock", isStock);
            cmd.Parameters.AddWithValue("@SubMenuID", subMenuID);
            cmd.Parameters.AddWithValue("@IsProductPhoto", isProductPhoto);
            if (Session["DTUnit"] != null) cmd.Parameters.AddWithValue("@tempunit", (DataTable)Session["DTUnit"]);

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isSuccess = Convert.ToBoolean(reader["IsSuccess"]);
                message = Convert.ToString(reader["Message"]);
                productId = Convert.ToInt32(reader["ProductID"]);
            }
            reader.Close();
            dataConnectorSQL.Close();

            if (isProductPhoto) savePhoto(productId);
            createUnitDataTable();
                          
            var Result = new
            {
                MESSAGE = message,
                ISSUCCESS = isSuccess
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ViewAction(int productId)
        {
            string productName = "", code = "", description = "", mainMenu = "", subMenu = "", isStock = "", base64Photo = "";
            int? sortCode = 0, alertQty = 0, discount = 0;
            double purPirce = 0, salePrice = 0, wholeSalePrice = 0;

            createUnitDataTable();
            var viewProduct = lstProductList.Where(c => c.ProductID == productId);
            foreach (var e in viewProduct)
            {
                productName = e.ProductName;
                code = e.Code;
                description = e.Description;
                mainMenu = e.MainMenuName;
                subMenu = e.SubMenuName;
                if (e.IsStock) isStock = "Stock";
                else isStock = "Non Stock";
                sortCode = e.SortCode;
                alertQty = e.AlertQty;
                discount = e.DisPercent;
                purPirce = e.PurchasePrice;
                salePrice = e.SalePrice;
                wholeSalePrice = e.WholeSalePrice;
                base64Photo = e.Base64Photo;
                break;
            }        

            var myResult = new
            {
                ProductName = productName,
                Code = code,
                Description = description,
                MainMenu = mainMenu,
                SubMenu = subMenu,
                SortCode = sortCode,
                IsStock = isStock,
                AlertQty = alertQty,
                Discount = discount,
                PurPrice = purPirce,
                SalePrice = salePrice,
                WholeSalePrice = wholeSalePrice,
                Base64Photo = base64Photo,
                LstProductUnitList = getProductUnitByProductID(productId),
                LstProductVariantList = getProductVariantByProductID(productId)
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string productName, string code, int sortCode, string description, double purchasePrice, double salePrice, double wholeSalePrice, int alertQty, short disPercent, bool isStock, int subMenuID, bool isProductPhoto)
        {
            string message = "";
            bool isSuccess = false;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcUpdateProduct, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", editProductID);
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.Parameters.AddWithValue("@SortCode", sortCode);
            cmd.Parameters.AddWithValue("@ProductName", productName);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@PurPrice", purchasePrice);
            cmd.Parameters.AddWithValue("@SalePrice", salePrice);
            cmd.Parameters.AddWithValue("@WholeSalePrice", wholeSalePrice);
            cmd.Parameters.AddWithValue("@AlertQty", alertQty);
            cmd.Parameters.AddWithValue("@DisPercent", disPercent);
            cmd.Parameters.AddWithValue("@IsStock", isStock);
            cmd.Parameters.AddWithValue("@SubMenuID", subMenuID);
            cmd.Parameters.AddWithValue("@IsProductPhoto", isProductPhoto);
            if (Session["DTUnit"] != null) cmd.Parameters.AddWithValue("@tempunit", (DataTable)Session["DTUnit"]);

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isSuccess = Convert.ToBoolean(reader["IsSuccess"]);
                message = Convert.ToString(reader["Message"]);
            }
            reader.Close();
            dataConnectorSQL.Close();

            if (isProductPhoto) savePhoto(editProductID);

            var Result = new
            {
                MESSAGE = message,
                ISSUCCESS = isSuccess
            };

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(string keyword, int mainMenuId, int? subMenuId, bool isProductPhoto,int p)
        {
            ProductModels.ProductModel productModel = new ProductModels.ProductModel();
            model.LstProduct = new List<ProductModels.ProductModel>();
            lstProductList = new List<ProductModels.ProductModel>();
            //var result = new JsonResult();

            getProductList(p, keyword, mainMenuId, subMenuId, isProductPhoto);

            //if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            //SqlCommand cmd = new SqlCommand(procedure.PrcSearchProduct, (SqlConnection)Session["SQLConnection"]);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@keyword", keyword);
            //cmd.Parameters.AddWithValue("@MainMenuID", mainMenuId);
            //cmd.Parameters.AddWithValue("@SubMenuID", subMenuId);

            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    productModel = new ProductModels.ProductModel();
            //    productModel.ProductID = Convert.ToInt32(reader["ProductID"]);
            //    productModel.ProductName = Convert.ToString(reader["ProductName"]);
            //    productModel.Code = Convert.ToString(reader["Code"]);
            //    productModel.SortCode = Convert.ToInt32(reader["SortCode"]);
            //    productModel.Description = Convert.ToString(reader["Description"]);
            //    productModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
            //    productModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);
            //    productModel.PurchasePrice = Convert.ToDouble(reader["PurPrice"]); 
            //    productModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
            //    productModel.WholeSalePrice = Convert.ToDouble(reader["WholeSalePrice"]);
            //    productModel.IsUnit = Convert.ToBoolean(reader["IsUnit"]);
            //    productModel.IsStock = Convert.ToBoolean(reader["IsStock"]);
            //    productModel.AlertQty = Convert.ToInt32(reader["AlertQty"]); 
            //    productModel.DisPercent = Convert.ToInt32(reader["DisPercent"]);             
            //    productModel.IsVariant = Convert.ToBoolean(reader["IsVariant"]);
            //    productModel.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
            //    productModel.MainMenuName = Convert.ToString(reader["MainMenuName"]);
            //    if (isProductPhoto)
            //    {
            //        if (reader["Photo"].ToString().Length != 0)
            //        {
            //            productModel.Photo = (byte[])(reader["Photo"]); 
            //            productModel.Base64Photo = Convert.ToBase64String(productModel.Photo);
            //        }
            //    }

            //    model.LstProduct.Add(productModel);
            //    lstProductList.Add(productModel);
            //}
            //reader.Close();
            //dataConnectorSQL.Close();

            var myResult = new
            {
                LstProduct = model.LstProduct,
                TotalPageNum = model.TotalPageNum
            };

            //result = Json(model.LstProduct, JsonRequestBehavior.AllowGet);
            //result.MaxJsonLength = int.MaxValue;

            //return result;
            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int productId)
        {            
            string message = "";
            bool isSuccess = false;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcDeleteProduct, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isSuccess = Convert.ToBoolean(reader["IsSuccess"]);
                message = Convert.ToString(reader["Message"]);
            }
            reader.Close();
            dataConnectorSQL.Close();

            var Result = new
            {
                MESSAGE = message,
                ISSUCCESS = isSuccess
            };

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadFile()
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                Session["PhotoFile"] = file;  
            }
            return Json("");
        }

        [HttpGet]
        public JsonResult RefreshAction()
        {
            clearSessionPhoto();
            createUnitDataTable();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UnitSaveAction(int unitId, double unitPurPrice, double unitSalePrice, short unitDiscount, string unitDescription,string unitKeyword)
        {
            string message = "";
            bool isSuccess = false, isAlreadyExist = false;
            ProductModels.ProductUnit productUnitModel = new ProductModels.ProductUnit();
            lstProductUnitList = new List<ProductModels.ProductUnit>();

            if (Session["DTUnit"] != null) dtUnit = (DataTable)Session["DTUnit"];
            foreach (DataRow row in dtUnit.Rows)
            {
                if(Convert.ToInt32(row["UnitID"]) == unitId)
                {
                    message = "Already existed with this Unit!";
                    isSuccess = false;
                    isAlreadyExist = true;
                }
            }
            if (!isAlreadyExist)
            {
                dtUnit.Rows.Add(unitId, unitKeyword, unitPurPrice, unitSalePrice, unitDiscount, unitDescription);
                isSuccess = true;
            }

            Session["DTUnit"] = dtUnit;

            for(int i = 0; i < dtUnit.Rows.Count; i++)
            {
                productUnitModel = new ProductModels.ProductUnit();            
                productUnitModel.UnitID = Convert.ToInt32(dtUnit.Rows[i]["UnitID"]);
                productUnitModel.UnitKeyword = Convert.ToString(dtUnit.Rows[i]["UnitKeyword"]);
                productUnitModel.SalePrice = Convert.ToDouble(dtUnit.Rows[i]["SalePrice"]);
                productUnitModel.PurchasePrice = Convert.ToDouble(dtUnit.Rows[i]["PurPrice"]);
                productUnitModel.DiscountPercent = Convert.ToInt16(dtUnit.Rows[i]["DisPercent"]);
                productUnitModel.Description = Convert.ToString(dtUnit.Rows[i]["Description"]);
                lstProductUnitList.Add(productUnitModel);
            }            

            var Result = new
            {
                MESSAGE = message,
                ISSUCCESS = isSuccess,
                LstProductUnitList = lstProductUnitList
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UnitEditPrepareAction(int Id)
        {
            ProductModels.ProductUnit editProductUnit = lstProductUnitList.Where(x => x.UnitID == Id).Single<ProductModels.ProductUnit>();
            int index = lstProductUnitList.IndexOf(editProductUnit);
            string unitKeyword = lstProductUnitList[index].UnitKeyword;
            double PurPrice = lstProductUnitList[index].PurchasePrice;
            double SalePrice = lstProductUnitList[index].SalePrice;
            short? Discount = lstProductUnitList[index].DiscountPercent;
            string Description = lstProductUnitList[index].Description;
            int unitId = lstProductUnitList[index].UnitID;

            var Result = new
            {
                UnitKeyword = unitKeyword,
                PurPrice = PurPrice,
                SalePrice = SalePrice,
                Discount = Discount,
                Description = Description,
                UnitID = unitId
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UnitEditAction(int Id, int unitId, double unitPurPrice, double unitSalePrice, short unitDiscount, string unitDescription)
        {
            ProductModels.ProductUnit productUnitModel = new ProductModels.ProductUnit();
            lstProductUnitList = new List<ProductModels.ProductUnit>();

            if (Session["DTUnit"] != null) dtUnit = (DataTable)Session["DTUnit"];

            foreach (DataRow row in dtUnit.Rows)
            {
                if (Convert.ToInt32(row["UnitID"]) == unitId)
                {
                    row["PurPrice"] = unitPurPrice;
                    row["SalePrice"] = unitSalePrice;
                    row["DisPercent"] = unitDiscount;
                    row["Description"] = unitDescription;                  
                }
            }            

            Session["DTUnit"] = dtUnit;

            for (int i = 0; i < dtUnit.Rows.Count; i++)
            {
                productUnitModel = new ProductModels.ProductUnit();
                productUnitModel.UnitID = Convert.ToInt32(dtUnit.Rows[i]["UnitID"]);
                productUnitModel.UnitKeyword = Convert.ToString(dtUnit.Rows[i]["UnitKeyword"]);
                productUnitModel.SalePrice = Convert.ToDouble(dtUnit.Rows[i]["SalePrice"]);
                productUnitModel.PurchasePrice = Convert.ToDouble(dtUnit.Rows[i]["PurPrice"]);
                productUnitModel.DiscountPercent = Convert.ToInt16(dtUnit.Rows[i]["DisPercent"]);
                productUnitModel.Description = Convert.ToString(dtUnit.Rows[i]["Description"]);
                lstProductUnitList.Add(productUnitModel);
            }

            var Result = new
            {
                LstProductUnitList = lstProductUnitList
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteUnitAction(int Id)
        {
            ProductModels.ProductUnit productUnitModel = new ProductModels.ProductUnit();
            lstProductUnitList = new List<ProductModels.ProductUnit>();

            if (Session["DTUnit"] != null) dtUnit = (DataTable)Session["DTUnit"];

            foreach (DataRow row in dtUnit.Rows)
            {
                if (Convert.ToInt32(row["UnitID"]) == Id)
                {
                    row.Delete();
                    break;
                }                 
            }

            Session["DTUnit"] = dtUnit;

            for (int i = 0; i < dtUnit.Rows.Count; i++)
            {
                productUnitModel = new ProductModels.ProductUnit();
                productUnitModel.UnitID = Convert.ToInt32(dtUnit.Rows[i]["UnitID"]);
                productUnitModel.UnitKeyword = Convert.ToString(dtUnit.Rows[i]["UnitKeyword"]);
                productUnitModel.SalePrice = Convert.ToDouble(dtUnit.Rows[i]["SalePrice"]);
                productUnitModel.PurchasePrice = Convert.ToDouble(dtUnit.Rows[i]["PurPrice"]);
                productUnitModel.DiscountPercent = Convert.ToInt16(dtUnit.Rows[i]["DisPercent"]);
                productUnitModel.Description = Convert.ToString(dtUnit.Rows[i]["Description"]);
                lstProductUnitList.Add(productUnitModel);
            }          

            var Result = new
            {
                LstProductUnitList = lstProductUnitList
            };

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult VariantSaveAction(string variant)
        {
            Temp_ProductVariant table = new Temp_ProductVariant();
            table.Variant = variant;
            Entities.Temp_ProductVariant.Add(table);
            Entities.SaveChanges();

            GetTempProductVariant();

            var Result = new
            {
                LstProductVariantList = lstProductVariantList
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteVariantAction(int Id)
        {
            Temp_ProductVariant var = Entities.Temp_ProductVariant.Where(x => x.ID == Id).Single<Temp_ProductVariant>();
            Entities.Temp_ProductVariant.Remove(var);
            Entities.SaveChanges();

            GetTempProductVariant();

            var Result = new
            {
                LstProductVariantList = lstProductVariantList
            };

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        private List<ProductModels.ProductUnit> getProductUnitByProductID(int productId)
        {
            ProductModels.ProductUnit productUnitModel = new ProductModels.ProductUnit();
            lstProductUnitList = new List<ProductModels.ProductUnit>();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcGetProductUnitByProductID, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productUnitModel = new ProductModels.ProductUnit();
                productUnitModel.ID = Convert.ToInt32(reader["ID"]);
                productUnitModel.UnitID = Convert.ToInt32(reader["UnitID"]);
                productUnitModel.UnitKeyword = Convert.ToString(reader["Keyword"]);
                productUnitModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
                productUnitModel.PurchasePrice = Convert.ToDouble(reader["PurPrice"]);               
                productUnitModel.DiscountPercent = Convert.ToInt16(reader["DisPercent"]);
                productUnitModel.Description = Convert.ToString(reader["Description"]);                
                lstProductUnitList.Add(productUnitModel);

                if (dtUnit != null) dtUnit.Rows.Add(Convert.ToInt32(reader["UnitID"]), Convert.ToString(reader["Keyword"]), Convert.ToDouble(reader["PurPrice"]), Convert.ToDouble(reader["SalePrice"]), Convert.ToInt16(reader["DisPercent"]), Convert.ToString(reader["Description"]));
            }
            reader.Close();
            dataConnectorSQL.Close();
            if (dtUnit != null) Session["DTUnit"] = dtUnit;

            return lstProductUnitList;           
        }

        private List<ProductModels.ProductVariant> getProductVariantByProductID(int productId)
        {
            ProductModels.ProductVariant productVariantModel = new ProductModels.ProductVariant();
            lstProductVariantList = new List<ProductModels.ProductVariant>();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcGetProductVariantByProductID, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productVariantModel = new ProductModels.ProductVariant();
                productVariantModel.ID = Convert.ToInt32(reader["ID"]);
                productVariantModel.Variant = Convert.ToString(reader["Variant"]);
                lstProductVariantList.Add(productVariantModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            return lstProductVariantList;         
        }

        private void GetTempProductVariant()
        {
            ProductModels.ProductVariant productVariantModel = new ProductModels.ProductVariant();
            lstProductVariantList = new List<ProductModels.ProductVariant>();

            foreach (var temp in Entities.Temp_ProductVariant)
            {
                productVariantModel = new ProductModels.ProductVariant();
                productVariantModel.ID = temp.ID;
                productVariantModel.Variant = temp.Variant;

                lstProductVariantList.Add(productVariantModel);
            }
        }

        private void getMainMenu()
        {         
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select MainMenuID,MainMenuName From S_MainMenu Order By SortCode", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.MainMenus.Add(new SelectListItem { Text = Convert.ToString(reader["MainMenuName"]), Value = Convert.ToString(reader["MainMenuID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void getMainMenuDefaultInclude()
        {           
            model.MainMenus.Add(new SelectListItem { Text = "Main Menu", Value = "0" });
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select MainMenuID,MainMenuName From S_MainMenu Order By SortCode", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.MainMenus.Add(new SelectListItem { Text = Convert.ToString(reader["MainMenuName"]), Value = Convert.ToString(reader["MainMenuID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void getSubMenuByMainMenu(int mainMenuId)
        {           
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select SubMenuID,SubMenuName From S_SubMenu Where MainMenuID="+ mainMenuId +" Order By SortCode", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.SubMenus.Add(new SelectListItem { Text = Convert.ToString(reader["SubMenuName"]), Value = Convert.ToString(reader["SubMenuID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void getSubMenuDefaultInclude()
        {            
            model.SubMenus.Add(new SelectListItem { Text = "Sub Menu", Value = "0" });
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select SubMenuID,SubMenuName From S_SubMenu Order By SortCode", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.SubMenus.Add(new SelectListItem { Text = Convert.ToString(reader["SubMenuName"]), Value = Convert.ToString(reader["SubMenuID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void getUnit()
        {            
            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand("Select UnitID,Keyword From S_Unit Order By ULID", (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                model.Units.Add(new SelectListItem { Text = Convert.ToString(reader["Keyword"]), Value = Convert.ToString(reader["UnitID"]) });
            }
            reader.Close();
            dataConnectorSQL.Close();
        }

        private void clearSessionPhoto()
        {
            Session["PhotoFile"] = null;
        }

        private void savePhoto(int productId)
        {
            if (Session["PhotoFile"] != null)
            {
                var result = Entities.S_Product.Where(x => x.ProductID == productId).SingleOrDefault();
                if (result != null)
                {
                    HttpPostedFileBase file = (HttpPostedFileBase)Session["PhotoFile"];
                    if (file != null)
                    {
                        if (file.ContentLength > 0)
                        {
                            result.Photo = new byte[file.ContentLength];
                            file.InputStream.Read(result.Photo, 0, file.ContentLength);

                            Entities.SaveChanges();
                        }
                    }                    
                }
                clearSessionPhoto();
            }
        }

        private void createUnitDataTable()
        {
            dtUnit = new DataTable();
            dtUnit.Columns.Add(new DataColumn("UnitID", typeof(int)));
            dtUnit.Columns.Add(new DataColumn("UnitKeyword", typeof(string)));
            dtUnit.Columns.Add(new DataColumn("PurPrice", typeof(double)));
            dtUnit.Columns.Add(new DataColumn("SalePrice", typeof(double)));
            dtUnit.Columns.Add(new DataColumn("DisPercent", typeof(short)));
            dtUnit.Columns.Add(new DataColumn("Description", typeof(string)));
            Session["DTUnit"] = dtUnit;
        }

        private void getProductList(int p,string keyword, int mainMenuId, int? subMenuId, bool isProductPhoto)
        {
            //ViewBag.IsMultiUnit = isMultiUnit;
            //ViewBag.IsProductVariant = isProductVariant;
            //ViewBag.IsProductPhoto = isProductPhoto;
            //getMainMenuDefaultInclude();
            //getSubMenuDefaultInclude();
            ProductModels.ProductModel productModel = new ProductModels.ProductModel();
            model.LstProduct = new List<ProductModels.ProductModel>();
            lstProductList = new List<ProductModels.ProductModel>();
            List<ProductModels.ProductModel> tempList = new List<ProductModels.ProductModel>();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(procedure.PrcSearchProduct, (SqlConnection)Session["SQLConnection"]);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", keyword);
            cmd.Parameters.AddWithValue("@MainMenuID", mainMenuId);
            cmd.Parameters.AddWithValue("@SubMenuID", subMenuId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productModel = new ProductModels.ProductModel();
                productModel.ProductID = Convert.ToInt32(reader["ProductID"]);
                productModel.ProductName = Convert.ToString(reader["ProductName"]);
                productModel.Code = Convert.ToString(reader["Code"]);
                productModel.SortCode = Convert.ToInt32(reader["SortCode"]);
                productModel.Description = Convert.ToString(reader["Description"]);
                productModel.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                productModel.SubMenuName = Convert.ToString(reader["SubMenuName"]);
                productModel.PurchasePrice = Convert.ToDouble(reader["PurPrice"]);
                productModel.SalePrice = Convert.ToDouble(reader["SalePrice"]);
                productModel.WholeSalePrice = Convert.ToDouble(reader["WholeSalePrice"]);
                productModel.IsUnit = Convert.ToBoolean(reader["IsUnit"]);
                productModel.IsStock = Convert.ToBoolean(reader["IsStock"]);
                productModel.AlertQty = Convert.ToInt32(reader["AlertQty"]);
                productModel.DisPercent = Convert.ToInt32(reader["DisPercent"]);
                productModel.IsVariant = Convert.ToBoolean(reader["IsVariant"]);
                productModel.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                productModel.MainMenuName = Convert.ToString(reader["MainMenuName"]);
                if (isProductPhoto)
                {
                    if (reader["Photo"].ToString().Length != 0)
                    {
                        productModel.Photo = (byte[])(reader["Photo"]);
                        productModel.Base64Photo = Convert.ToBase64String(productModel.Photo);
                    }
                }
                tempList.Add(productModel);
            }
            reader.Close();
            dataConnectorSQL.Close();

            if (tempList.Count > setting.pageSize)
            {
                model.TotalPageNum = tempList.Count / setting.pageSize;
                setting.left = tempList.Count % setting.pageSize;
                if (setting.left != 0) model.TotalPageNum += 1;

                int i = p * setting.pageSize;
                int j = (i - setting.pageSize) + 1;
                int start = j;
                int end = i;
                setting.startRowIndex = start - 1;
                setting.endRowIndex = end - 1;
                Session["PageNumber"] = "Page : " + p;
            }
            else
            {
                setting.startRowIndex = 0;
                setting.endRowIndex = tempList.Count - 1;
                Session["PageNumber"] = "";
            }

            for (int page = setting.startRowIndex; page < tempList.Count; page++)
            {
                if (page > setting.endRowIndex) break;

                productModel = new ProductModels.ProductModel();
                productModel.ProductID = tempList[page].ProductID;
                productModel.ProductName = tempList[page].ProductName;
                productModel.Code = tempList[page].Code;
                productModel.SortCode = tempList[page].SortCode;
                productModel.Description = tempList[page].Description;
                productModel.SubMenuID = tempList[page].SubMenuID;
                productModel.SubMenuName = tempList[page].SubMenuName;
                productModel.PurchasePrice = tempList[page].PurchasePrice;
                productModel.SalePrice = tempList[page].SalePrice;
                productModel.WholeSalePrice = tempList[page].WholeSalePrice;
                productModel.IsUnit = tempList[page].IsUnit;
                productModel.IsStock = tempList[page].IsStock;
                productModel.AlertQty = tempList[page].AlertQty;
                productModel.DisPercent = tempList[page].DisPercent;
                productModel.IsVariant = tempList[page].IsVariant;
                productModel.MainMenuID = tempList[page].MainMenuID;
                productModel.MainMenuName = tempList[page].MainMenuName;
                if (isProductPhoto)
                {
                    if (tempList[page].Photo.ToString().Length != 0)
                    {
                        productModel.Photo = tempList[page].Photo;
                        productModel.Base64Photo = Convert.ToBase64String(productModel.Photo);
                    }
                }

                model.LstProduct.Add(productModel);
                lstProductList.Add(productModel);
            }
        }
       
    }
}
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
        DataTable dtUnit = new DataTable();
        Setting setting = new Setting();

        public ActionResult ProductEntry(int productId)
        {                     
            getMainMenu();
            if (model.MainMenus.Count() != 0) getSubMenuByMainMenu(Convert.ToInt32(model.MainMenus[0].Value));
          
            clearSessionPhoto();           

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

                    break;
                }
            }
            else
            {
                Session["IsEdit"] = 0;
            }
            return View(model);
        }

        public ActionResult ProductList(int p)
        {           
            getMainMenuDefaultInclude();
            getSubMenuDefaultInclude();

            getProductList(p, "", 0, 0);
            
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
        public JsonResult SaveAction(string productName, string code, int sortCode, string description, double purchasePrice, double salePrice, double wholeSalePrice, int alertQty, short disPercent, bool isStock, int subMenuID)
        {
            string message = "";
            bool isSuccess = false;
            int productId = 0;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcInsertProduct, (SqlConnection)Session["SQLConnection"]);
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

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isSuccess = Convert.ToBoolean(reader["IsSuccess"]);
                message = Convert.ToString(reader["Message"]);
                productId = Convert.ToInt32(reader["ProductID"]);
            }
            reader.Close();
            dataConnectorSQL.Close();

            savePhoto(productId);           
                          
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
                Base64Photo = base64Photo
            };

            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAction(string productName, string code, int sortCode, string description, double purchasePrice, double salePrice, double wholeSalePrice, int alertQty, short disPercent, bool isStock, int subMenuID)
        {
            string message = "";
            bool isSuccess = false;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateProduct, (SqlConnection)Session["SQLConnection"]);
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

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isSuccess = Convert.ToBoolean(reader["IsSuccess"]);
                message = Convert.ToString(reader["Message"]);
            }
            reader.Close();
            dataConnectorSQL.Close();

            savePhoto(editProductID);

            var Result = new
            {
                MESSAGE = message,
                ISSUCCESS = isSuccess
            };

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchAction(string keyword, int mainMenuId, int? subMenuId,int p)
        {
            ProductModels.ProductModel productModel = new ProductModels.ProductModel();
            model.LstProduct = new List<ProductModels.ProductModel>();
            lstProductList = new List<ProductModels.ProductModel>();        

            getProductList(p, keyword, mainMenuId, subMenuId);
            
            var myResult = new
            {
                LstProduct = model.LstProduct,
                TotalPageNum = model.TotalPageNum
            };
           
            return Json(myResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int productId)
        {            
            string message = "";
            bool isSuccess = false;

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcDeleteProduct, (SqlConnection)Session["SQLConnection"]);
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

            return Json("", JsonRequestBehavior.AllowGet);
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

        //private void getUnit()
        //{            
        //    if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
        //    SqlCommand cmd = new SqlCommand("Select UnitID,Keyword From S_Unit Order By ULID", (SqlConnection)Session["SQLConnection"]);
        //    cmd.CommandType = CommandType.Text;

        //    SqlDataReader reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        model.Units.Add(new SelectListItem { Text = Convert.ToString(reader["Keyword"]), Value = Convert.ToString(reader["UnitID"]) });
        //    }
        //    reader.Close();
        //    dataConnectorSQL.Close();
        //}

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
       
        private void getProductList(int p,string keyword, int mainMenuId, int? subMenuId)
        {
            ProductModels.ProductModel productModel = new ProductModels.ProductModel();
            model.LstProduct = new List<ProductModels.ProductModel>();
            lstProductList = new List<ProductModels.ProductModel>();
            List<ProductModels.ProductModel> tempList = new List<ProductModels.ProductModel>();

            if (Session["SQLConnection"] == null) Session["SQLConnection"] = dataConnectorSQL.Connect();
            SqlCommand cmd = new SqlCommand(Procedure.PrcSearchProduct, (SqlConnection)Session["SQLConnection"]);
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
                productModel.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                productModel.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                productModel.WholeSalePrice = Convert.ToInt32(reader["WholeSalePrice"]);              
                productModel.IsStock = Convert.ToBoolean(reader["IsStock"]);
                productModel.AlertQty = Convert.ToInt32(reader["AlertQty"]);
                productModel.DisPercent = Convert.ToInt16(reader["DisPercent"]);             
                productModel.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                productModel.MainMenuName = Convert.ToString(reader["MainMenuName"]);               
                if (reader["Photo"].ToString().Length != 0)
                {
                    productModel.Photo = (byte[])(reader["Photo"]);
                    productModel.Base64Photo = Convert.ToBase64String(productModel.Photo);
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
                productModel.IsStock = tempList[page].IsStock;
                productModel.AlertQty = tempList[page].AlertQty;
                productModel.DisPercent = tempList[page].DisPercent;               
                productModel.MainMenuID = tempList[page].MainMenuID;
                productModel.MainMenuName = tempList[page].MainMenuName;               
                if (tempList[page].Photo.ToString().Length != 0)
                {
                    productModel.Photo = tempList[page].Photo;
                    productModel.Base64Photo = Convert.ToBase64String(productModel.Photo);
                }             
                model.LstProduct.Add(productModel);
                lstProductList.Add(productModel);
            }
        }
       
    }
}
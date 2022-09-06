using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class ProductModels
    {
        public class ProductModel
        {
            public ProductModel()
            {
                this.MainMenus = new List<SelectListItem>();
                this.SubMenus = new List<SelectListItem>();
                this.Units = new List<SelectListItem>();
                this.LstProduct = new List<ProductModels.ProductModel>();
                this.LstUnit = new List<ProductModels.ProductUnit>();
                this.LstVariant = new List<ProductModels.ProductVariant>();
            }
            public List<ProductModels.ProductModel> LstProduct { get; set; }
            public List<ProductModels.ProductUnit> LstUnit { get; set; }
            public List<ProductModels.ProductVariant> LstVariant { get; set; }
            public List<SelectListItem> MainMenus { get; set; }
            public List<SelectListItem> SubMenus { get; set; }
            public List<SelectListItem> Units { get; set; }
            public int ProductID { get; set; }
            [Required(ErrorMessage = "Please Enter Product Code")]
            public string Code { get; set; }
            [DisplayName("Sort Code")]
            [Required(ErrorMessage = "Please Enter Sort Code")]
            public int SortCode { get; set; }
            [DisplayName("Name")]
            [Required(ErrorMessage = "Please Enter Product Name")]
            public string ProductName { get; set; }
            public string Description { get; set; }
            public int SubMenuID { get; set; }
            public int MainMenuID { get; set; }
            [DisplayName("Purchase Price")]
            public double PurchasePrice { get; set; }
            [DisplayName("Sale Price")]
            public double SalePrice { get; set; }
            [DisplayName("Whole Sale Price")]
            public double WholeSalePrice { get; set; }
            public bool IsUnit { get; set; }
            public bool IsStock { get; set; }
            public byte[] Photo { get; set; }
            public string Base64Photo { get; set; }
            [DisplayName("Alert Quantity")]
            public int? AlertQty { get; set; }
            [DisplayName("Discount(%)")]
            public int? DisPercent { get; set; }
            public string Barcode { get; set; }
            public string QRcode { get; set; }
            public bool IsVariant { get; set; }
            public string SubMenuName { get; set; }
            public string MainMenuName { get; set; }
            public int UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public string Variant { get; set; }
            public int TotalPageNum { get; set; }
        }
        public class ProductBranch
        {
            public int ID { get; set; }
            public int ProductID { get; set; }
            public int BranchID { get; set; }
        }
        public class ProductUnit
        {
            public ProductUnit()
            {
                this.LstUnit = new List<ProductModels.ProductUnit>();
            }
            public List<ProductModels.ProductUnit> LstUnit { get; set; }
            public int ID { get; set; }
            public int ProductID { get; set; }
            public int UnitID { get; set; }
            public double PurchasePrice { get; set; }
            public double SalePrice { get; set; }
            public short? DiscountPercent { get; set; }
            public string Description { get; set; }
            public string UnitKeyword { get; set; }
        }
        public class ProductVariant
        {
            public int ID { get; set; }
            public int ProductID { get; set; }
            public string Variant { get; set; }
        }
        public class ProductDetail
        {
            public ProductModel ProductModel { get; set; }
            public ProductBranch ProductBranch { get; set; }
            public ProductUnit ProductUnit { get; set; }
            public ProductVariant ProductVariant { get; set; }
            public List<BranchModels.BranchModel> lstBranch { get; set; }

        }
    }
}
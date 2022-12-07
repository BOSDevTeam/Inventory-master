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
                this.LstProduct = new List<ProductModels.ProductModel>();
            }
            public List<ProductModels.ProductModel> LstProduct { get; set; }
            public List<SelectListItem> MainMenus { get; set; }
            public List<SelectListItem> SubMenus { get; set; }
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
            public int PurchasePrice { get; set; }
            [DisplayName("Sale Price")]
            public int SalePrice { get; set; }
            [DisplayName("Whole Sale Price")]
            public int WholeSalePrice { get; set; }
            public bool IsStock { get; set; }
            public byte[] Photo { get; set; }
            public string Base64Photo { get; set; }
            [DisplayName("Alert Quantity")]
            public int? AlertQty { get; set; }
            [DisplayName("Discount(%)")]
            public short? DisPercent { get; set; }
            public string SubMenuName { get; set; }
            public string MainMenuName { get; set; }
            public int TotalPageNum { get; set; }
            public int Quantity { get; set; }
        }     
        public class ProductDetail
        {
            public ProductModel ProductModel { get; set; }
        }
    }
}
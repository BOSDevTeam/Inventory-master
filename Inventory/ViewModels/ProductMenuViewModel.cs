using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class ProductMenuViewModel
    {
        public IEnumerable<MainMenuModels.MainMenuModel> MainMenus { get; set; }
        public IEnumerable<SubMenuModels.SubMenuModel> SubMenus { get; set; }
        public IEnumerable<ProductModels.ProductModel> Products { get; set; }
    }
}
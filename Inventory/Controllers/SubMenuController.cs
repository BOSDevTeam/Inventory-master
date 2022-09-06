using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class SubMenuController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        SubMenuModels.SubMenuModel model = new SubMenuModels.SubMenuModel();
        static List<SubMenuModels.SubMenuModel> lstSubMenuList = new List<SubMenuModels.SubMenuModel>();
        static int editSubMenuID;

        public ActionResult SubMenuEntry(int subMenuId)
        {            
            ViewBag.IsProductPhoto = isProductPhoto();
            GetMainMenu();
            clearSessionPhoto();
            if (subMenuId != 0)
            {
                Session["IsEdit"] = 1;
                editSubMenuID = subMenuId;
                var editSubMenu = lstSubMenuList.Where(c => c.SubMenuID == subMenuId);
                foreach (var e in editSubMenu)
                {
                    Session["EditSubMenuName"] = e.SubMenuName;
                    Session["EditCode"] = e.Code;
                    Session["EditSortCode"] = e.SortCode;
                    Session["EditMainMenuID"] = e.MainMenuID;

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

        public ActionResult SubMenuList()
        {           
            ViewBag.IsProductPhoto = isProductPhoto();
            GetMainMenuDefaultInclude();
            SubMenuModels.SubMenuModel subMenuModel = new SubMenuModels.SubMenuModel();
            model.LstSubMenu = new List<SubMenuModels.SubMenuModel>();
            lstSubMenuList = new List<SubMenuModels.SubMenuModel>();

            foreach (var sub in Entities.PrcGetSubMenu())
            {
                subMenuModel = new SubMenuModels.SubMenuModel();
                subMenuModel.SubMenuID = sub.SubMenuID;
                subMenuModel.SubMenuName = sub.SubMenuName;
                subMenuModel.Code = sub.Code;
                subMenuModel.SortCode = sub.SortCode;
                subMenuModel.MainMenuID = sub.MainMenuID;
                subMenuModel.MainMenuName = sub.MainMenuName;
                if (isProductPhoto())
                {
                    if (sub.Photo != null)
                    {
                        subMenuModel.Photo = sub.Photo;
                        subMenuModel.Base64Photo = Convert.ToBase64String(sub.Photo);
                    }
                }

                model.LstSubMenu.Add(subMenuModel);
                lstSubMenuList.Add(subMenuModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string subMenuName, string code, int sortCode, int mainMenuID)
        {
            string message;
            int saveOk;
            var subs = (from sub in Entities.S_SubMenu where sub.Code == code where sub.MainMenuID == mainMenuID select sub).ToList();
            if (subs.Count() == 0)
            {
                var subss = (from sub in Entities.S_SubMenu where sub.SortCode == sortCode select sub).ToList();
                if (subss.Count() == 0)
                {
                    S_SubMenu table = new S_SubMenu();
                    table.SubMenuName = subMenuName;
                    table.Code = code;
                    table.SortCode = sortCode;
                    table.MainMenuID = mainMenuID;

                    if (isProductPhoto())
                    {
                        if (Session["PhotoFile"] != null)
                        {
                            HttpPostedFileBase file = (HttpPostedFileBase)Session["PhotoFile"];
                            if (file != null)
                            {
                                if (file.ContentLength > 0)
                                {
                                    table.Photo = new byte[file.ContentLength];
                                    file.InputStream.Read(table.Photo, 0, file.ContentLength);
                                }
                            }
                            clearSessionPhoto();
                        }                    
                    }

                    Entities.S_SubMenu.Add(table);
                    Entities.SaveChanges();

                    message = "Saved Successfully!";
                    saveOk = 1;
                }
                else
                {
                    message = "Sort Code Duplicate!";
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
        public JsonResult EditAction(string subMenuName, string code, int sortCode, int mainMenuID)
        {
            string message = "";
            int editOk = 0;
            var subs = (from sub in Entities.S_SubMenu where sub.Code == code where sub.MainMenuID == mainMenuID where sub.SubMenuID != editSubMenuID select sub).ToList();
            if (subs.Count() == 0)
            {
                var subss = (from sub in Entities.S_SubMenu where sub.SortCode == sortCode where sub.SubMenuID != editSubMenuID select sub).ToList();
                if (subss.Count() == 0)
                {
                    var result = Entities.S_SubMenu.SingleOrDefault(c => c.SubMenuID == editSubMenuID);
                    if (result != null)
                    {
                        result.SubMenuName = subMenuName;
                        result.Code = code;
                        result.SortCode = sortCode;
                        result.MainMenuID = mainMenuID;

                        if (isProductPhoto())
                        {
                            if (Session["PhotoFile"] != null)
                            {
                                HttpPostedFileBase file = (HttpPostedFileBase)Session["PhotoFile"];
                                if (file != null)
                                {
                                    if (file.ContentLength > 0)
                                    {
                                        result.Photo = new byte[file.ContentLength];
                                        file.InputStream.Read(result.Photo, 0, file.ContentLength);
                                    }
                                }
                                clearSessionPhoto();
                            }                          
                        }

                        Entities.SaveChanges();

                        message = "Edited Successfully!";
                        editOk = 1;
                    }
                }
                else
                {
                    message = "Sort Code Duplicate!";
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
        public JsonResult SearchAction(int mainMenuID, string keyword)
        {
            SubMenuModels.SubMenuModel subMenuModel = new SubMenuModels.SubMenuModel();
            model.LstSubMenu = new List<SubMenuModels.SubMenuModel>();
            lstSubMenuList = new List<SubMenuModels.SubMenuModel>();

            foreach (var sub in Entities.PrcSearchSubMenu(mainMenuID, keyword))
            {
                subMenuModel = new SubMenuModels.SubMenuModel();
                subMenuModel.SubMenuID = sub.SubMenuID;
                subMenuModel.SubMenuName = sub.SubMenuName;
                subMenuModel.Code = sub.Code;
                subMenuModel.SortCode = sub.SortCode;
                if (isProductPhoto())
                {
                    if (sub.Photo != null)
                    {
                        subMenuModel.Photo = sub.Photo;
                        subMenuModel.Base64Photo = Convert.ToBase64String(sub.Photo);
                    }
                }
                subMenuModel.MainMenuID = sub.MainMenuID;
                subMenuModel.MainMenuName = sub.MainMenuName;

                model.LstSubMenu.Add(subMenuModel);
                lstSubMenuList.Add(subMenuModel);
            }

            return Json(model.LstSubMenu, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int subMenuId)
        {
            int delOk;
            var pros = (from pro in Entities.S_Product where pro.SubMenuID == subMenuId select pro).ToList();
            if (pros.Count == 0)
            {
                S_SubMenu sub = Entities.S_SubMenu.Where(x => x.SubMenuID == subMenuId).Single<S_SubMenu>();
                Entities.S_SubMenu.Remove(sub);
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

        [HttpGet]
        public JsonResult RefreshAction()
        {
            clearSessionPhoto();

            return Json("", JsonRequestBehavior.AllowGet);
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

        private void clearSessionPhoto()
        {
            Session["PhotoFile"] = null;
        }

        private void GetMainMenu()
        {
            foreach (var main in Entities.S_MainMenu.OrderBy(m => m.SortCode))
            {
                model.MainMenus.Add(new SelectListItem { Text = main.MainMenuName, Value = main.MainMenuID.ToString() });
            }
        }

        private void GetMainMenuDefaultInclude()
        {
            model.MainMenus.Add(new SelectListItem { Text = "Main Menu", Value = "0" });
            foreach (var main in Entities.S_MainMenu.OrderBy(m => m.SortCode))
            {
                model.MainMenus.Add(new SelectListItem { Text = main.MainMenuName, Value = main.MainMenuID.ToString() });
            }
        }

        private bool isProductPhoto()
        {
            CompanySettingModels cModel = new CompanySettingModels();
            var isProductPhoto = Entities.S_CompanySetting.Select(c => c.IsProductPhoto);
            cModel.IsProductPhoto = isProductPhoto.FirstOrDefault();
            return Convert.ToBoolean(cModel.IsProductPhoto);
        }
    }
}
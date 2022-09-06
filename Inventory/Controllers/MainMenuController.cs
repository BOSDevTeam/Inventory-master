using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Inventory.Common;

namespace Inventory.Controllers
{
    public class MainMenuController : MyController
    {
        InventoryDBEntities Entities = new InventoryDBEntities();
        MainMenuModels.MainMenuModel model = new MainMenuModels.MainMenuModel();
        static List<MainMenuModels.MainMenuModel> lstMainMenuList = new List<MainMenuModels.MainMenuModel>();
        static int editMainMenuID;

        public ActionResult MainMenuEntry(int mainMenuId)
        {           
            ViewBag.IsProductPhoto = isProductPhoto();
            clearSessionPhoto();
            if (mainMenuId != 0)
            {
                Session["IsEdit"] = 1;
                editMainMenuID = mainMenuId;
                var editMainMenu = lstMainMenuList.Where(c => c.MainMenuID == mainMenuId);
                foreach (var e in editMainMenu)
                {
                    Session["EditMainMenuName"] = e.MainMenuName;
                    Session["EditCode"] = e.Code;
                    Session["EditSortCode"] = e.SortCode;

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

        public ActionResult MainMenuList()
        {         
            ViewBag.IsProductPhoto = isProductPhoto();
            MainMenuModels.MainMenuModel mainMenuModel = new MainMenuModels.MainMenuModel();
            model.LstMainMenu = new List<MainMenuModels.MainMenuModel>();
            lstMainMenuList = new List<MainMenuModels.MainMenuModel>();

            foreach (var main in Entities.S_MainMenu.OrderBy(m => m.SortCode))
            {
                mainMenuModel = new MainMenuModels.MainMenuModel();
                mainMenuModel.MainMenuID = main.MainMenuID;
                mainMenuModel.MainMenuName = main.MainMenuName;
                mainMenuModel.Code = main.Code;
                mainMenuModel.SortCode = main.SortCode;
                if (isProductPhoto())
                {
                    if (main.Photo != null)
                    {
                        mainMenuModel.Photo = main.Photo;
                        mainMenuModel.Base64Photo = Convert.ToBase64String(main.Photo);
                    }
                }

                model.LstMainMenu.Add(mainMenuModel);
                lstMainMenuList.Add(mainMenuModel);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult SaveAction(string mainMenuName, string code, int sortCode)
        {
            string message;
            int saveOk;
            var mains = (from main in Entities.S_MainMenu where main.Code == code select main).ToList();
            if (mains.Count() == 0)
            {
                var mainss = (from main in Entities.S_MainMenu where main.SortCode == sortCode select main).ToList();
                if (mainss.Count() == 0)
                {
                    S_MainMenu table = new S_MainMenu();
                    table.MainMenuName = mainMenuName;
                    table.Code = code;
                    table.SortCode = sortCode;

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

                    Entities.S_MainMenu.Add(table);
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
        public JsonResult EditAction(string mainMenuName, string code, int sortCode)
        {
            string message = "";
            int editOk = 0;
            var mains = (from main in Entities.S_MainMenu where main.Code == code where main.MainMenuID != editMainMenuID select main).ToList();
            if (mains.Count() == 0)
            {
                var mainss = (from main in Entities.S_MainMenu where main.SortCode == sortCode where main.MainMenuID != editMainMenuID select main).ToList();
                if (mainss.Count() == 0)
                {
                    var result = Entities.S_MainMenu.SingleOrDefault(c => c.MainMenuID == editMainMenuID);
                    if (result != null)
                    {
                        result.MainMenuName = mainMenuName;
                        result.Code = code;
                        result.SortCode = sortCode;

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
        public JsonResult SearchAction(string keyword)
        {
            MainMenuModels.MainMenuModel mainMenuModel = new MainMenuModels.MainMenuModel();
            model.LstMainMenu = new List<MainMenuModels.MainMenuModel>();
            lstMainMenuList = new List<MainMenuModels.MainMenuModel>();

            foreach (var main in Entities.PrcSearchMainMenu(keyword))
            {
                mainMenuModel = new MainMenuModels.MainMenuModel();
                mainMenuModel.MainMenuID = main.MainMenuID;
                mainMenuModel.MainMenuName = main.MainMenuName;
                mainMenuModel.Code = main.Code;
                mainMenuModel.SortCode = main.SortCode;
                if (isProductPhoto())
                {
                    if (main.Photo != null)
                    {
                        mainMenuModel.Photo = main.Photo;
                        mainMenuModel.Base64Photo = Convert.ToBase64String(main.Photo);
                    }
                }

                model.LstMainMenu.Add(mainMenuModel);
                lstMainMenuList.Add(mainMenuModel);
            }

            return Json(model.LstMainMenu, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAction(int mainMenuId)
        {
            int delOk;
            var subs = (from sub in Entities.S_SubMenu where sub.MainMenuID == mainMenuId select sub).ToList();
            if (subs.Count == 0)
            {
                S_MainMenu main = Entities.S_MainMenu.Where(x => x.MainMenuID == mainMenuId).Single<S_MainMenu>();
                Entities.S_MainMenu.Remove(main);
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

        private bool isProductPhoto()
        {
            CompanySettingModels cModel = new CompanySettingModels();
            var isProductPhoto = Entities.S_CompanySetting.Select(c => c.IsProductPhoto);
            cModel.IsProductPhoto = isProductPhoto.FirstOrDefault();
            return Convert.ToBoolean(cModel.IsProductPhoto);
        }
    }
}
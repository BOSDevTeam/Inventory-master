using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using Inventory.Common;
using Inventory.Models;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class OpeningStockController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        OpeningStockViewModel openingStockViewModel = new OpeningStockViewModel();
        AppData appData = new AppData();
        TextQuery textQuery = new TextQuery();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();

        [SessionTimeoutAttribute]
        public ActionResult OpeningStock(int userId, int? openingStockId)
        {
            if (checkConnection())
            {               
                getLocation();
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                clearOSSession();
                if(openingStockId != null) // edit
                {
                    ViewBag.IsEdit = true;
                    OpeningStockViewModel.MasterOpeningStockViewModel data = selectMasterOpeningStockByID((int)openingStockId);
                    List<ProductModels.ProductModel> lstTranOpeningStock = selectTranOpeningStockByID((int)openingStockId);                  
                    Session["TranOpeningStockData"] = lstTranOpeningStock;                 
                    ViewBag.UserVoucherNo = data.UserVoucherNo;
                    DateTime date = setting.convertStringToDate(data.OpeningDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = data.VoucherID;                 
                    ViewBag.LocationID = data.LocationID;                  
                    ViewBag.OpeningStockID = openingStockId;
                }
                else ViewBag.UserVoucherNo = getUserVoucherNo(userId);

                return View(openingStockViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        [SessionTimeoutAttribute]
        public ActionResult ListOpeningStock(int userId)
        {
            if (checkConnection())
            {              
                List<OpeningStockViewModel.MasterOpeningStockViewModel> tempList = selectMasterOpeningStock(userId);
                PagingViewModel pagingViewModel = calcMasterOpeningStockPaging(tempList);
                List<OpeningStockViewModel.MasterOpeningStockViewModel> lstMasterOpeningStock = getMasterOpeningStockByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstMasterOpeningStock"] = lstMasterOpeningStock;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
                return View(openingStockViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public JsonResult AddQuantityAction(int productId, int quantity)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranOpeningStockData"] != null)
            {
                try
                {
                    List<ProductModels.ProductModel> lstTranOpeningStock = Session["TranOpeningStockData"] as List<ProductModels.ProductModel>;
                    ProductModels.ProductModel product = lstTranOpeningStock.Where(x => x.ProductID == productId).SingleOrDefault();
                    int index = lstTranOpeningStock.IndexOf(product);
                    product.Quantity = quantity;
                    lstTranOpeningStock.RemoveAt(index);
                    lstTranOpeningStock.Insert(index, product);
                    Session["TranOpeningStockData"] = lstTranOpeningStock;
                    resultDefaultData.IsRequestSuccess = true;
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SubMenuClickAction(int subMenuId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            getProduct(subMenuId);
            if (!checkIsExistInTran(openingStockViewModel.ProductMenus.Products))
            {
                resultDefaultData.IsRequestSuccess = true;
                setOSSession();
            }

            var jsonResult = new
            {
                Products = openingStockViewModel.ProductMenus.Products,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ResetAction()
        {
            clearOSSession();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAction(string userVoucherNo, string date, string voucherId, int locationId, int userId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranOpeningStockData"] != null)
            {
                try
                {
                    List<ProductModels.ProductModel> list = Session["TranOpeningStockData"] as List<ProductModels.ProductModel>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity);
                    }

                    DateTime openingDateTime = DateTime.Parse(date);
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertOpeningStock, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@OpeningDateTime", openingDateTime);
                    cmd.Parameters.AddWithValue("@LocationID", locationId);
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.OpeningStockModule);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string val = Convert.ToString(reader[0]);
                        if (val.Length == 1) // duplicate location
                        {
                            resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.InCompletedData.ToString();
                            resultDefaultData.Message = "Already have opening stock with this location!";
                        }
                        else
                        {
                            userVoucherNo = val;
                            clearOSSession();
                            resultDefaultData.IsRequestSuccess = true;
                            resultDefaultData.Message = AppConstants.Message.SaveSuccess;
                        }
                    }
                    reader.Close();
                    dataConnectorSQL.Close();
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                UserVoucherNo = userVoucherNo,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditAction(int openingStockId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();

            if (Session["TranOpeningStockData"] != null)
            {
                try
                {
                    List<ProductModels.ProductModel> list = Session["TranOpeningStockData"] as List<ProductModels.ProductModel>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity);
                    }

                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateOpeningStock, dataConnectorSQL.Connect());
                    cmd.Parameters.AddWithValue("@OpeningStockID", openingStockId);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    dataConnectorSQL.Close();
                    resultDefaultData.IsRequestSuccess = true;
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteAction(int openingStockId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalPageNum = 0;

            if (Session["MasterOpeningStockData"] != null)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(textQuery.deleteOpeningStockQuery(openingStockId), (SqlConnection)getConnection());
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    List<OpeningStockViewModel.MasterOpeningStockViewModel> lstMasterOpeningStock = Session["MasterOpeningStockData"] as List<OpeningStockViewModel.MasterOpeningStockViewModel>;
                    int index = lstMasterOpeningStock.FindIndex(x => x.OpeningStockID == openingStockId);
                    lstMasterOpeningStock.RemoveAt(index);

                    if (lstMasterOpeningStock.Count > paging.eachItemCount)
                    {
                        totalPageNum = lstMasterOpeningStock.Count / paging.eachItemCount;
                        paging.lastItemCount = lstMasterOpeningStock.Count % paging.eachItemCount;
                        if (paging.lastItemCount != 0) totalPageNum += 1;
                    }
                    resultDefaultData.IsRequestSuccess = true;
                    resultDefaultData.Message = AppConstants.Message.DeleteSuccess;
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                TotalPage = totalPageNum,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OpeningStockPagingAction(int currentPage)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<OpeningStockViewModel.MasterOpeningStockViewModel> lstMasterOpeningStock = new List<OpeningStockViewModel.MasterOpeningStockViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["MasterOpeningStockData"] != null)
            {
                try
                {
                    List<OpeningStockViewModel.MasterOpeningStockViewModel> tempList = Session["MasterOpeningStockData"] as List<OpeningStockViewModel.MasterOpeningStockViewModel>;
                    pagingViewModel = calcMasterOpeningStockPaging(tempList, currentPage);
                    lstMasterOpeningStock = getMasterOpeningStockByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                    resultDefaultData.IsRequestSuccess = true;
                }
                catch (Exception ex)
                {
                    resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                    resultDefaultData.Message = ex.Message;
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                LstMasterOpeningStock = lstMasterOpeningStock,
                TotalPage = pagingViewModel.TotalPageNum,
                ResultDefaultData = resultDefaultData
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        #region Method

        private OpeningStockViewModel.MasterOpeningStockViewModel selectMasterOpeningStockByID(int openingStockId)
        {
            OpeningStockViewModel.MasterOpeningStockViewModel item = new OpeningStockViewModel.MasterOpeningStockViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterOpeningStockByID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OpeningStockID", openingStockId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.OpeningDateTime = Convert.ToString(reader["Date"]);               
                item.LocationID = Convert.ToInt32(reader["LocationID"]);
            }
            reader.Close();

            return item;
        }

        private List<OpeningStockViewModel.MasterOpeningStockViewModel> selectMasterOpeningStock(int userId)
        {
            List<OpeningStockViewModel.MasterOpeningStockViewModel> tempList = new List<OpeningStockViewModel.MasterOpeningStockViewModel>();
            OpeningStockViewModel.MasterOpeningStockViewModel item = new OpeningStockViewModel.MasterOpeningStockViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterOpeningStockList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new OpeningStockViewModel.MasterOpeningStockViewModel();
                item.OpeningStockID = Convert.ToInt32(reader["OpeningStockID"]);
                item.OpeningDateTime = Convert.ToString(reader["OpeningDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.LocationName = Convert.ToString(reader["LocationName"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterOpeningStockData"] = tempList;  // for paging

            return tempList;
        }

        private List<ProductModels.ProductModel> selectTranOpeningStockByID(int openingStockId)
        {
            List<ProductModels.ProductModel> list = new List<ProductModels.ProductModel>();
            ProductModels.ProductModel item = new ProductModels.ProductModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranOpeningStockByID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OpeningStockID", openingStockId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new ProductModels.ProductModel();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.Code = Convert.ToString(reader["Code"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        private PagingViewModel calcMasterOpeningStockPaging(List<OpeningStockViewModel.MasterOpeningStockViewModel> tempList, [Optional]int currentPage)
        {
            PagingViewModel item = new PagingViewModel();
            int totalPageNum = 0;

            if (currentPage == 0) currentPage = 1;
            if (tempList.Count > paging.eachItemCount)
            {
                totalPageNum = tempList.Count / paging.eachItemCount;
                paging.lastItemCount = tempList.Count % paging.eachItemCount;
                if (paging.lastItemCount != 0) totalPageNum += 1;

                int i = currentPage * paging.eachItemCount;
                int j = (i - paging.eachItemCount) + 1;
                int start = j;
                int end = i;
                paging.startItemIndex = start - 1;
                paging.endItemIndex = end - 1;
            }
            else
            {
                paging.startItemIndex = 0;
                paging.endItemIndex = tempList.Count - 1;
            }

            item.CurrentPage = currentPage;
            item.TotalPageNum = totalPageNum;
            item.StartItemIndex = paging.startItemIndex;
            item.EndItemIndex = paging.endItemIndex;

            return item;
        }

        private List<OpeningStockViewModel.MasterOpeningStockViewModel> getMasterOpeningStockByPaging(List<OpeningStockViewModel.MasterOpeningStockViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<OpeningStockViewModel.MasterOpeningStockViewModel> list = new List<OpeningStockViewModel.MasterOpeningStockViewModel>();
            OpeningStockViewModel.MasterOpeningStockViewModel item = new OpeningStockViewModel.MasterOpeningStockViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new OpeningStockViewModel.MasterOpeningStockViewModel();
                item.OpeningStockID = tempList[page].OpeningStockID;
                item.OpeningDateTime = tempList[page].OpeningDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.VoucherID = tempList[page].VoucherID;
                item.LocationName = tempList[page].LocationName;
                item.UserName = tempList[page].UserName;
                list.Add(item);
            }
            return list;
        }

        private void setOSSession()
        {
            if (Session["TranOpeningStockData"] != null)
            {
                List<ProductModels.ProductModel> lstTranOpeningStock = Session["TranOpeningStockData"] as List<ProductModels.ProductModel>;
                lstTranOpeningStock.AddRange(openingStockViewModel.ProductMenus.Products);
            }
            else
                Session["TranOpeningStockData"] = openingStockViewModel.ProductMenus.Products;
        }

        private void clearOSSession()
        {
            Session["TranOpeningStockData"] = null;
        }

        private void getMainMenu()
        {
            openingStockViewModel.ProductMenus.MainMenus = appData.selectMainMenu(getConnection());
        }

        private int getFirstMainMenuID()
        {
            int mainMenuId = 0;
            if (openingStockViewModel.ProductMenus.MainMenus.Count() != 0)
            {
                MainMenuModels.MainMenuModel firstMainMenu = openingStockViewModel.ProductMenus.MainMenus.First();
                mainMenuId = firstMainMenu.MainMenuID;
            }
            return mainMenuId;
        }

        private void getSubMenu(int mainMenuId)
        {
            openingStockViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(), mainMenuId);
        }

        private void getProduct(int subMenuId)
        {
            openingStockViewModel.ProductMenus.Products = appData.selectProduct(getConnection(), subMenuId);      
        }

        private bool checkIsExistInTran(IEnumerable<ProductModels.ProductModel> products)
        {
            bool result = false;
            int productId = 0;

            if (products.Count() != 0)
            {
                productId = products.ElementAt(0).ProductID;
                if (Session["TranOpeningStockData"] != null)
                {
                    List<ProductModels.ProductModel> lstTranOpeningStock = Session["TranOpeningStockData"] as List<ProductModels.ProductModel>;
                    if (lstTranOpeningStock.Count() != 0)
                    {
                        ProductModels.ProductModel product=lstTranOpeningStock.Where(x => x.ProductID == productId).SingleOrDefault();
                        if (product != null) result = true;                      
                    }
                }
            }
            return result;
        }

        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                openingStockViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
        }

        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.OpeningStockModule, userId, getConnection());
            return userVoucherNo;
        }

        private bool checkConnection()
        {
            if (Session[AppConstants.SQLConnection] != null) return true;
            else return false;
        }

        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();

            connection = Session[AppConstants.SQLConnection];
            return connection;
        }

        #endregion
    }
}
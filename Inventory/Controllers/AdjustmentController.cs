using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Models;
using Inventory.Common;
using Inventory.ViewModels;
using System.Runtime.InteropServices;

namespace Inventory.Controllers
{
    public class AdjustmentController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        AppData appData = new AppData();
        AdjustmentViewModel adjustmentViewModel = new AdjustmentViewModel();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();
        TextQuery textQuery = new TextQuery();
        public ActionResult Adjustment(int userId,int? adjustmentId)
        {
            if (checkConnection())
            {
                getLocation();
                getAdjustType(true);
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());

                if (adjustmentId != null)   //edit mode
                {
                    ViewBag.IsEdit = true;
                    MasterAdjustmentViewModel data = selectMasterAdjustment((int)adjustmentId);
                    List<TranAdjustmentModels> lstTranAdjustment = selectTranAdjustmentByAdjustmentID((int)adjustmentId);
                    
                    Session["TranAdjustmentData"] = lstTranAdjustment;
                    
                    ViewBag.UserVoucherNo = data.MasterAdjustmentModel.UserVoucherNo;
                    DateTime date = setting.convertStringToDate(data.MasterAdjustmentModel.AdjustDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterAdjustmentModel.VoucherID;
                    ViewBag.LocationID = data.MasterAdjustmentModel.LocationID;
                    ViewBag.AdjustmentID = adjustmentId;
                    ViewBag.Remark = data.MasterAdjustmentModel.Remark;
                }
                //new mode
                else
                {
                    ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                }
                return View(adjustmentViewModel);
            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult ListAdjustment(int userId)
        {
            if(checkConnection())
            {
                List<AdjustmentViewModel.MasterAdjustmentViewModel> tempList = selectMasterAdjustment(userId, false);
                PagingViewModel pagingViewModel = calcMasterAdjustmentPaging(tempList);
                List<AdjustmentViewModel.MasterAdjustmentViewModel> lstMasterAdjustment = getMasterAdjustmentByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstMasterAdjustment"] = lstMasterAdjustment;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
                return View(adjustmentViewModel);
            }
            return RedirectToAction("Login", "User");           
        }
        #region Adjustment Action
        [HttpGet]
        public JsonResult GetProductByCodeAction(string productCode, bool isMultiUnit)
        {
            ProductModels.ProductModel data = new ProductModels.ProductModel();
            string productName = "";
            int productId = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<AdjustTypeModels> lstAdjustment = new List<AdjustTypeModels>();
            bool isExistProduct = true;

            data = appData.selectProductByCode(getConnection(), productCode);
            if (data.ProductID != 0)
            {
                productId = data.ProductID;
                productName = data.ProductName;
                if (isMultiUnit) lstUnit = getUnit();
            }
            else isExistProduct = false;

            var jsonResult = new
            {
                ProductID = productId,
                ProductName = productName,
                LstUnit = lstUnit,
                IsExistProduct = isExistProduct
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetProductByKeywordAction(string keyword)
        {
            List<ProductModels.ProductModel> list = appData.selectProductByKeyword(getConnection(), keyword);
            Session["SearchProductData"] = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchProductClickAction(int productId, bool isMultiUnit)
        {
            string productName = "", code = "";
            List<UnitModels> lstUnit = new List<UnitModels>();
            bool isRequestSuccess = false;

            if (Session["SearchProductData"] != null)
            {
                List<ProductModels.ProductModel> list = Session["SearchProductData"] as List<ProductModels.ProductModel>;
                var result = list.Where(c => c.ProductID == productId).SingleOrDefault();
                if (result != null)
                {
                    productName = result.ProductName;
                    code = result.Code;
                    if (isMultiUnit) lstUnit = getUnit();
                    isRequestSuccess = true;
                }
            }

            var jsonResult = new
            {
                ProductName = productName,
                Code = code,
                LstUnit = lstUnit,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TranAdjustmentAddEditAction(int productId, string productCode, string productName, int quantity, int? unitId, string unitKeyword, int? adjustTypeId, string adjustTypeKeyword, bool isEdit, int? number)
        {
            List<TranAdjustmentModels> lstTranAdjustment = new List<TranAdjustmentModels>();
            TranAdjustmentModels data = new TranAdjustmentModels();
            bool isRequestSuccess = true;

            data.Number = number;
            data.ProductID = productId;
            data.ProductCode = productCode;
            data.Quantity = quantity;
            data.UnitID = unitId;
            data.AdjustTypeID = adjustTypeId;
            data.AdjustTypeKeyword = adjustTypeKeyword;
            data.ProductName = productName;
            if (unitKeyword != null) data.UnitKeyword = unitKeyword;
            else data.UnitKeyword = "";

            if (!isEdit)
            {
                if (Session["TranAdjustmentData"] != null)
                {
                    lstTranAdjustment = Session["TranAdjustmentData"] as List<TranAdjustmentModels>;
                    lstTranAdjustment.Add(data);
                }
                else lstTranAdjustment.Add(data);
            }
            else
            {
                if (Session["TranAdjustmentData"] != null)
                {
                    lstTranAdjustment = Session["TranAdjustmentData"] as List<TranAdjustmentModels>;
                    int index = (int)number - 1;
                    lstTranAdjustment[index] = data;
                }
                else isRequestSuccess = false;
            }
            Session["TranAdjustmentData"] = lstTranAdjustment;

            var jsonResult = new
            {
                LstTranAdjustment = lstTranAdjustment,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult TranAdjustmentDeleteAction(int number)
        {
            List<TranAdjustmentModels> lstTranAdjustment = new List<TranAdjustmentModels>();
            bool isRequestSuccess = true;

            if (Session["TranAdjustmentData"] != null)
            {
                lstTranAdjustment = Session["TranAdjustmentData"] as List<TranAdjustmentModels>;
                lstTranAdjustment.RemoveAt(number - 1);
            }
            else isRequestSuccess = false;
            Session["TranAdjustmentData"] = lstTranAdjustment;

            var jsonResult = new
            {
                LstTranAdjustment = lstTranAdjustment,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult PrepareToEditTranAdjustmentAction(int number, bool isMultiUnit)
        {
            List<TranAdjustmentModels> lstTranAdjustment = new List<TranAdjustmentModels>();
            TranAdjustmentModels data = new TranAdjustmentModels();
            string productCode = "", productName = "";
            int productId = 0, quantity = 0;
            int? unitId = 0, adjustTypeId = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            bool isRequestSuccess = false;

            if (Session["TranAdjustmentData"] != null)
            {
                lstTranAdjustment = Session["TranAdjustmentData"] as List<TranAdjustmentModels>;

                if (lstTranAdjustment.Count() != 0)
                {
                    data = lstTranAdjustment[number - 1];
                    if (data != null)
                    {
                        productId = data.ProductID;
                        productCode = data.ProductCode;
                        productName = data.ProductName;
                        quantity = data.Quantity;
                        unitId = data.UnitID;
                        adjustTypeId = data.AdjustTypeID;
                     
                        if (isMultiUnit) lstUnit = getUnit();
                        isRequestSuccess = true;
                    }
                }
            }

            var jsonResult = new
            {
                ProductID = productId,
                ProductCode = productCode,
                ProductName = productName,
                Quantity = quantity,
                UnitID = unitId,
                AdjustTypeID = adjustTypeId,
                LstUnit = lstUnit,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CancelAction()
        {
            Session["TranAdjustmentData"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AdjustmentSubmitAction(string userVoucherNo, string date, string voucherId, int locationId,
                string remark, int userId)
        {
            bool isRequestSuccess = true;

            if (Session["TranAdjustmentData"] != null)
            {
                List<TranAdjustmentModels> list = Session["TranAdjustmentData"] as List<TranAdjustmentModels>;
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                dt.Columns.Add(new DataColumn("AdjustTypeID", typeof(int)));
                
                for (int i = 0; i < list.Count; i++)
                {
                    dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].AdjustTypeID);
                }

                DateTime adjustDateTime = DateTime.Parse(date);            

                SqlCommand cmd = new SqlCommand(Procedure.PrcInsertAdjustment, dataConnectorSQL.Connect());
                cmd.Parameters.AddWithValue("@AdjustDateTime", adjustDateTime);
                cmd.Parameters.AddWithValue("@LocationID", locationId);               
                cmd.Parameters.AddWithValue("@UserID", userId);                
                cmd.Parameters.AddWithValue("@Remark", remark);
                cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.AdjustmentModule);
                cmd.Parameters.AddWithValue("@temptbl", dt);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) userVoucherNo = Convert.ToString(reader[0]);
                reader.Close();
                dataConnectorSQL.Close();
                clearTranAdjustment();
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                UserVoucherNo = userVoucherNo,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AdjustmentPagingAction(int currentPage)
        {
            bool isRequestSuccess = true;
            List<AdjustmentViewModel.MasterAdjustmentViewModel> lstMasterAdjustment = new List<AdjustmentViewModel.MasterAdjustmentViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["MasterAdjustmentData"] != null)
            {
                List<AdjustmentViewModel.MasterAdjustmentViewModel> tempList = Session["MasterAdjustmentData"] as List<AdjustmentViewModel.MasterAdjustmentViewModel>;
                pagingViewModel = calcMasterAdjustmentPaging(tempList, currentPage);
                lstMasterAdjustment = getMasterAdjustmentByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                LstMasterAdjustment = lstMasterAdjustment,
                TotalPage = pagingViewModel.TotalPageNum,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAction(int adjustmentId)
        {
            bool isRequestSuccess = true;
            int totalPageNum = 0;

            if (Session["MasterAdjustmentData"] != null)
            {
                SqlCommand cmd = new SqlCommand(textQuery.deleteAdjustmentQuery(adjustmentId), (SqlConnection)getConnection());
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                List<AdjustmentViewModel.MasterAdjustmentViewModel> lstMasterAdjustment = Session["MasterAdjustmentData"] as List<AdjustmentViewModel.MasterAdjustmentViewModel>;
                int index = lstMasterAdjustment.FindIndex(x => x.AdjustmentID == adjustmentId);
                lstMasterAdjustment.RemoveAt(index);

                if (lstMasterAdjustment.Count > paging.eachItemCount)
                {
                    totalPageNum = lstMasterAdjustment.Count / paging.eachItemCount;
                    paging.lastItemCount = lstMasterAdjustment.Count % paging.eachItemCount;
                    if (paging.lastItemCount != 0) totalPageNum += 1;
                }
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                TotalPage = totalPageNum,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ViewAction(int adjustmentId)
        {
            MasterAdjustmentViewModel item = selectMasterAdjustment(adjustmentId);
            List<TranAdjustmentModels> lstTranAdjustment = selectTranAdjustmentByAdjustmentID(adjustmentId);

            
            var jsonResult = new
            {
                LstTranAdjustment = lstTranAdjustment,
                UserVoucherNo = item.MasterAdjustmentModel.UserVoucherNo,
                VoucherID = item.MasterAdjustmentModel.VoucherID,
                Remark = item.MasterAdjustmentModel.Remark,
                LocationName = item.LocationName,
                AdjustDateTime = item.MasterAdjustmentModel.AdjustDateTime,
                UserName = item.UserName,               
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult SearchAction(int userId, DateTime fromDate, DateTime toDate, string userVoucherNo)
        {
            List<AdjustmentViewModel.MasterAdjustmentViewModel> tempList = selectMasterAdjustment(userId, true, fromDate, toDate, userVoucherNo);
            PagingViewModel pagingViewModel = calcMasterAdjustmentPaging(tempList);
            List<AdjustmentViewModel.MasterAdjustmentViewModel> lstMasterAdjustment = getMasterAdjustmentByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterAdjustment = lstMasterAdjustment
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult RefreshAction(int userId)
        {
            List<AdjustmentViewModel.MasterAdjustmentViewModel> tempList = selectMasterAdjustment(userId, false);
            PagingViewModel pagingViewModel = calcMasterAdjustmentPaging(tempList);
            List<AdjustmentViewModel.MasterAdjustmentViewModel> lstMasterAdjustment = getMasterAdjustmentByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterAdjustment = lstMasterAdjustment
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult AdjustmentEditAction(int adjustmentId, string date, string voucherId, int locationId,string remark)
        {
            bool isRequestSuccess = true;

            if (Session["TranAdjustmentData"] != null)
            {
                List<TranAdjustmentModels> list = Session["TranAdjustmentData"] as List<TranAdjustmentModels>;
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                dt.Columns.Add(new DataColumn("AdjustTypeID", typeof(int)));
                
                for (int i = 0; i < list.Count; i++)
                {
                    dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].AdjustTypeID);
                }

                DateTime adjustDateTime = DateTime.Parse(date);
                SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateAdjustment, dataConnectorSQL.Connect());
                cmd.Parameters.AddWithValue("@AdjustmentID", adjustmentId);
                cmd.Parameters.AddWithValue("@AdjustDateTime", adjustDateTime);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@temptbl", dt);
                cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                cmd.Parameters.AddWithValue("@Remark", remark);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                dataConnectorSQL.Close();
                clearTranAdjustment();
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                IsRequestSuccess = isRequestSuccess
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Methods
        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                adjustmentViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
        }
        private void getAdjustType(bool isIncludeDefault)
        {
            if (isIncludeDefault) adjustmentViewModel.AdjustmentsType.Add(new SelectListItem { Text = "Adjust Type", Value = "0" });

            List<AdjustTypeModels> list = appData.selectAdjustType(getConnection());
            for(int i=0;i<list.Count;i++)
            {
                adjustmentViewModel.AdjustmentsType.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].AdjustTypeID) });
            }
        }
        private object getConnection()
        {
            object connection;
            if (Session[AppConstants.SQLConnection] == null)
                Session[AppConstants.SQLConnection] = dataConnectorSQL.Connect();

            connection = Session[AppConstants.SQLConnection];
            return connection;
        }
        private bool checkConnection()
        {
            if (Session[AppConstants.SQLConnection] != null) return true;
            else return false;
        }
        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.AdjustmentModule, userId, getConnection());
            return userVoucherNo;
        }
        private void getMainMenu()
        {
            adjustmentViewModel.ProductMenus.MainMenus = appData.selectMainMenu(getConnection());
        }
        private int getFirstMainMenuID()
        {
            int mainMenuId = 0;
            if (adjustmentViewModel.ProductMenus.MainMenus.Count() != 0)
            {
                MainMenuModels.MainMenuModel firstMainMenu = adjustmentViewModel.ProductMenus.MainMenus.First();
                mainMenuId = firstMainMenu.MainMenuID;
            }
            return mainMenuId;
        }
        private void getSubMenu(int mainMenuId)
        {
            adjustmentViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(), mainMenuId);
        }
        private int getFirstSubMenuID()
        {
            int subMenuId = 0;
            if (adjustmentViewModel.ProductMenus.SubMenus.Count() != 0)
            {
                SubMenuModels.SubMenuModel firstSubMenu = adjustmentViewModel.ProductMenus.SubMenus.First();
                subMenuId = firstSubMenu.SubMenuID;
            }
            return subMenuId;
        }
        private void getProduct(int subMenuId)
        {
            adjustmentViewModel.ProductMenus.Products = appData.selectProduct(getConnection(), subMenuId);
            Session["ProductData"] = adjustmentViewModel.ProductMenus.Products;
        }
        private List<UnitModels> getUnit()
        {
            List<UnitModels> list = new List<UnitModels>();
            if (Session["UnitData"] == null)
            {
                list = appData.selectUnit(getConnection());
                Session["UnitData"] = list;
            }
            else list = Session["UnitData"] as List<UnitModels>;
            return list;
        }
        private void clearTranAdjustment()
        {
            Session["TranAdjustmentData"] = null;
        }
        private List<AdjustmentViewModel.MasterAdjustmentViewModel> selectMasterAdjustment(int userId, bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]string userVoucherNo)
        {
            List<AdjustmentViewModel.MasterAdjustmentViewModel> tempList = new List<AdjustmentViewModel.MasterAdjustmentViewModel>();
            AdjustmentViewModel.MasterAdjustmentViewModel item = new AdjustmentViewModel.MasterAdjustmentViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterAdjustmentList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@IsSearch", isSearch);
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@UserVoucherNo", "");
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
            }

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new AdjustmentViewModel.MasterAdjustmentViewModel();
                item.AdjustmentID = Convert.ToInt32(reader["AdjustmentID"]);
                item.AdjustDateTime = Convert.ToString(reader["AdjustDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.Remark = Convert.ToString(reader["Remark"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterAdjustmentData"] = tempList;  // for paging

            return tempList;
        }
        private PagingViewModel calcMasterAdjustmentPaging(List<AdjustmentViewModel.MasterAdjustmentViewModel> tempList, [Optional]int currentPage)
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
        private List<AdjustmentViewModel.MasterAdjustmentViewModel> getMasterAdjustmentByPaging(List<AdjustmentViewModel.MasterAdjustmentViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<AdjustmentViewModel.MasterAdjustmentViewModel> list = new List<AdjustmentViewModel.MasterAdjustmentViewModel>();
            AdjustmentViewModel.MasterAdjustmentViewModel item = new AdjustmentViewModel.MasterAdjustmentViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new AdjustmentViewModel.MasterAdjustmentViewModel();
                item.AdjustmentID = tempList[page].AdjustmentID;
                item.AdjustDateTime = tempList[page].AdjustDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.Remark = tempList[page].Remark;
                list.Add(item);
            }
            return list;
        }
        private MasterAdjustmentViewModel selectMasterAdjustment(int adjustmentId)
        {
            MasterAdjustmentViewModel item = new MasterAdjustmentViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterAdjustmentByAdjustmentID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AdjustmentID", adjustmentId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.MasterAdjustmentModel.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.MasterAdjustmentModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.MasterAdjustmentModel.Remark = Convert.ToString(reader["Remark"]);
                item.LocationName = Convert.ToString(reader["LocationName"]);            
                item.MasterAdjustmentModel.AdjustDateTime = Convert.ToString(reader["Date"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                item.MasterAdjustmentModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                
            }
            reader.Close();

            return item;
        }
        private List<TranAdjustmentModels> selectTranAdjustmentByAdjustmentID(int adjustmentId)
        {
            List<TranAdjustmentModels> list = new List<TranAdjustmentModels>();
            TranAdjustmentModels item = new TranAdjustmentModels();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranAdjustmentByAdjustmentID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AdjustmentID", adjustmentId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranAdjustmentModels();
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);                
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.AdjustTypeKeyword = Convert.ToString(reader["AdjustTypeName"]);
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.AdjustTypeID = Convert.ToInt32(reader["AdjustTypeID"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        #endregion
    }
}
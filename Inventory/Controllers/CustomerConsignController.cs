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
    public class CustomerConsignController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        AppData appData = new AppData();
        CustomerConsignViewModel customerConsignViewModel = new CustomerConsignViewModel();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();
        TextQuery textQuery = new TextQuery();
        public ActionResult CustomerConsign(int userId,int? customerConsignId)
        {
            if (checkConnection())
            {
                getLocation();
                getCustomer(false);
                getDivision(true);
                getSalePerson(true);
                getMainMenu();
                getSubMenu(getFirstMainMenuID());
                getProduct(getFirstSubMenuID());
                clearTranCustomerConsign();
                if (customerConsignId!=null)
                {
                    ViewBag.IsEdit = true;
                    MasterCustomerConsignViewModel data = selectMasterCustomerConsign((int)customerConsignId);
                    List<TranCustomerConsignModels> lstTranCustomerConsign = selectTranCustomerConsignByCustomerConsignID((int)customerConsignId);

                    Session["TranCustomerConsignData"] = lstTranCustomerConsign;

                    ViewBag.UserVoucherNo = data.MasterCustomerConsignModel.UserVoucherNo;
                    DateTime date = setting.convertStringToDate(data.MasterCustomerConsignModel.ConsignDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterCustomerConsignModel.VoucherID;
                    ViewBag.LocationID = data.MasterCustomerConsignModel.LocationID;
                    ViewBag.CustomerConsignID = customerConsignId;
                    ViewBag.Remark = data.MasterCustomerConsignModel.Remark;
                    if (data.MasterCustomerConsignModel.DueDateTime != "")
                    {
                        DateTime dueDate = setting.convertStringToDate(data.MasterCustomerConsignModel.DueDateTime);
                        ViewBag.DueDate = setting.convertDateToString(dueDate);
                    }
                    else { ViewBag.DueDate = data.MasterCustomerConsignModel.DueDateTime; }
                    ViewBag.CustomerID = data.MasterCustomerConsignModel.CustomerID;
                    ViewBag.DivisionID = data.MasterCustomerConsignModel.DivisionID;
                    ViewBag.SalePersonID = data.MasterCustomerConsignModel.SalePersonID;
                }
                else
                {
                    ViewBag.UserVoucherNo = getUserVoucherNo(userId); // new Customer Consign
                }
                return View(customerConsignViewModel);
            }           
            return RedirectToAction("Login", "User");
        }

        public ActionResult ListCustomerConsign(int userId)
        {
            if (checkConnection())
            {
                getCustomer(true);
                List<CustomerConsignViewModel.MasterCustomerConsignViewModel> tempList = selectMasterCustomerConsign(userId, false);
                PagingViewModel pagingViewModel = calcMasterCustomerConsignPaging(tempList);
                List<CustomerConsignViewModel.MasterCustomerConsignViewModel> lstMasterCustomerConsign = getMasterCustomerConsignByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstMasterCustomerConsign"] = lstMasterCustomerConsign;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
                return View(customerConsignViewModel);
            }
            return RedirectToAction("Login", "User");
        }
        #region Customer Consign Action
        [HttpGet]
        public JsonResult GetProductByCodeAction(string productCode, bool isMultiUnit)
        {
            ProductModels.ProductModel data = new ProductModels.ProductModel();
            string productName = "";
            int productId = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
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
        public JsonResult TranCustomerConsignAddEditAction(int productId, string productCode, string productName, int quantity, int? unitId, string unitKeyword, bool isEdit, int? number)
        {
            List<TranCustomerConsignModels> lstTranCustomerConsign = new List<TranCustomerConsignModels>();
            TranCustomerConsignModels data = new TranCustomerConsignModels();
            bool isRequestSuccess = true;

            data.Number = number;
            data.ProductID = productId;
            data.ProductCode = productCode;
            data.Quantity = quantity;
            data.UnitID = unitId;
            data.ProductName = productName;
            if (unitKeyword != null) data.UnitKeyword = unitKeyword;
            else data.UnitKeyword = "";

            if (!isEdit)
            {
                if (Session["TranCustomerConsignData"] != null)
                {
                    lstTranCustomerConsign = Session["TranCustomerConsignData"] as List<TranCustomerConsignModels>;
                    lstTranCustomerConsign.Add(data);
                }
                else lstTranCustomerConsign.Add(data);
            }
            else
            {
                if (Session["TranCustomerConsignData"] != null)
                {
                    lstTranCustomerConsign = Session["TranCustomerConsignData"] as List<TranCustomerConsignModels>;
                    int index = (int)number - 1;
                    lstTranCustomerConsign[index] = data;
                }
                else isRequestSuccess = false;
            }
            Session["TranCustomerConsignData"] = lstTranCustomerConsign;

            var jsonResult = new
            {
                LstTranCustomerConsign = lstTranCustomerConsign,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult TranCustomerConsignDeleteAction(int number)
        {
            List<TranCustomerConsignModels> lstTranCustomerConsign = new List<TranCustomerConsignModels>();
            bool isRequestSuccess = true;

            if (Session["TranCustomerConsignData"] != null)
            {
                lstTranCustomerConsign = Session["TranCustomerConsignData"] as List<TranCustomerConsignModels>;
                lstTranCustomerConsign.RemoveAt(number - 1);
            }
            else isRequestSuccess = false;
            Session["TranCustomerConsignData"] = lstTranCustomerConsign;

            var jsonResult = new
            {
                LstTranCustomerConsign = lstTranCustomerConsign,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult PrepareToEditTranCustomerConsignAction(int number, bool isMultiUnit)
        {
            List<TranCustomerConsignModels> lstTranCustomerConsign = new List<TranCustomerConsignModels>();
            TranCustomerConsignModels data = new TranCustomerConsignModels();
            string productCode = "", productName = "";
            int productId = 0, quantity = 0;
            int? unitId = 0;
            List<UnitModels> lstUnit = new List<UnitModels>();
            bool isRequestSuccess = false;

            if (Session["TranCustomerConsignData"] != null)
            {
                lstTranCustomerConsign = Session["TranCustomerConsignData"] as List<TranCustomerConsignModels>;

                if (lstTranCustomerConsign.Count() != 0)
                {
                    data = lstTranCustomerConsign[number - 1];
                    if (data != null)
                    {
                        productId = data.ProductID;
                        productCode = data.ProductCode;
                        productName = data.ProductName;
                        quantity = data.Quantity;
                        unitId = data.UnitID;
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
                LstUnit = lstUnit,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CustomerConsignSubmitAction(string userVoucherNo, string date, string voucherId, int locationId,int customerId,
              int divisionId, int salePersonId, string dueDate , string remark, int userId)
        {
            bool isRequestSuccess = true;

            if (Session["TranCustomerConsignData"] != null)
            {
                List<TranCustomerConsignModels> list = Session["TranCustomerConsignData"] as List<TranCustomerConsignModels>;
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                dt.Columns.Add(new DataColumn("UnitID", typeof(int)));

                for (int i = 0; i < list.Count; i++)
                {
                    dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID);
                }

                DateTime consignDateTime = DateTime.Parse(date);

                SqlCommand cmd = new SqlCommand(Procedure.PrcInsertCustomerConsign, dataConnectorSQL.Connect());
                cmd.Parameters.AddWithValue("@ConsignDateTime", consignDateTime);
                cmd.Parameters.AddWithValue("@DueDateTime", dueDate);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@DivisionID", divisionId);
                cmd.Parameters.AddWithValue("@ClientID", salePersonId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@Remark", remark);
                cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.CustomerConsignModule);
                cmd.Parameters.AddWithValue("@temptbl", dt);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) userVoucherNo = Convert.ToString(reader[0]);
                reader.Close();
                dataConnectorSQL.Close();
                clearTranCustomerConsign();
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
        public JsonResult SearchAction(int userId, DateTime fromDate, DateTime toDate, string userVoucherNo,int customerId)
        {
            List<CustomerConsignViewModel.MasterCustomerConsignViewModel> tempList = selectMasterCustomerConsign(userId, true, fromDate, toDate, userVoucherNo,customerId);
            PagingViewModel pagingViewModel = calcMasterCustomerConsignPaging(tempList);
            List<CustomerConsignViewModel.MasterCustomerConsignViewModel> lstMasterCustomerConsign = getMasterCustomerConsignByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterCustomerConsign = lstMasterCustomerConsign
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RefreshAction(int userId)
        {
            List<CustomerConsignViewModel.MasterCustomerConsignViewModel> tempList = selectMasterCustomerConsign(userId, false);
            PagingViewModel pagingViewModel = calcMasterCustomerConsignPaging(tempList);
            List<CustomerConsignViewModel.MasterCustomerConsignViewModel> lstMasterCustomerConsign = getMasterCustomerConsignByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterCustomerConsign = lstMasterCustomerConsign
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ViewAction(int customerConsignId)
        {
            MasterCustomerConsignViewModel item = selectMasterCustomerConsign(customerConsignId);
            List<TranCustomerConsignModels> lstTranCustomerConsign = selectTranCustomerConsignByCustomerConsignID(customerConsignId);

            var jsonResult = new
            {
                LstTranCustomerConsign = lstTranCustomerConsign,
                UserVoucherNo = item.MasterCustomerConsignModel.UserVoucherNo,
                VoucherID = item.MasterCustomerConsignModel.VoucherID,
                Remark = item.MasterCustomerConsignModel.Remark,             
                ConsignDateTime = item.MasterCustomerConsignModel.ConsignDateTime,
                DueDateTime = item.MasterCustomerConsignModel.DueDateTime,
                UserName = item.UserName,
                LocationName = item.LocationName,
                CustomerName=item.CustomerName,
                DivisionName=item.DivisionName,
                SalePersonName=item.SalePersonName
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAction(int customerConsignId)
        {
            bool isRequestSuccess = true;
            int totalPageNum = 0;

            if (Session["MasterCustomerConsignData"] != null)
            {
                SqlCommand cmd = new SqlCommand(textQuery.deleteCustomerConsignQuery(customerConsignId), (SqlConnection)getConnection());
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                List<CustomerConsignViewModel.MasterCustomerConsignViewModel> lstMasterCustomerConsign = Session["MasterCustomerConsignData"] as List<CustomerConsignViewModel.MasterCustomerConsignViewModel>;
                int index = lstMasterCustomerConsign.FindIndex(x => x.CustomerConsignID == customerConsignId);
                lstMasterCustomerConsign.RemoveAt(index);

                if (lstMasterCustomerConsign.Count > paging.eachItemCount)
                {
                    totalPageNum = lstMasterCustomerConsign.Count / paging.eachItemCount;
                    paging.lastItemCount = lstMasterCustomerConsign.Count % paging.eachItemCount;
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
        public JsonResult CustomerConsignPagingAction(int currentPage)
        {
            bool isRequestSuccess = true;
            List<CustomerConsignViewModel.MasterCustomerConsignViewModel> lstMasterCustomerConsign = new List<CustomerConsignViewModel.MasterCustomerConsignViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["MasterCustomerConsignData"] != null)
            {
                List<CustomerConsignViewModel.MasterCustomerConsignViewModel> tempList = Session["MasterCustomerConsignData"] as List<CustomerConsignViewModel.MasterCustomerConsignViewModel>;
                pagingViewModel = calcMasterCustomerConsignPaging(tempList, currentPage);
                lstMasterCustomerConsign = getMasterCustomerConsignByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                LstMasterCustomerConsign = lstMasterCustomerConsign,
                TotalPage = pagingViewModel.TotalPageNum,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CustomerConsignEditAction(int customerConsignId, string date,string dueDate ,string voucherId, int locationId,int customerId,int divisionId,int salepersonId ,string remark)
        {
            bool isRequestSuccess = true;

            if (Session["TranCustomerConsignData"] != null)
            {
                List<TranCustomerConsignModels> list = Session["TranCustomerConsignData"] as List<TranCustomerConsignModels>;
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                
                for (int i = 0; i < list.Count; i++)
                {
                    dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID);
                }
                DateTime consignDateTime = DateTime.Parse(date);               
                SqlCommand cmd = new SqlCommand(Procedure.PrcUpdateCustomerConsign, dataConnectorSQL.Connect());
                cmd.Parameters.AddWithValue("@CustomerConsignID", customerConsignId);
                cmd.Parameters.AddWithValue("@ConsignDateTime", consignDateTime);
                cmd.Parameters.AddWithValue("@DueDateTime", dueDate);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@DivisionID", divisionId);
                cmd.Parameters.AddWithValue("@ClientID", salepersonId);
                cmd.Parameters.AddWithValue("@temptbl", dt);
                cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                cmd.Parameters.AddWithValue("@Remark", remark);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                dataConnectorSQL.Close();
                clearTranCustomerConsign();
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                IsRequestSuccess = isRequestSuccess
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CancelAction()
        {
            Session["TranCustomerConsignData"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }      

        #endregion

        #region Methods
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
        private string getUserVoucherNo(int userId)
        {
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.CustomerConsignModule, userId, getConnection());
            return userVoucherNo;
        }
        private void clearTranCustomerConsign()
        {
            Session["TranCustomerConsignData"] = null;
        }
        private void getCustomer(bool isIncludeDefault)
        {
            if (isIncludeDefault) customerConsignViewModel.Customers.Add(new SelectListItem { Text = AppConstants.AllCustomer, Value = "0" });

            List<CustomerModels.CustomerModel> list = appData.selectCustomer(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                customerConsignViewModel.Customers.Add(new SelectListItem { Text = list[i].CustomerName, Value = Convert.ToString(list[i].CustomerID) });
            }
        }
        private void getSalePerson(bool isIncludeDefault)
        {
            if (isIncludeDefault) customerConsignViewModel.SalePersons.Add(new SelectListItem { Text = AppConstants.SalePerson, Value = "0" });

            List<ClientModels> list = appData.selectClientSalePerson(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                customerConsignViewModel.SalePersons.Add(new SelectListItem { Text = list[i].ClientName, Value = Convert.ToString(list[i].ClientID) });
            }
        }

        private void getDivision(bool isIncludeDefault)
        {
            if (isIncludeDefault) customerConsignViewModel.Divisions.Add(new SelectListItem { Text = AppConstants.Division, Value = "0" });

            List<DivisionModels> list = appData.selectDivision(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                customerConsignViewModel.Divisions.Add(new SelectListItem { Text = list[i].DivisionName, Value = Convert.ToString(list[i].DivisionID) });
            }
        }
        private void getLocation()
        {
            List<LocationModels.LocationModel> list = appData.selectLocation(getConnection());
            for (int i = 0; i < list.Count; i++)
            {
                customerConsignViewModel.Locations.Add(new SelectListItem { Text = list[i].ShortName, Value = Convert.ToString(list[i].LocationID) });
            }
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
        private void getMainMenu()
        {
            customerConsignViewModel.ProductMenus.MainMenus = appData.selectMainMenu(getConnection());
        }
        private int getFirstMainMenuID()
        {
            int mainMenuId = 0;
            if (customerConsignViewModel.ProductMenus.MainMenus.Count() != 0)
            {
                MainMenuModels.MainMenuModel firstMainMenu = customerConsignViewModel.ProductMenus.MainMenus.First();
                mainMenuId = firstMainMenu.MainMenuID;
            }
            return mainMenuId;
        }
        private void getSubMenu(int mainMenuId)
        {
            customerConsignViewModel.ProductMenus.SubMenus = appData.selectSubMenu(getConnection(), mainMenuId);
        }
        private int getFirstSubMenuID()
        {
            int subMenuId = 0;
            if (customerConsignViewModel.ProductMenus.SubMenus.Count() != 0)
            {
                SubMenuModels.SubMenuModel firstSubMenu = customerConsignViewModel.ProductMenus.SubMenus.First();
                subMenuId = firstSubMenu.SubMenuID;
            }
            return subMenuId;
        }
        private void getProduct(int subMenuId)
        {
            customerConsignViewModel.ProductMenus.Products = appData.selectProduct(getConnection(), subMenuId);
            Session["ProductData"] = customerConsignViewModel.ProductMenus.Products;
        }
        private List<CustomerConsignViewModel.MasterCustomerConsignViewModel> selectMasterCustomerConsign(int userId, bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]string userVoucherNo,[Optional]int customerId)
        {
            List<CustomerConsignViewModel.MasterCustomerConsignViewModel> tempList = new List<CustomerConsignViewModel.MasterCustomerConsignViewModel>();
            CustomerConsignViewModel.MasterCustomerConsignViewModel item = new CustomerConsignViewModel.MasterCustomerConsignViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterCustomerConsignList, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@IsSearch", isSearch);
            if (!isSearch)
            {
                cmd.Parameters.AddWithValue("@FromDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@ToDate", setting.getLocalDate());
                cmd.Parameters.AddWithValue("@UserVoucherNo", "");
                cmd.Parameters.AddWithValue("@CustomerID", "");
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
            }

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new CustomerConsignViewModel.MasterCustomerConsignViewModel();
                item.CustomerConsignID = Convert.ToInt32(reader["CustomerConsignID"]);
                item.ConsignDateTime = Convert.ToString(reader["ConsignDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterCustomerConsignData"] = tempList;  // for paging

            return tempList;
        }
        private PagingViewModel calcMasterCustomerConsignPaging(List<CustomerConsignViewModel.MasterCustomerConsignViewModel> tempList, [Optional]int currentPage)
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
        private List<CustomerConsignViewModel.MasterCustomerConsignViewModel> getMasterCustomerConsignByPaging(List<CustomerConsignViewModel.MasterCustomerConsignViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<CustomerConsignViewModel.MasterCustomerConsignViewModel> list = new List<CustomerConsignViewModel.MasterCustomerConsignViewModel>();
            CustomerConsignViewModel.MasterCustomerConsignViewModel item = new CustomerConsignViewModel.MasterCustomerConsignViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new CustomerConsignViewModel.MasterCustomerConsignViewModel();
                item.CustomerConsignID = tempList[page].CustomerConsignID;
                item.ConsignDateTime = tempList[page].ConsignDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.CustomerName = tempList[page].CustomerName;
                list.Add(item);
            }
            return list;
        }
        private MasterCustomerConsignViewModel selectMasterCustomerConsign(int customerConsignId)
        {
            MasterCustomerConsignViewModel item = new MasterCustomerConsignViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterCustomerConsignByCustomerConsignID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerConsignID", customerConsignId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.MasterCustomerConsignModel.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.MasterCustomerConsignModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.MasterCustomerConsignModel.Remark = Convert.ToString(reader["Remark"]);              
                item.MasterCustomerConsignModel.ConsignDateTime = Convert.ToString(reader["Date"]);
                item.MasterCustomerConsignModel.DueDateTime = Convert.ToString(reader["DueDate"]);
                item.MasterCustomerConsignModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.MasterCustomerConsignModel.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                item.MasterCustomerConsignModel.DivisionID = Convert.ToInt32(reader["DivisionID"]);
                item.MasterCustomerConsignModel.SalePersonID = Convert.ToInt32(reader["ClientID"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                item.LocationName = Convert.ToString(reader["LocationName"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                item.DivisionName = Convert.ToString(reader["DivisionName"]);
                item.SalePersonName = Convert.ToString(reader["ClientName"]);

            }
            reader.Close();

            return item;
        }

        private List<TranCustomerConsignModels> selectTranCustomerConsignByCustomerConsignID(int customerConsignId)
        {
            List<TranCustomerConsignModels> list = new List<TranCustomerConsignModels>();
            TranCustomerConsignModels item = new TranCustomerConsignModels();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranCustomerConsignByCustomerConsignID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerConsignID", customerConsignId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranCustomerConsignModels();
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }


        #endregion
    }
}
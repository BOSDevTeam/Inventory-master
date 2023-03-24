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
using Inventory.Filters;

namespace Inventory.Controllers
{
    public class PurchaseReturnController : MyController
    {
        DataConnectorSQL dataConnectorSQL = new DataConnectorSQL();
        AppData appData = new AppData();
        AppSetting setting = new AppSetting();
        AppSetting.Paging paging = new AppSetting.Paging();
        TextQuery textQuery = new TextQuery();
        PurchaseReturnViewModel purchaseReturnViewModel = new PurchaseReturnViewModel();
        [SessionTimeoutAttribute]
        public ActionResult PurchaseReturn(int userId, int? purchaseReturnId)
        {
            if (checkConnection())
            {
                clearTranPurchaseReturn();
                if (purchaseReturnId != null)//edit mode
                {
                    ViewBag.IsEdit = true;
                    MasterPurchaseReturnViewModel data = selectMasterPurchaseReturn((int)purchaseReturnId);
                    List<TranPurchaseReturnModels> lstTranPurchaseReturn = selectTranPurchaseReturnByPurchaseReturnID((int)purchaseReturnId);
                    if (data.MasterPurchaseReturnModel.ReturnVoucherNo != null)
                    {
                        //PurchaseReturn By VoucherNo
                        MasterPurchaseViewModel item = new MasterPurchaseViewModel();
                        List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
                        item = selectMasterPurchase(data.MasterPurchaseReturnModel.ReturnVoucherNo);
                        lstTranPurchase = selectTranPurchaseByPurchaseID(item.MasterPurchaseModel.PurchaseID);

                        for (int i = 0; i < lstTranPurchaseReturn.Count; i++)
                        {
                            var purchase = lstTranPurchase.Where(m => m.ProductID == lstTranPurchaseReturn[i].ProductID && m.UnitID == lstTranPurchaseReturn[i].UnitID && m.CurrencyID == lstTranPurchaseReturn[i].CurrencyID && m.DiscountPercent == lstTranPurchaseReturn[i].DiscountPercent && m.PurchasePrice == lstTranPurchaseReturn[i].PurchasePrice).FirstOrDefault();
                            if (purchase != null)
                            {
                                lstTranPurchaseReturn[i].Number = purchase.ID;
                                lstTranPurchaseReturn[i].MaxQuantity = purchase.Quantity;
                            }
                            else
                            {
                                int j = i;
                                if (lstTranPurchase.Where(m => m.ID == i).ToList().Count > 0)
                                {
                                    j = i + lstTranPurchase.LastOrDefault().ID;
                                }
                                lstTranPurchaseReturn[i].Number = j;
                            }
                        }
                        Session["TranPurchaseData"] = lstTranPurchase;
                        Session["MasterPurchaseData"] = item;
                        Session["TranPurchaseReturnData"] = lstTranPurchaseReturn;
                        ViewBag.TotalItem = lstTranPurchase.Count();
                        ViewBag.TotalQuantity = lstTranPurchase.Sum(m => m.Quantity);
                    }
                                                                                                                 
                    Session["TranPurchaseReturnByProductData"] = lstTranPurchaseReturn;
                    Session["MasterPurchaseReturnData"] = null;
                    Session["MasterPurchaseReturnData"] = data;                   
                    ViewBag.UserVoucherNo = data.MasterPurchaseReturnModel.UserVoucherNo;
                    DateTime date = setting.convertStringToDate(data.MasterPurchaseReturnModel.PurchaseReturnDateTime);
                    ViewBag.Date = setting.convertDateToString(date);
                    ViewBag.VoucherID = data.MasterPurchaseReturnModel.VoucherID;
                    ViewBag.Total = data.MasterPurchaseReturnModel.Total;
                    ViewBag.PurchaseReturnID = purchaseReturnId;
                    ViewBag.ReturnVoucherNo = data.MasterPurchaseReturnModel.ReturnVoucherNo;                   
                    ViewBag.Remark = data.MasterPurchaseReturnModel.Remark;
                }
                else //new Mode
                {
                    ViewBag.UserVoucherNo = getUserVoucherNo(userId);
                }
                return View(purchaseReturnViewModel);
            }
            return RedirectToAction("Login", "User");           
        }

        [SessionTimeoutAttribute]
        public ActionResult ListPurchaseReturn(int userId)
        {
            if (checkConnection())
            {               
                List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> tempList = selectMasterPurchaseReturn(userId, false);
                PagingViewModel pagingViewModel = calcMasterPurchaseReturnPaging(tempList);
                List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> lstMasterPurchaseReturn = getMasterPurchaseReturnByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
                ViewData["LstMasterPurchaseReturn"] = lstMasterPurchaseReturn;
                ViewBag.TotalPageNum = pagingViewModel.TotalPageNum;
                ViewBag.CurrentPage = pagingViewModel.CurrentPage;
                return View(purchaseReturnViewModel);
            }
            return RedirectToAction("Login", "User");

        }


        #region Action
        [HttpGet]
        public JsonResult GetPurchaseItemsByPurchaseVoucherNoAction(string purchaseVoucherNo)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            MasterPurchaseViewModel item = new MasterPurchaseViewModel();
            List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            bool isExistPurchase = true, isVoucherFOC = false ;            
            try
            {
                item = selectMasterPurchase(purchaseVoucherNo);
                if (item.MasterPurchaseModel.PurchaseID >0)
                {
                    if (!item.MasterPurchaseModel.IsVouFOC)
                    {
                        lstTranPurchase = selectTranPurchaseByPurchaseID(item.MasterPurchaseModel.PurchaseID);
                        Session["TranPurchaseData"] = null;
                        Session["TranPurchaseData"] = lstTranPurchase;
                        Session["MasterPurchaseData"] = null;
                        Session["MasterPurchaseData"] = item;
                        Session["TranPurchaseReturnData"] = null;                                               
                    }
                    else
                    {
                        resultDefaultData.Message = AppConstants.Message.NoReturnFOCVoucher;
                        isVoucherFOC = true;
                    }  
                }
                else
                {
                    isExistPurchase = false;
                }
                resultDefaultData.IsRequestSuccess = true;
            }
            catch(Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }
            var jsonResult = new
            {
                LstTranPurchase = lstTranPurchase,                            
                TotalPurchaseItem = lstTranPurchase.Count,
                TotalPurchaseItemQty = lstTranPurchase.Sum(m => m.Quantity),
                IsExistPurchase=isExistPurchase,
                IsVoucherFOC=isVoucherFOC,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }      

        [HttpGet]
        public JsonResult PrepareToEditTranPurchaseAction(int number)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            TranPurchaseModels data = new TranPurchaseModels();
            string productName = "",unitKeyword="",currencyKeyword="";
            int id=0,productId = 0, quantity = 0, price = 0, disPercent = 0;
            int? unitId = 0, currencyId = 0;                       

            if (Session["TranPurchaseData"] != null)
            {
                lstTranPurchase = Session["TranPurchaseData"] as List<TranPurchaseModels>;

                if (lstTranPurchase.Count() != 0)
                {
                    data = lstTranPurchase.Where(m => m.ID == number).FirstOrDefault();
                    if (data != null)
                    {
                        id = data.ID;                       
                        productId = data.ProductID;                      
                        productName = data.ProductName;
                        quantity = data.Quantity;
                        //maxQuantity = data.Quantity;                      
                        price = data.PurchasePrice;
                        unitId = data.UnitID;
                        currencyId = data.CurrencyID;
                        disPercent = data.DiscountPercent;
                        unitKeyword = data.UnitKeyword;
                        currencyKeyword = data.CurrencyKeyword;
                        resultDefaultData.IsRequestSuccess = true;
                    }
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ID=id,               
                ProductID = productId,               
                ProductName = productName,
                Quantity = quantity,
                //MaxQuantity=maxQuantity,
                Price = price,
                UnitID = unitId,
                CurrencyID = currencyId,
                DisPercent = disPercent,
                UnitKeyword=unitKeyword,
                CurrencyKeyword=currencyKeyword,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranPurchaseReturnAddEditAction(int ID,int productId, string productName, int quantity,int maxQuantity ,int price, int disPercent, int? unitId, string unitKeyword, int? currencyId, string currencyKeyword, bool isEdit, int? number)
        {
            //List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            List<TranPurchaseReturnModels> lstTranPurchaseReturn = new List<TranPurchaseReturnModels>();           
            TranPurchaseReturnModels data = new TranPurchaseReturnModels();            
            int subtotal = 0, discount;
            bool isRequestSuccess = true;

            data.Number = number;
            data.ID = ID;
            data.ProductID = productId;      
            data.ProductName = productName;
            data.Quantity = quantity;
            data.MaxQuantity = maxQuantity;
            data.PurchasePrice = price;
            data.DiscountPercent = disPercent;
            data.UnitID = unitId;
            data.CurrencyID = currencyId;           
            if (unitKeyword != null) data.UnitKeyword = unitKeyword;
            else data.UnitKeyword = "";
            if (currencyKeyword != null) data.CurrencyKeyword = currencyKeyword;        
            else data.CurrencyKeyword = "";
            discount = ((price * quantity) * disPercent) / 100;
            data.Discount = discount;
            data.Amount = (quantity * price) - discount;
            if(price<=0 && discount<=0&& data.Amount<=0 )
            {
                data.IsFOC = true;
            }
                        
            if (!isEdit)
            {
                if (Session["TranPurchaseReturnData"] != null)
                {
                    lstTranPurchaseReturn = Session["TranPurchaseReturnData"] as List<TranPurchaseReturnModels>;
                    if(lstTranPurchaseReturn.Where(m=>m.Number==number).Count()>0)
                    {
                        foreach(var purchaseReturn in lstTranPurchaseReturn.Where(m=>m.Number==number))
                        {
                            purchaseReturn.Quantity = data.Quantity;
                            purchaseReturn.Discount = data.Discount;
                            purchaseReturn.Amount = data.Amount;
                        }
                    }
                    else
                    {
                        lstTranPurchaseReturn.Add(data);
                    }                                                            
                }
                else lstTranPurchaseReturn.Add(data);
              
            }
            else
            {   
                if (Session["TranPurchaseReturnData"] != null)
                {
                    lstTranPurchaseReturn = Session["TranPurchaseReturnData"] as List<TranPurchaseReturnModels>;
                       
                    foreach(var purchaseReturn in lstTranPurchaseReturn.Where(m => m.Number == number))
                    {
                        purchaseReturn.Quantity = data.Quantity;
                        purchaseReturn.Discount = data.Discount;
                        purchaseReturn.Amount = data.Amount;
                    }                             
                }
                else isRequestSuccess = false;
            }

            for (int i = 0; i < lstTranPurchaseReturn.Count(); i++)
            {
                subtotal += lstTranPurchaseReturn[i].Amount;                
            }

            Session["TranPurchaseReturnData"] = lstTranPurchaseReturn;                       

            var jsonResult = new
            {
                LstTranPurchaseReturn = lstTranPurchaseReturn,
                SubTotal = subtotal,                                 
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult TranPurchaseReturnDeleteAction(int number)
        {
            List<TranPurchaseReturnModels> lstTranPurchaseReturn = new List<TranPurchaseReturnModels>();
            //List<TranPurchaseModels> lstTranPurchase = new List<TranPurchaseModels>();
            int subtotal = 0;
            bool isRequestSuccess = true;
            //lstTranPurchase = Session["TranPurchaseData"] as List<TranPurchaseModels>;
            if (Session["TranPurchaseReturnData"] != null)
            {
                lstTranPurchaseReturn = Session["TranPurchaseReturnData"] as List<TranPurchaseReturnModels>;
                
                //if (lstTranPurchase.Where(m => m.ID == number).Count() > 0)
                //{
                //    if(isUpdate)
                //    {
                //        foreach (var item in lstTranPurchase.Where(m => m.ID == number))
                //        {
                //            var data = lstTranPurchaseReturn.Where(m => m.Number == item.ID).FirstOrDefault();
                //            item.Quantity = data.MaxQuantity;
                //            item.Discount = (data.PurchasePrice*data.MaxQuantity*data.DiscountPercent)/100;
                //            item.Amount = (data.PurchasePrice*data.MaxQuantity)-item.Discount;                          
                //        }
                //    }
                //}
                //else
                //{
                //    TranPurchaseModels item = new TranPurchaseModels();
                //    var purchaseReturn = lstTranPurchaseReturn.Where(m => m.Number == number).FirstOrDefault();
                //    item.ID = Convert.ToInt32(purchaseReturn.Number);
                //    item.ProductID = purchaseReturn.ProductID;
                //    item.ProductName = purchaseReturn.ProductName;
                //    item.PurchasePrice = purchaseReturn.PurchasePrice;
                //    item.Quantity = purchaseReturn.Quantity;
                //    item.MaxQuantity = purchaseReturn.MaxQuantity;
                //    item.Discount = purchaseReturn.Discount;
                //    item.CurrencyID = purchaseReturn.CurrencyID;
                //    item.CurrencyKeyword = purchaseReturn.CurrencyKeyword;
                //    item.UnitID = purchaseReturn.UnitID;
                //    item.UnitKeyword = purchaseReturn.UnitKeyword;
                //    item.IsFOC = purchaseReturn.IsFOC;
                //    item.DiscountPercent = purchaseReturn.DiscountPercent;
                //    lstTranPurchase.Add(item);
                //}

                lstTranPurchaseReturn.Remove(lstTranPurchaseReturn.Where(m=>m.Number==number).First());
                for (int i = 0; i < lstTranPurchaseReturn.Count(); i++)
                {
                    subtotal += lstTranPurchaseReturn[i].Amount;
                }
                Session["TranPurchaseReturnData"] = lstTranPurchaseReturn;
            }
            else isRequestSuccess = false;
                       
            //Session["TranPurchaseData"] = lstTranPurchase;
            var jsonResult = new
            {
                LstTranPurchaseReturn = lstTranPurchaseReturn,
                SubTotal = subtotal,                 
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PrepareToEditTranPurchaseReturnAction(int number)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<TranPurchaseReturnModels> lstTranPurchaseReturn = new List<TranPurchaseReturnModels>();
            TranPurchaseReturnModels data = new TranPurchaseReturnModels();
            string productName = "", unitKeyword = "", currencyKeyword = "";
            int ID=0,productId = 0, quantity = 0, maxQuantity=0,price = 0, disPercent = 0;
            int? unitId = 0, currencyId = 0;

            if (Session["TranPurchaseReturnData"] != null)
            {
                lstTranPurchaseReturn = Session["TranPurchaseReturnData"] as List<TranPurchaseReturnModels>;

                if (lstTranPurchaseReturn.Count() != 0)
                {
                    data = lstTranPurchaseReturn.Where(m=>m.Number==number).First();
                    if (data != null)
                    {
                        ID = data.ID;
                        productId = data.ProductID;
                        productName = data.ProductName;
                        quantity = data.Quantity;
                        maxQuantity = data.MaxQuantity;
                        price = data.PurchasePrice;
                        unitId = data.UnitID;
                        currencyId = data.CurrencyID;
                        disPercent = data.DiscountPercent;
                        unitKeyword = data.UnitKeyword;
                        currencyKeyword = data.CurrencyKeyword;
                        resultDefaultData.IsRequestSuccess = true;
                    }
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                ID=ID,
                ProductID = productId,
                ProductName = productName,
                Quantity = quantity,
                MaxQuantity = maxQuantity,
                Price = price,
                UnitID = unitId,
                CurrencyID = currencyId,
                DisPercent = disPercent,
                UnitKeyword = unitKeyword,
                CurrencyKeyword = currencyKeyword,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CancelAction()
        {           
            Session["TranPurchaseReturnData"] = null;
            var jsonResult = new
            {
                data="",              
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PurchaseReturnSubmitAction(string userVoucherNo, string date, string voucherId, string remark,int total, int userId,bool isPurchaseReturnByVoucher)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranPurchaseReturnData"] != null || Session["TranPurchaseReturnByProductData"]!=null)
            {
                try
                {
                    List<TranPurchaseReturnModels> list = new List<TranPurchaseReturnModels>();
                    if (isPurchaseReturnByVoucher) list= Session["TranPurchaseReturnData"] as List<TranPurchaseReturnModels>;
                    else list = Session["TranPurchaseReturnByProductData"] as List<TranPurchaseReturnModels>;                    
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                    dt.Columns.Add(new DataColumn("PurchasePrice", typeof(int)));
                    dt.Columns.Add(new DataColumn("CurrencyID", typeof(int)));
                    dt.Columns.Add(new DataColumn("DiscountPercent", typeof(int)));
                    dt.Columns.Add(new DataColumn("Discount", typeof(int)));
                    dt.Columns.Add(new DataColumn("Amount", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsFOC", typeof(bool)));

                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].PurchasePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                    }
                    MasterPurchaseViewModel item = new MasterPurchaseViewModel();
                    if (isPurchaseReturnByVoucher) item = Session["MasterPurchaseData"] as MasterPurchaseViewModel;
                    DateTime purchaseReturnDateTime = DateTime.Parse(date);                                       
                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcInsertPurchaseReturn, setting.conn);
                    if (isPurchaseReturnByVoucher) {
                        cmd.Parameters.AddWithValue("@ReturnVoucherNo", item.MasterPurchaseModel.UserVoucherNo);
                        cmd.Parameters.AddWithValue("@LocationID", item.MasterPurchaseModel.LocationID);

                        cmd.Parameters.AddWithValue("@AccountCode", AppConstants.PurchaseReturnAccountCode);
                        cmd.Parameters.AddWithValue("@PaymentID", item.MasterPurchaseModel.PaymentID);
                        cmd.Parameters.AddWithValue("@SupplierID", item.MasterPurchaseModel.SupplierID);
                        cmd.Parameters.AddWithValue("@CurrencyID", item.MasterPurchaseModel.MCurrencyID);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ReturnVoucherNo", DBNull.Value);
                        cmd.Parameters.AddWithValue("@LocationID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@AccountCode", DBNull.Value);
                        cmd.Parameters.AddWithValue("@PaymentID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@SupplierID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@CurrencyID", DBNull.Value);
                    }                
                    cmd.Parameters.AddWithValue("@PurchaseReturnDateTime", purchaseReturnDateTime);
                    cmd.Parameters.AddWithValue("@UserVoucherNo", userVoucherNo);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@UserID", userId);                                       
                    cmd.Parameters.AddWithValue("@Remark", remark);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@ModuleCode", AppConstants.PurchaseReturnModule);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = setting.conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) userVoucherNo = Convert.ToString(reader[0]);
                    reader.Close();
                    setting.conn.Close();
                    clearTranPurchaseReturn();
                    resultDefaultData.IsRequestSuccess = true;
                    resultDefaultData.Message = AppConstants.Message.SaveSuccess;
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

        [HttpGet]
        public JsonResult SearchAction(int userId, DateTime fromDate, DateTime toDate, string userVoucherNo)
        {
            List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> tempList = selectMasterPurchaseReturn(userId, true, fromDate, toDate, userVoucherNo);
            PagingViewModel pagingViewModel = calcMasterPurchaseReturnPaging(tempList);
            List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> lstMasterPurchaseReturn = getMasterPurchaseReturnByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterPurchaseReturn = lstMasterPurchaseReturn
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult RefreshAction(int userId)
        {
            List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> tempList = selectMasterPurchaseReturn(userId, false);
            PagingViewModel pagingViewModel = calcMasterPurchaseReturnPaging(tempList);
            List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> lstMasterPurchaseReturn = getMasterPurchaseReturnByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            var jsonResult = new
            {
                TotalPage = pagingViewModel.TotalPageNum,
                CurrentPage = pagingViewModel.CurrentPage,
                LstMasterPurchaseReturn = lstMasterPurchaseReturn
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PurchcaseReturnPagingAction(int currentPage)
        {
            bool isRequestSuccess = true;
            List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> lstMasterPurchaseReturn = new List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel>();
            PagingViewModel pagingViewModel = new PagingViewModel();

            if (Session["MasterPurchaseReturnData"] != null)
            {
                List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> tempList = Session["MasterPurchaseReturnData"] as List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel>;
                pagingViewModel = calcMasterPurchaseReturnPaging(tempList, currentPage);
                lstMasterPurchaseReturn = getMasterPurchaseReturnByPaging(tempList, pagingViewModel.StartItemIndex, pagingViewModel.EndItemIndex);
            }
            else isRequestSuccess = false;

            var jsonResult = new
            {
                LstMasterPurchaseReturn = lstMasterPurchaseReturn,
                TotalPage = pagingViewModel.TotalPageNum,
                IsRequestSuccess = isRequestSuccess
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ViewAction(int purchaseReturnId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            MasterPurchaseReturnViewModel item = new MasterPurchaseReturnViewModel();
            List<TranPurchaseReturnModels> lstTranPurchaseReturn = new List<TranPurchaseReturnModels>();           
            try
            {
                item = selectMasterPurchaseReturn(purchaseReturnId);
                lstTranPurchaseReturn = selectTranPurchaseReturnByPurchaseReturnID(purchaseReturnId);                
                resultDefaultData.IsRequestSuccess = true;
            }
            catch (Exception ex)
            {
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }

            var jsonResult = new
            {
                LstTranPurchaseReturn = lstTranPurchaseReturn,
                ReturnVoucherNo=item.MasterPurchaseReturnModel.ReturnVoucherNo,
                ReturnDateTime = item.MasterPurchaseReturnModel.PurchaseReturnDateTime,
                UserVoucherNo = item.MasterPurchaseReturnModel.UserVoucherNo,
                VoucherID = item.MasterPurchaseReturnModel.VoucherID,
                Remark = item.MasterPurchaseReturnModel.Remark,
                LocationName = item.LocationName,
                UserName = item.UserName,
                Total = item.MasterPurchaseReturnModel.Total,               
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAction(int purchaseReturnId)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            int totalPageNum = 0;

            if (Session["MasterPurchaseReturnData"] != null)
            {
                try
                {                   
                    SqlCommand cmd = new SqlCommand(textQuery.deletePurchaseReturnQuery(purchaseReturnId), (SqlConnection)getConnection());
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> lstMasterPurchaseReturn = Session["MasterPurchaseReturnData"] as List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel>;
                    int index = lstMasterPurchaseReturn.FindIndex(x => x.PurchaseReturnID == purchaseReturnId);
                    lstMasterPurchaseReturn.RemoveAt(index);

                    if (lstMasterPurchaseReturn.Count > paging.eachItemCount)
                    {
                        totalPageNum = lstMasterPurchaseReturn.Count / paging.eachItemCount;
                        paging.lastItemCount = lstMasterPurchaseReturn.Count % paging.eachItemCount;
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
        public JsonResult PurchaseReturnEditAction(int purchaseReturnId, string date, string voucherId, int total, string returnVoucherNo, string remark,bool isPurchaseReturnByVoucher)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            if (Session["TranPurchaseReturnData"] != null || Session["TranPurchaseReturnByProductData"]!=null)
            {
                try
                {
                    MasterPurchaseViewModel masterData = Session["MasterPurchaseData"] as MasterPurchaseViewModel;                 
                    
                    List<TranPurchaseReturnModels> list = new List<TranPurchaseReturnModels>();
                    if(isPurchaseReturnByVoucher) list = Session["TranPurchaseReturnData"] as List<TranPurchaseReturnModels>;
                    else list= Session["TranPurchaseReturnByProductData"] as List<TranPurchaseReturnModels>;
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("ProductID", typeof(int)));
                    dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
                    dt.Columns.Add(new DataColumn("UnitID", typeof(int)));
                    dt.Columns.Add(new DataColumn("PurPrice", typeof(int)));
                    dt.Columns.Add(new DataColumn("CurrencyID", typeof(int)));
                    dt.Columns.Add(new DataColumn("DiscountPercent", typeof(int)));
                    dt.Columns.Add(new DataColumn("Discount", typeof(int)));
                    dt.Columns.Add(new DataColumn("Amount", typeof(int)));
                    dt.Columns.Add(new DataColumn("IsFOC", typeof(bool)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].ProductID, list[i].Quantity, list[i].UnitID, list[i].PurchasePrice, list[i].CurrencyID, list[i].DiscountPercent, list[i].Discount, list[i].Amount, list[i].IsFOC);
                    }                   
                    
                    DateTime purchaseReturnDateTime = DateTime.Parse(date);
                    setting.conn.Open();
                    SqlCommand cmd = new SqlCommand(Procedure.PrcUpdatePurchaseReturn,setting.conn);
                    cmd.Connection = setting.conn;
                    cmd.Parameters.AddWithValue("@PurchaseReturnID", purchaseReturnId);
                    cmd.Parameters.AddWithValue("@PurchaseReturnDateTime", purchaseReturnDateTime);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@temptbl", dt);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                    cmd.Parameters.AddWithValue("@Remark", remark);
                    cmd.Parameters.AddWithValue("@ReturnVoucherNo", returnVoucherNo);
                    cmd.Parameters.AddWithValue("@AccountCode", AppConstants.PurchaseReturnAccountCode);
                    if (isPurchaseReturnByVoucher)
                    {
                        cmd.Parameters.AddWithValue("@SupplierID", masterData.MasterPurchaseModel.SupplierID);
                        cmd.Parameters.AddWithValue("@LocationID", masterData.MasterPurchaseModel.LocationID);
                        cmd.Parameters.AddWithValue("@PaymentID", masterData.MasterPurchaseModel.PaymentID);
                        cmd.Parameters.AddWithValue("@CurrencyID", masterData.MasterPurchaseModel.MCurrencyID);                       
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SupplierID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@LocationID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@PaymentID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@CurrencyID", DBNull.Value);                       
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    setting.conn.Close();
                    clearTranPurchaseReturn();
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

        public JsonResult GetProductByCodeAction(string productCode, bool isMultiUnit,bool isMultiCurrency)
        {
            ResultDefaultData resultDefaultData =new ResultDefaultData();
            ProductModels.ProductModel product=new ProductModels.ProductModel();
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            bool isExistProduct = true;
            try{
                product=appData.selectProductByCode(getConnection(),productCode);
                if(product.ProductID!=0){
                    if(isMultiUnit)lstUnit=getUnit();
                    if(isMultiCurrency)lstCurrency=getCurrency();
                }
                else isExistProduct=false;
                resultDefaultData.IsRequestSuccess=true;
            }
            catch(Exception ex){
                resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
                resultDefaultData.Message = ex.Message;
            }
            var jsonResult=new{
                Product=product,
                LstUnit=lstUnit,
                LstCurrency=lstCurrency,
                IsExistProduct=isExistProduct,
                ResultDefaultData=resultDefaultData
            };
            return Json(jsonResult,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductByKeywordAction(string keyword)
        {
            List<ProductModels.ProductModel> list = appData.selectProductByKeyword(getConnection(), keyword);
            Session["SearchProductData"] = null;
            Session["SearchProductData"] = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchProductClickAction(int productID,bool isMultiUnit,bool isMultiCurrency)
        {
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            ProductModels.ProductModel product = new ProductModels.ProductModel();
            bool isRequestSuccess = false;
            if(Session["SearchProductData"] != null)
            {
                List<ProductModels.ProductModel> list = Session["SearchProductData"] as List<ProductModels.ProductModel>;               
                product = list.Where(c => c.ProductID == productID).SingleOrDefault();
                if (product.ProductID != 0)
                {
                    if (isMultiUnit) lstUnit = getUnit();
                    if (isMultiCurrency) lstCurrency = getCurrency();
                    isRequestSuccess = true;
                }
            }
            var jsonResult = new
            {
                Product=product,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                IsRequestSuccess = isRequestSuccess
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranPurchaseReturnByProductAddEditAction(int productID,string productName,string code,int quantity,int purPrice,
            int disPercent,int unitID,string unitKeyword,int currencyId,string currencyKeyword,bool isEdit,bool isItemFOC,int number)
        {
            List<TranPurchaseReturnModels> lstTranPurchaseReturn = new List<TranPurchaseReturnModels>();
            TranPurchaseReturnModels data = new TranPurchaseReturnModels();
            int subtotal = 0, discount;
            bool isRequestSuccess = true;      
            data.Number=number; 
            data.ProductID = productID;
            data.ProductCode = code;
            data.ProductName = productName;
            data.Quantity = quantity;           
            data.PurchasePrice = purPrice;
            data.DiscountPercent = disPercent;
            data.UnitID = unitID;
            data.CurrencyID = currencyId;
            if (unitKeyword != null) data.UnitKeyword = unitKeyword;
            else data.UnitKeyword = "";
            if (currencyKeyword != null) data.CurrencyKeyword = currencyKeyword;
            else data.CurrencyKeyword = "";
            discount = ((purPrice * quantity) * disPercent) / 100;
            data.Discount = discount;
            data.Amount = (quantity * purPrice) - discount;
            data.IsFOC = isItemFOC;           
            if (isEdit)
            {
                if (Session["TranPurchaseReturnByProductData"] != null)
                {
                    lstTranPurchaseReturn = Session["TranPurchaseReturnByProductData"] as List<TranPurchaseReturnModels>;
                    foreach (var purchaseReturn in lstTranPurchaseReturn.Where(m => m.Number == number))
                    {
                        purchaseReturn.Quantity = data.Quantity;
                        purchaseReturn.Discount = data.Discount;
                        purchaseReturn.Amount = data.Amount;
                    }
                }
                else isRequestSuccess = false;
            }
            else
            {
                if (Session["TranPurchaseReturnByProductData"] != null)
                {
                    lstTranPurchaseReturn = Session["TranPurchaseReturnByProductData"] as List<TranPurchaseReturnModels>;
                    if (lstTranPurchaseReturn.Where(m => m.Number == number).Count() > 0)
                    {
                        foreach (var purchaseReturn in lstTranPurchaseReturn.Where(m => m.Number == number))
                        {
                            purchaseReturn.Quantity = data.Quantity;
                            purchaseReturn.Discount = data.Discount;
                            purchaseReturn.Amount = data.Amount;
                        }
                    }
                    else
                    {
                        lstTranPurchaseReturn.Add(data);
                    }
                }
                else lstTranPurchaseReturn.Add(data);
            }
            for (int i = 0; i < lstTranPurchaseReturn.Count(); i++)
            {
                subtotal += lstTranPurchaseReturn[i].Amount;
            }
            Session["TranPurchaseReturnByProductData"] = lstTranPurchaseReturn;
            var jsonResult = new
            {
                LstTranPurchaseReturn = lstTranPurchaseReturn,
                SubTotal = subtotal,
                IsRequestSuccess = isRequestSuccess
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TranPurchaseReturnByProductDeleteAction(int number)
        {
            List<TranPurchaseReturnModels> lstTranPurchaseReturn = new List<TranPurchaseReturnModels>();           
            int subtotal = 0; bool isRequestSuccess = true;           
            if (Session["TranPurchaseReturnByProductData"] != null)
            {
                lstTranPurchaseReturn = Session["TranPurchaseReturnByProductData"] as List<TranPurchaseReturnModels>;                               
                lstTranPurchaseReturn.Remove(lstTranPurchaseReturn.Where(m => m.Number == number).First());
                for (int i = 0; i < lstTranPurchaseReturn.Count(); i++)
                {
                    subtotal += lstTranPurchaseReturn[i].Amount;
                }
                Session["TranPurchaseReturnByProductData"] = lstTranPurchaseReturn;
            }
            else isRequestSuccess = false;            
            var jsonResult = new
            {
                LstTranPurchaseReturn = lstTranPurchaseReturn,
                SubTotal = subtotal,
                IsRequestSuccess = isRequestSuccess
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PrepareToEditTranPurchaseReturnByProductAction(int number,bool isMultiUnit,bool isMultiCurrency)
        {
            ResultDefaultData resultDefaultData = new ResultDefaultData();
            List<TranPurchaseReturnModels> lstTranPurchaseReturn = new List<TranPurchaseReturnModels>();
            TranPurchaseReturnModels data = new TranPurchaseReturnModels();
            List<UnitModels> lstUnit = new List<UnitModels>();
            List<CurrencyModels> lstCurrency = new List<CurrencyModels>();
            if (Session["TranPurchaseReturnByProductData"] != null)
            {
                lstTranPurchaseReturn = Session["TranPurchaseReturnByProductData"] as List<TranPurchaseReturnModels>;
                if (lstTranPurchaseReturn.Count() != 0)
                {
                    data = lstTranPurchaseReturn.Where(m => m.Number == number).First();
                    if (data != null)
                    {                       
                        resultDefaultData.IsRequestSuccess = true;
                        if (isMultiUnit)lstUnit= getUnit();
                        if (isMultiCurrency) lstCurrency = getCurrency();
                    }
                }
            }
            else resultDefaultData.UnSuccessfulReason = AppConstants.RequestUnSuccessful.SessionExpired.ToString();

            var jsonResult = new
            {
                Product=data,
                LstUnit = lstUnit,
                LstCurrency = lstCurrency,
                ResultDefaultData = resultDefaultData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Methods
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
            string userVoucherNo = appData.selectUserVoucherNo(AppConstants.PurchaseReturnModule, userId, getConnection());
            return userVoucherNo;
        }
        private void clearTranPurchaseReturn()
        {
            Session["TranPurchaseReturnData"] = null;
            Session["TranPurchaseData"] = null;
            Session["MasterPurchaseData"] = null;
            Session["TranPurchaseReturnByProductData"] = null;
        }

        private MasterPurchaseViewModel selectMasterPurchase(string purchaseVoucherNo)
        {
            MasterPurchaseViewModel item = new MasterPurchaseViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterPurchaseByPurchaseVoucherNo, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseVoucherNo", purchaseVoucherNo);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.MasterPurchaseModel.PurchaseID = Convert.ToInt32(reader["PurchaseID"]);
                item.MasterPurchaseModel.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.MasterPurchaseModel.VoucherID = Convert.ToString(reader["VoucherID"]);

                item.MasterPurchaseModel.Subtotal = Convert.ToInt32(reader["Subtotal"]);
                item.MasterPurchaseModel.Tax = Convert.ToInt32(reader["Tax"]);
                item.MasterPurchaseModel.TaxAmt = Convert.ToInt32(reader["TaxAmt"]);
                item.MasterPurchaseModel.Charges = Convert.ToInt32(reader["Charges"]);
                item.MasterPurchaseModel.ChargesAmt = Convert.ToInt32(reader["ChargesAmt"]);
                item.MasterPurchaseModel.Total = Convert.ToInt32(reader["Total"]);

                item.MasterPurchaseModel.VouDisPercent = Convert.ToInt32(reader["VouDisPercent"]);
                item.MasterPurchaseModel.VouDisAmount = Convert.ToInt32(reader["VouDisAmount"]);
                item.MasterPurchaseModel.VoucherDiscount = Convert.ToInt32(reader["VoucherDiscount"]);

                item.MasterPurchaseModel.AdvancedPay = Convert.ToInt32(reader["AdvancedPay"]);
                item.MasterPurchaseModel.PaymentPercent = Convert.ToInt32(reader["PaymentPercent"]);
                item.MasterPurchaseModel.PayPercentAmt = Convert.ToInt32(reader["PayPercentAmt"]);
                item.MasterPurchaseModel.Grandtotal = Convert.ToInt32(reader["Grandtotal"]);               
                item.MasterPurchaseModel.LocationID = Convert.ToInt32(reader["LocationID"]);               
                item.MasterPurchaseModel.IsVouFOC = Convert.ToBoolean(reader["IsVouFOC"]);
                item.MasterPurchaseModel.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                item.MasterPurchaseModel.SupplierID = Convert.ToInt32(reader["SupplierID"]);
                item.MasterPurchaseModel.MCurrencyID = Convert.ToInt32(reader["CurrencyID"]);
            }
            reader.Close();
            return item;
        }

        private List<TranPurchaseModels> selectTranPurchaseByPurchaseID(int purchaseID)
        {
            List<TranPurchaseModels> list = new List<TranPurchaseModels>();
            TranPurchaseModels item = new TranPurchaseModels();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranPurchaseByPurchaseID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseID", purchaseID);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranPurchaseModels();
                item.ID = Convert.ToInt32(reader["ID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                //item.MaxQuantity = Convert.ToInt32(reader["Quantity"]);
                item.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);
                item.ProductCode = Convert.ToString(reader["Code"]);
                item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                bool isduplicate = false;
                foreach(var row in list.Where(m=>m.ProductCode==item.ProductCode && m.ProductID==item.ProductID && m.PurchasePrice==item.PurchasePrice && m.DiscountPercent==item.DiscountPercent && m.UnitID==item.UnitID && m.CurrencyID==item.CurrencyID))
                {
                    row.Quantity += item.Quantity;
                    //row.MaxQuantity += item.MaxQuantity;
                    row.Discount += item.Discount;
                    row.Amount += item.Amount;
                    isduplicate = true;
                }
                if (!isduplicate)
                {
                    list.Add(item);
                }               
            }
            reader.Close();

            return list.OrderBy(m=>m.ID).ToList();
        }

        private List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> selectMasterPurchaseReturn(int userId, bool isSearch, [Optional]DateTime fromDate, [Optional]DateTime toDate, [Optional]string userVoucherNo)
        {
            List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> tempList = new List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel>();
            PurchaseReturnViewModel.MasterPurchaseReturnViewModel item = new PurchaseReturnViewModel.MasterPurchaseReturnViewModel();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterPurchaseReturnList, (SqlConnection)getConnection());
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
                item = new PurchaseReturnViewModel.MasterPurchaseReturnViewModel();
                item.PurchaseReturnID = Convert.ToInt32(reader["PurchaseReturnID"]);
                item.ReturnDateTime = Convert.ToString(reader["ReturnDateTime"]);
                item.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.ReturnVoucherNo = Convert.ToString(reader["ReturnVoucherNo"]);            
                item.Total = Convert.ToInt32(reader["Total"]);
                tempList.Add(item);
            }
            reader.Close();
            Session["MasterPurchaseReturnData"] = tempList;  // for paging

            return tempList;
        }

        private PagingViewModel calcMasterPurchaseReturnPaging(List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> tempList, [Optional]int currentPage)
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

        private List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> getMasterPurchaseReturnByPaging(List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> tempList, int startRowIndex, int endRowIndex)
        {
            List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel> list = new List<PurchaseReturnViewModel.MasterPurchaseReturnViewModel>();
            PurchaseReturnViewModel.MasterPurchaseReturnViewModel item = new PurchaseReturnViewModel.MasterPurchaseReturnViewModel();

            for (int page = startRowIndex; page < tempList.Count; page++)
            {
                if (page > endRowIndex) break;

                item = new PurchaseReturnViewModel.MasterPurchaseReturnViewModel();
                item.PurchaseReturnID = tempList[page].PurchaseReturnID;
                item.ReturnDateTime = tempList[page].ReturnDateTime;
                item.UserVoucherNo = tempList[page].UserVoucherNo;
                item.ReturnVoucherNo = tempList[page].ReturnVoucherNo;                
                item.Total = tempList[page].Total;
                list.Add(item);
            }
            return list;
        }

        private MasterPurchaseReturnViewModel selectMasterPurchaseReturn(int purchaseReturnId)
        {
            MasterPurchaseReturnViewModel item = new MasterPurchaseReturnViewModel();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMasterPurchaseReturnByPurchaseReturnID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseReturnID", purchaseReturnId);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.MasterPurchaseReturnModel.ReturnVoucherNo = Convert.ToString(reader["ReturnVoucherNo"]);
                item.MasterPurchaseReturnModel.PurchaseReturnDateTime = Convert.ToString(reader["ReturnDateTime"]);
                item.MasterPurchaseReturnModel.UserVoucherNo = Convert.ToString(reader["UserVoucherNo"]);
                item.MasterPurchaseReturnModel.VoucherID = Convert.ToString(reader["VoucherID"]);
                item.MasterPurchaseReturnModel.Remark = Convert.ToString(reader["Remark"]);
                item.MasterPurchaseReturnModel.Total = Convert.ToInt32(reader["Total"]);
                item.UserName = Convert.ToString(reader["UserName"]);
                item.MasterPurchaseReturnModel.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.LocationName = Convert.ToString(reader["LocationName"]);                                                                              
            }
            reader.Close();
            return item;
        }

        private List<TranPurchaseReturnModels> selectTranPurchaseReturnByPurchaseReturnID(int purchaseReturnId)
        {
            List<TranPurchaseReturnModels> list = new List<TranPurchaseReturnModels>();
            TranPurchaseReturnModels item = new TranPurchaseReturnModels();

            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTranPurchaseReturnByPurchaseReturnID, (SqlConnection)getConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PurchaseReturnID", purchaseReturnId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new TranPurchaseReturnModels();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductCode = Convert.ToString(reader["Code"]);             
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.MaxQuantity = Convert.ToInt32(reader["Quantity"]);
                item.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                item.Discount = Convert.ToInt32(reader["Discount"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
                item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                item.CurrencyKeyword = Convert.ToString(reader["CurrencyKeyword"]);                
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.DiscountPercent = Convert.ToInt32(reader["DiscountPercent"]);                
                item.IsFOC = Convert.ToBoolean(reader["IsFOC"]);
                list.Add(item);
            }
            reader.Close();

            return list;
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
        private List<CurrencyModels> getCurrency()
        {
            List<CurrencyModels> list = new List<CurrencyModels>();
            if (Session["CurrencyData"] == null)
            {
                list = appData.selectCurrency(getConnection());
                Session["CurrencyData"] = list;
            }
            else list = Session["CurrencyData"] as List<CurrencyModels>;
            return list;
        }



        #endregion
    }
}
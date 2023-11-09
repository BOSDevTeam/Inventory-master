using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Common;


namespace Inventory.Controllers
{
    public class ReportController : MyController
    {
        ReportViewModel reportViewModel = new ReportViewModel();
        AppSetting setting = new AppSetting();
        TextQuery textQuery = new TextQuery();

        public ActionResult Index(int userId, short isTechnician)
        {
            ViewBag.IsTechnician = isTechnician;
            if (isTechnician == 0) selectReportModule(userId);
            return View();
        }

        public ActionResult Filter(string reportName)
        {
            reportViewModel.ReportName = reportName;

            if (reportName.Equals(Resource.SaleAmountOnlyReport))
            {
                reportViewModel.ControllerName = "RpSaleAmountOnly";
                reportViewModel.ActionName = "SaleAmountOnlyReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAmountOnlyRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleAmountSummaryReport))
            {
                reportViewModel.ControllerName = "RpSaleAmountSummary";
                reportViewModel.ActionName = "SaleAmountSummaryReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAmountSummaryRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleAmountByMultiPayReport))
            {
                reportViewModel.ControllerName = "RpSaleAmountByMultiPay";
                reportViewModel.ActionName = "SaleAmountByMultiPayReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAmountByMultiPayRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleItemSimpleReport))
            {
                reportViewModel.ControllerName = "RpSaleItemSimple";
                reportViewModel.ActionName = "SaleItemSimpleReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleItemSimpleRp";
                reportViewModel.IsShowMenuFilter = true;
                GetMenuData();
                ViewBag.isShowMenu = 1;
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleItemReport))
            {
                reportViewModel.ControllerName = "RpSaleItem";
                reportViewModel.ActionName = "SaleItemReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleItemRp";
                reportViewModel.IsShowPaymentFilter = true;
                ViewBag.isShowPayment = 1;
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.PurchaseAmountOnlyReport))
            {
                reportViewModel.ControllerName = "RpPurchaseAmountOnly";
                reportViewModel.ActionName = "PurchaseAmountOnlyReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseAmountOnlyRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleAuditReport))
            {
                reportViewModel.ControllerName = "RpSaleAudit";
                reportViewModel.ActionName = "SaleAuditReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAuditRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleAmountByCustomerReport))
            {
                reportViewModel.ControllerName = "RpSaleAmountByCustomer";
                reportViewModel.ActionName = "SaleAmountByCustomerReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAmountByCustomerRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleAmountBySalePersonReport))
            {
                reportViewModel.ControllerName = "RpSaleAmountBySalePerson";
                reportViewModel.ActionName = "SaleAmountBySalePersonReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAmountBySalePersonRp";
                reportViewModel.IsShowUserClientFilter = true;
                ViewBag.isShowUserClient = 1;
                GetUserList();
                GetClientList();
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleItemByCustomerReport))
            {
                reportViewModel.ControllerName = "RpSaleItemByCustomer";
                reportViewModel.ActionName = "SaleItemByCustomerReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleItemByCustomerRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.TopSaleItemReport))
            {
                reportViewModel.ControllerName = "RpTopSaleItem";
                reportViewModel.ActionName = "TopSaleItemReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuTopSaleItemRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.BottomSaleItemReport))
            {
                reportViewModel.ControllerName = "RpBottomSaleItem";
                reportViewModel.ActionName = "BottomSaleItemReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuBottomSaleItemRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.PurchaseItemSimpleReport))
            {
                reportViewModel.ControllerName = "RpPurchaseItemSimple";
                reportViewModel.ActionName = "PurchaseItemSimpleReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseItemSimpleRp";
                reportViewModel.IsShowMenuFilter = true;
                ViewBag.isShowMenu = 1;
                GetMenuData();
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.PurchaseAuditReport))
            {
                reportViewModel.ControllerName = "RpPurchaseAudit";
                reportViewModel.ActionName = "PurchaseAuditReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseAuditRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.PurchaseAmountBySupplierReport))
            {
                reportViewModel.ControllerName = "RpPurchaseAmountBySupplier";
                reportViewModel.ActionName = "PurchaseAmountBySupplierReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseAmountBySupplierRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.PurchaseItemBySupplierReport))
            {
                reportViewModel.ControllerName = "RpPurchaseItemBySupplier";
                reportViewModel.ActionName = "PurchaseItemBySupplierReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseItemBySupplierRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.AdjustmentVoucherReport))
            {
                reportViewModel.ControllerName = "RpAdjustmentVoucher";
                reportViewModel.ActionName = "AdjustmentVoucherReport";
                reportViewModel.HtmlGroupElementID = "subMenuAdjustmentRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuAdjustmentVoucherRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.AdjustmentItemReport))
            {
                reportViewModel.ControllerName = "RpAdjustmentItem";
                reportViewModel.ActionName = "AdjustmentItemReport";
                reportViewModel.HtmlGroupElementID = "subMenuAdjustmentRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuAdjustmentItemRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.TransferVoucherReport))
            {
                reportViewModel.ControllerName = "RpTransferVoucher";
                reportViewModel.ActionName = "TransferVoucherReport";
                reportViewModel.HtmlGroupElementID = "subMenuTransferRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuTransferVoucherRp";
            }
            else if (reportName.Equals(Resource.TransferItemReport))
            {
                reportViewModel.ControllerName = "RpTransferItem";
                reportViewModel.ActionName = "TransferItemReport";
                reportViewModel.HtmlGroupElementID = "subMenuTransferRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuTransferItemRp";
            }
            else if (reportName.Equals(Resource.SaleItemProfitReport))
            {
                reportViewModel.ControllerName = "RpSaleItemProfit";
                reportViewModel.ActionName = "SaleItemProfitReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleItemProfitRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleLogReport))
            {
                reportViewModel.ControllerName = "RpSaleLog";
                reportViewModel.ActionName = "SaleLogReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleLogRp";
                reportViewModel.IsShowLogFilter = true;
                ViewBag.isShowLog = 1;
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.PurchaseLogReport))
            {
                reportViewModel.ControllerName = "RpPurchaseLog";
                reportViewModel.ActionName = "PurchaseLogReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseLogRp";
                reportViewModel.IsShowLogFilter = true;
                ViewBag.isShowLog = 1;
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.StockStatusDetailReport))
            {
                reportViewModel.ControllerName = "RpStockStatusByModule";
                reportViewModel.ActionName = "StockStatusByModuleReport";
                reportViewModel.HtmlGroupElementID = "subMenuStockStatusRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuStockStatusDetailRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            else if (reportName.Equals(Resource.SaleItemByStaffReport))
            {
                reportViewModel.ControllerName = "RpSaleItemByStaff";
                reportViewModel.ActionName = "SaleItemByStaffReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleItemByStaffRp";
                reportViewModel.IsShowLocationFilter = true;
                ViewBag.LocationData = getLocation();
                ViewBag.isShowLocation = 1;
            }
            return View(reportViewModel);
        }

        private void GetMenuData()
        {
            List<SubMenuModels.SubMenuModel> lst = new List<SubMenuModels.SubMenuModel>();
            SubMenuModels.SubMenuModel menu = new SubMenuModels.SubMenuModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetMenuData, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                menu = new SubMenuModels.SubMenuModel();
                menu.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                menu.MainMenuName = Convert.ToString(reader["MainMenuName"]);
                menu.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                menu.SubMenuName = Convert.ToString(reader["SubMenuName"]);
                lst.Add(menu);
            }
            reader.Close();
            setting.conn.Close();
            ViewBag.MenuData = lst;
        }

        private void GetUserList()
        {
            List<UserModels.UserModel> lstUser = new List<UserModels.UserModel>();
            UserModels.UserModel user = new UserModels.UserModel();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetUser, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                user = new UserModels.UserModel();
                user.UserID = Convert.ToInt32(reader["UserID"]);
                user.UserName = Convert.ToString(reader["UserName"]);
                lstUser.Add(user);
            }
            reader.Close();
            setting.conn.Close();
            ViewBag.UserData= lstUser;
        }

        private void GetClientList()
        {
            List<ClientModels> lstClient = new List<ClientModels>();
            ClientModels client = new ClientModels();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(TextQuery.clientSalePersonQuery, setting.conn);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                client = new ClientModels();
                client.ClientID = Convert.ToInt32(reader["ClientID"]);
                client.ClientName = Convert.ToString(reader["ClientName"]);
                lstClient.Add(client);
            }
            reader.Close();
            setting.conn.Close();
            ViewBag.ClientData= lstClient;
        }

        private List<LocationModels.LocationModel> getLocation()
        {
            AppData appData = new AppData();
            return appData.selectLocation();
        }

        public void selectReportModule(int userId)
        {
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(textQuery.getReportModuleAccessQuery(userId), setting.conn);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                switch (Convert.ToString(reader["ReportModuleName"]))
                {
                    case "Sale Amount Only Report":
                        ViewBag.IsAllowSaleAmountOnly = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Amount Summary Report":
                        ViewBag.IsAllowSaleAmountSummary = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Amount by MultiPay Report":
                        ViewBag.IsAllowSaleAmountByMultiPay = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Item Simple Report":
                        ViewBag.IsAllowSaleItemSimple = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Item Report":
                        ViewBag.IsAllowSaleItem = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Audit Report":
                        ViewBag.IsAllowSaleAudit = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Amount by Customer Report":
                        ViewBag.IsAllowSaleAmountByCustomer = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Amount by SalePerson Report":
                        ViewBag.IsAllowSaleAmountBySalePerson = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Item by Customer Report":
                        ViewBag.IsAllowSaleItemByCustomer = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Item by Staff Report":
                        ViewBag.IsAllowSaleItemByStaff = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Top Sale Item Report":
                        ViewBag.IsAllowTopSaleItem = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Bottom Sale Item Report":
                        ViewBag.IsAllowBottomSaleItem = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Item Profit Report":
                        ViewBag.IsAllowSaleItemProfit = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Log Report":
                        ViewBag.IsAllowSaleLog = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Purchase Amount Only Report":
                        ViewBag.IsAllowPurchaseAmountOnly = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Purchase Item Simple Report":
                        ViewBag.IsAllowPurchaseItemSimple = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Purchase Audit Report":
                        ViewBag.IsAllowPurchaseAudit = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Purchase Amount by Supplier Report":
                        ViewBag.IsAllowPurchaseAmountBySupplier = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Purchase Item by Supplier Report":
                        ViewBag.IsAllowPurchaseItemBySupplier = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Purchase Log Report":
                        ViewBag.IsAllowPurchaseLog = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Adjustment Voucher Report":
                        ViewBag.IsAllowAdjustmentVoucher = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Adjustment Item Report":
                        ViewBag.IsAllowAdjustmentItem = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Transfer Voucher Report":
                        ViewBag.IsAllowTransferVoucher = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Transfer Item Report":
                        ViewBag.IsAllowTransferItem = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Stock Status Detail Report":
                        ViewBag.IsAllowStockStatusDetail = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                }
            }
            reader.Close();
            setting.conn.Close();
        }
    }
}
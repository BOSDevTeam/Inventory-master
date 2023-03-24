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
        public ActionResult Index()
        {
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
            }
            else if (reportName.Equals(Resource.SaleAmountSummaryReport))
            {
                reportViewModel.ControllerName = "RpSaleAmountSummary";
                reportViewModel.ActionName = "SaleAmountSummaryReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAmountSummaryRp";
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
            }
            else if (reportName.Equals(Resource.SaleItemReport))
            {
                reportViewModel.ControllerName = "RpSaleItem";
                reportViewModel.ActionName = "SaleItemReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleItemRp";
                reportViewModel.IsShowPaymentFilter = true;
                ViewBag.isShowPayment = 1;
            }
            else if (reportName.Equals(Resource.PurchaseAmountOnlyReport))
            {
                reportViewModel.ControllerName = "RpPurchaseAmountOnly";
                reportViewModel.ActionName = "PurchaseAmountOnlyReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseAmountOnlyRp";
            }
            else if (reportName.Equals(Resource.SaleAuditReport))
            {
                reportViewModel.ControllerName = "RpSaleAudit";
                reportViewModel.ActionName = "SaleAuditReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAuditRp";
            }
            else if (reportName.Equals(Resource.SaleAmountByCustomerReport))
            {
                reportViewModel.ControllerName = "RpSaleAmountByCustomer";
                reportViewModel.ActionName = "SaleAmountByCustomerReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAmountByCustomerRp";
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
            }
            else if (reportName.Equals(Resource.SaleItemByCustomerReport))
            {
                reportViewModel.ControllerName = "RpSaleItemByCustomer";
                reportViewModel.ActionName = "SaleItemByCustomerReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleItemByCustomerRp";
            }
            else if (reportName.Equals(Resource.TopSaleItemReport))
            {
                reportViewModel.ControllerName = "RpTopSaleItem";
                reportViewModel.ActionName = "TopSaleItemReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuTopSaleItemRp";
            }
            else if (reportName.Equals(Resource.BottomSaleItemReport))
            {
                reportViewModel.ControllerName = "RpBottomSaleItem";
                reportViewModel.ActionName = "BottomSaleItemReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuBottomSaleItemRp";
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
            }
            else if (reportName.Equals(Resource.PurchaseAuditReport))
            {
                reportViewModel.ControllerName = "RpPurchaseAudit";
                reportViewModel.ActionName = "PurchaseAuditReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseAuditRp";
            }
            else if (reportName.Equals(Resource.PurchaseAmountBySupplierReport))
            {
                reportViewModel.ControllerName = "RpPurchaseAmountBySupplier";
                reportViewModel.ActionName = "PurchaseAmountBySupplierReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseAmountBySupplierRp";
            }
            else if (reportName.Equals(Resource.PurchaseItemBySupplierReport))
            {
                reportViewModel.ControllerName = "RpPurchaseItemBySupplier";
                reportViewModel.ActionName = "PurchaseItemBySupplierReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseItemBySupplierRp";
            }
            else if (reportName.Equals(Resource.AdjustmentVoucherReport))
            {
                reportViewModel.ControllerName = "RpAdjustmentVoucher";
                reportViewModel.ActionName = "AdjustmentVoucherReport";
                reportViewModel.HtmlGroupElementID = "subMenuAdjustmentRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuAdjustmentVoucherRp";
            }
            else if (reportName.Equals(Resource.AdjustmentItemReport))
            {
                reportViewModel.ControllerName = "RpAdjustmentItem";
                reportViewModel.ActionName = "AdjustmentItemReport";
                reportViewModel.HtmlGroupElementID = "subMenuAdjustmentRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuAdjustmentItemRp";
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using System.Data.SqlClient;
using System.Data;

namespace Inventory.Controllers
{
    public class EntryController : MyController
    {
        TextQuery textQuery = new TextQuery();
        AppSetting setting = new AppSetting();

        public ActionResult Index(int userId, short isTechnician)
        {
            ViewBag.IsTechnician = isTechnician;
            if (isTechnician == 0) selectEntryModule(userId);         
            return View();
        }

        public void selectEntryModule(int userId)
        {
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(textQuery.getEntryModuleAccessQuery(userId), setting.conn);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                switch (Convert.ToString(reader["EntryModuleName"]))
                {
                    case "Dashboard":
                        ViewBag.IsAllowDashboardModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale":
                        ViewBag.IsAllowSaleModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Client Sale Order":
                        ViewBag.IsAllowClientSaleOrderModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Order":
                        ViewBag.IsAllowSaleOrderModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sale Return":
                        ViewBag.IsAllowSaleReturnModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Purchase":
                        ViewBag.IsAllowPurchaseModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Purchase Order":
                        ViewBag.IsAllowPurchaseOrderModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Purchase Return":
                        ViewBag.IsAllowPurchaseReturnModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Adjustment":
                        ViewBag.IsAllowAdjustmentModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Transfer":
                        ViewBag.IsAllowTransferModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Consignment":
                        ViewBag.IsAllowConsignmentModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Customer Opening":
                        ViewBag.IsAllowCustomerOpeningModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Customer Outstanding":
                        ViewBag.IsAllowCustomerOutstandingModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Supplier Opening":
                        ViewBag.IsAllowSupplierOpeningModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Supplier Outstanding":
                        ViewBag.IsAllowSupplierOutstandingModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Opening Stock":
                        ViewBag.IsAllowOpeningStockModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Stock Status":
                        ViewBag.IsAllowStockStatusModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                }
            }
            reader.Close();
            setting.conn.Close();
        }
    }
}
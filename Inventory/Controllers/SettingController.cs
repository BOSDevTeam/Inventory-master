using Inventory.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using System.Data.SqlClient;
using System.Data;

namespace Inventory.Controllers
{
    public class SettingController : MyController
    {
        TextQuery textQuery = new TextQuery();
        AppSetting setting = new AppSetting();

        public ActionResult Index(int userId,short isTechnician)
        {
            ViewBag.IsTechnician = isTechnician;
            if (isTechnician == 0) selectSetupModule(userId);
            return View();
        }

        public void selectSetupModule(int userId)
        {
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(textQuery.getSetupModuleAccessQuery(userId), setting.conn);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                switch (Convert.ToString(reader["SetupModuleName"]))
                {
                    case "User":
                        ViewBag.IsAllowUserModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "User Right":
                        ViewBag.IsAllowUserRightModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Division":
                        ViewBag.IsAllowDivisionModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Township":
                        ViewBag.IsAllowTownshipModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Customer":
                        ViewBag.IsAllowCustomerModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Supplier":
                        ViewBag.IsAllowSupplierModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Staff":
                        ViewBag.IsAllowStaffModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Main Menu":
                        ViewBag.IsAllowMainMenuModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Sub Menu":
                        ViewBag.IsAllowSubMenuModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Product":
                        ViewBag.IsAllowProductModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Bank":
                        ViewBag.IsAllowBankModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Bank Payment":
                        ViewBag.IsAllowBankPaymentModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Client":
                        ViewBag.IsAllowClientModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                    case "Adjust Type":
                        ViewBag.IsAllowAdjustTypeModule = Convert.ToBoolean(reader["IsAllow"]);
                        break;
                }
            }
            reader.Close();
            setting.conn.Close();
        }
    }
}
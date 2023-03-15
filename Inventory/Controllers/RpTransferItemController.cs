using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;
using Inventory.Common;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Controllers
{
    public class RpTransferItemController : MyController
    {      
        RpTransferItemViewModel transferItemViewModel = new RpTransferItemViewModel();
        AppSetting setting = new AppSetting();
        public ActionResult TransferItemReportFilter()
        {
            return View();
        }

        public ActionResult TransferItemReport(DateTime fromDate, DateTime toDate)
        {
            try
            {
                transferItemViewModel.FromDate = fromDate;
                transferItemViewModel.ToDate = toDate;
                transferItemViewModel.lstMasterTransfer = GetRptTransferItem(fromDate, toDate);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }          
            return View(transferItemViewModel);
        }

        private List<RpTransferItemViewModel.MasterTransferModels> GetRptTransferItem(DateTime fromDate,DateTime toDate)
        {
            List<RpTransferItemViewModel.MasterTransferModels> lstLoc = new List<RpTransferItemViewModel.MasterTransferModels>();
            RpTransferItemViewModel.MasterTransferModels Location = new RpTransferItemViewModel.MasterTransferModels();
            List<RpTransferItemViewModel.TranTransferModels> lstTransferItem = new List<RpTransferItemViewModel.TranTransferModels>();
            RpTransferItemViewModel.TranTransferModels item = new RpTransferItemViewModel.TranTransferModels();
            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetRptTransferItem, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Location = new RpTransferItemViewModel.MasterTransferModels();
                Location.FromLocationID = Convert.ToInt32(reader["FromLocationID"]);
                Location.ToLocationID = Convert.ToInt32(reader["ToLocationID"]);
                if(lstLoc.Where(m=>m.FromLocationID==Location.FromLocationID && m.ToLocationID == Location.ToLocationID).Count() > 0)
                {
                    item = new RpTransferItemViewModel.TranTransferModels();
                    item.ProductCode = Convert.ToString(reader["Code"]);
                    item.ProductName = Convert.ToString(reader["ProductName"]);
                    item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                    item.PurPrice = Convert.ToInt32(reader["PurPrice"]);
                    item.Quantity = Convert.ToInt32(reader["Quantity"]);
                    foreach(var loc in lstLoc.Where(m => m.FromLocationID == Location.FromLocationID && m.ToLocationID == Location.ToLocationID))
                    {
                        loc.lstTransferItem.Add(item);
                    }
                }
                else
                {
                    Location.FromLocationName = Convert.ToString(reader["LocationName1"]);
                    Location.ToLocationName = Convert.ToString(reader["LocationName2"]);
                    lstTransferItem = new List<RpTransferItemViewModel.TranTransferModels>();
                    item = new RpTransferItemViewModel.TranTransferModels();
                    item.ProductCode = Convert.ToString(reader["Code"]);
                    item.ProductName = Convert.ToString(reader["ProductName"]);
                    item.UnitKeyword = Convert.ToString(reader["UnitKeyword"]);
                    item.PurPrice = Convert.ToInt32(reader["PurPrice"]);
                    item.Quantity = Convert.ToInt32(reader["Quantity"]);
                    lstTransferItem.Add(item);
                    Location.lstTransferItem = lstTransferItem;
                    lstLoc.Add(Location);
                }

            }
            reader.Close();
            setting.conn.Close();
            return lstLoc;
        }       
    }
}
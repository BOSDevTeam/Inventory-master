using Inventory.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;

namespace Inventory.Controllers
{
    public class HomeController : MyController
    {
        AppSetting setting = new AppSetting();
        HomeViewModel homeViewModel = new HomeViewModel();

        public ActionResult Dashboard()
        {
            homeViewModel.lstTopSaleProduct = selectTopSaleProduct(2);
            homeViewModel.lstTodaySaleQuantity = selectTodaySaleQuantity(setting.getLocalDate());
            return View(homeViewModel);
        }

        public List<HomeViewModel.TodaySaleQuantityView> selectTodaySaleQuantity(DateTime todayDate)
        {
            List<HomeViewModel.TodaySaleQuantityView> list = new List<HomeViewModel.TodaySaleQuantityView>();
            HomeViewModel.TodaySaleQuantityView item;
            string discount = "";

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTodaySaleQuantity, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TodayDate", todayDate);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new HomeViewModel.TodaySaleQuantityView();
                if (Convert.ToBoolean(reader["IsDiscount"]))
                {
                    discount = Convert.ToString(reader["Name"]);
                    item.Name= "Discount ("+ discount +"%)";
                }else item.Name = Convert.ToString(reader["Name"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                item.Amount = Convert.ToInt32(reader["Amount"]);
               
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }

        public List<HomeViewModel.TopSaleProductView> selectTopSaleProduct(int month)
        {
            List<HomeViewModel.TopSaleProductView> list = new List<HomeViewModel.TopSaleProductView>();
            HomeViewModel.TopSaleProductView item;
            int number = 0;

            setting.conn.Open();
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetTopSaleProductByMonth, setting.conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Month", month);
            cmd.Connection = setting.conn;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new HomeViewModel.TopSaleProductView();
                number++;
                item.Number = number;
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Quantity = Convert.ToInt32(reader["Quantity"]);
                list.Add(item);
            }
            reader.Close();
            setting.conn.Close();
            return list;
        }
    }
}
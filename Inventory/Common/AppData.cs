using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Inventory.Models;

namespace Inventory.Common
{
    public class AppData
    {
        TextQuery textQuery = new TextQuery();
        public List<CustomerModels.CustomerModel> selectCustomer(object connection)
        {
            List<CustomerModels.CustomerModel> list = new List<CustomerModels.CustomerModel>();
            CustomerModels.CustomerModel item;

            SqlCommand cmd = new SqlCommand(TextQuery.customerQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new CustomerModels.CustomerModel();
                item.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                item.CustomerName = Convert.ToString(reader["CustomerName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public List<SupplierModels.SupplierModel> selectSupplier(object connection)
        {
            List<SupplierModels.SupplierModel> list = new List<SupplierModels.SupplierModel>();
            SupplierModels.SupplierModel item;

            SqlCommand cmd = new SqlCommand(TextQuery.supplierQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new SupplierModels.SupplierModel();
                item.SupplierID = Convert.ToInt32(reader["SupplierID"]);
                item.SupplierName = Convert.ToString(reader["SupplierName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public List<LocationModels.LocationModel> selectLocation(object connection)
        {
            List<LocationModels.LocationModel> list = new List<LocationModels.LocationModel>();
            LocationModels.LocationModel item;

            SqlCommand cmd = new SqlCommand(TextQuery.locationQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new LocationModels.LocationModel();
                item.LocationID = Convert.ToInt32(reader["LocationID"]);
                item.ShortName = Convert.ToString(reader["ShortName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public string selectUserVoucherNo(int moduleCode, int userId, object connection)
        {
            string result = "";
            SqlCommand cmd = new SqlCommand(Procedure.PrcGetUserVoucherNo, (SqlConnection)connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ModuleCode", moduleCode);  //sale,purchase...
            cmd.Parameters.AddWithValue("@ID", userId);
            cmd.Parameters.AddWithValue("@IsClient", false);  //client is mobile user
            cmd.Parameters.AddWithValue("@IsUpdateNo", false);  //client is mobile user
            cmd.Parameters.AddWithValue("@UserVoucherNo", "");  //output parameter
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) result = Convert.ToString(reader[0]);
            reader.Close();

            return result;
        }

        public IEnumerable<MainMenuModels.MainMenuModel> selectMainMenu(object connection)
        {
            List<MainMenuModels.MainMenuModel> list=new List<MainMenuModels.MainMenuModel>();
            MainMenuModels.MainMenuModel item;

            SqlCommand cmd = new SqlCommand(TextQuery.mainMenuQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new MainMenuModels.MainMenuModel();
                item.MainMenuID = Convert.ToInt32(reader["MainMenuID"]);
                item.MainMenuName = Convert.ToString(reader["MainMenuName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public IEnumerable<SubMenuModels.SubMenuModel> selectSubMenu(object connection,int mainMenuId)
        {
            List<SubMenuModels.SubMenuModel> list = new List<SubMenuModels.SubMenuModel>();
            SubMenuModels.SubMenuModel item;

            SqlCommand cmd = new SqlCommand(textQuery.getSubMenuQuery(mainMenuId), (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new SubMenuModels.SubMenuModel();
                item.SubMenuID = Convert.ToInt32(reader["SubMenuID"]);
                item.SubMenuName = Convert.ToString(reader["SubMenuName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public IEnumerable<ProductModels.ProductModel> selectProduct(object connection, int subMenuId)
        {
            List<ProductModels.ProductModel> list = new List<ProductModels.ProductModel>();
            ProductModels.ProductModel item;

            SqlCommand cmd = new SqlCommand(textQuery.getProductQuery(subMenuId), (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new ProductModels.ProductModel();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Code = Convert.ToString(reader["Code"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.DisPercent = Convert.ToInt16(reader["DisPercent"]);
                item.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public ProductModels.ProductModel selectProductByCode(object connection, string productCode)
        {
            ProductModels.ProductModel item = new ProductModels.ProductModel();

            SqlCommand cmd = new SqlCommand(textQuery.getProductByCodeQuery(productCode), (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.DisPercent = Convert.ToInt16(reader["DisPercent"]);
                item.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
            }
            reader.Close();

            return item;
        }

        public List<ProductModels.ProductModel> selectProductByKeyword(object connection, string keyword)
        {
            List<ProductModels.ProductModel> list = new List<ProductModels.ProductModel>();
            ProductModels.ProductModel item;

            SqlCommand cmd = new SqlCommand(Procedure.PrcSearchProductByCodeName, (SqlConnection)connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@keyword", keyword);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new ProductModels.ProductModel();
                item.ProductID = Convert.ToInt32(reader["ProductID"]);
                item.ProductName = Convert.ToString(reader["ProductName"]);
                item.Code = Convert.ToString(reader["Code"]);            
                item.SalePrice = Convert.ToInt32(reader["SalePrice"]);
                item.DisPercent = Convert.ToInt16(reader["DisPercent"]);
                item.PurchasePrice = Convert.ToInt32(reader["PurPrice"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public int selectSalePriceByProduct(object connection, int productId)
        {
            int price = 0;
            SqlCommand cmd = new SqlCommand(textQuery.getSalePriceQuery(productId), (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) price = Convert.ToInt32(reader["SalePrice"]);
            reader.Close();

            return price;
        }

        public int selectPurPriceByProduct(object connection, int productId)
        {
            int price = 0;
            SqlCommand cmd = new SqlCommand(textQuery.getPurPriceQuery(productId), (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) price = Convert.ToInt32(reader["PurPrice"]);
            reader.Close();

            return price;
        }

        public List<UnitModels> selectUnit(object connection)
        {
            List<UnitModels> list = new List<UnitModels>();
            UnitModels item;

            SqlCommand cmd = new SqlCommand(TextQuery.unitQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new UnitModels();
                item.UnitID = Convert.ToInt32(reader["UnitID"]);
                item.Keyword = Convert.ToString(reader["Keyword"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public List<CurrencyModels> selectCurrency(object connection)
        {
            List<CurrencyModels> list = new List<CurrencyModels>();
            CurrencyModels item;

            SqlCommand cmd = new SqlCommand(TextQuery.currencyQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new CurrencyModels();
                item.CurrencyID = Convert.ToInt32(reader["CurrencyID"]);
                item.Keyword = Convert.ToString(reader["Keyword"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public List<PaymentModels> selectPayment(object connection)
        {
            List<PaymentModels> list = new List<PaymentModels>();
            PaymentModels item;

            SqlCommand cmd = new SqlCommand(TextQuery.paymentQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new PaymentModels();
                item.PaymentID = Convert.ToInt32(reader["PaymentID"]);
                item.Keyword = Convert.ToString(reader["Keyword"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public List<PayMethodModels> selectPayMethod(object connection)
        {
            List<PayMethodModels> list = new List<PayMethodModels>();
            PayMethodModels item;

            SqlCommand cmd = new SqlCommand(TextQuery.payMethodQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new PayMethodModels();
                item.PayMethodID = Convert.ToInt32(reader["PayMethodID"]);
                item.PayMethodName = Convert.ToString(reader["PayMethodName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public List<LimitedDayModels> selectLimitedDay(object connection)
        {
            List<LimitedDayModels> list = new List<LimitedDayModels>();
            LimitedDayModels item;

            SqlCommand cmd = new SqlCommand(TextQuery.limitedDayQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new LimitedDayModels();
                item.LimitedDayID = Convert.ToInt32(reader["LimitedDayID"]);
                item.LimitedDayName = Convert.ToString(reader["LimitedDayName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public List<BankPaymentModels> selectBankPayment(object connection)
        {
            List<BankPaymentModels> list = new List<BankPaymentModels>();
            BankPaymentModels item;

            SqlCommand cmd = new SqlCommand(TextQuery.bankPaymentQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new BankPaymentModels();
                item.BankPaymentID = Convert.ToInt32(reader["BankPaymentID"]);
                item.Name = Convert.ToString(reader["BankPaymentName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public VoucherSettingModels selectVoucherSettingByLocation(object connection, int locationId)
        {
            VoucherSettingModels item = new VoucherSettingModels();

            SqlCommand cmd = new SqlCommand(textQuery.getVoucherSettingQuery(locationId), (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                item.HeaderName = Convert.ToString(reader["HeaderName"]);
                item.HeaderDesp = Convert.ToString(reader["HeaderDesp"]);
                item.HeaderPhone = Convert.ToString(reader["HeaderPhone"]);
                item.HeaderAddress = Convert.ToString(reader["HeaderAddress"]);
                item.OtherHeader1 = Convert.ToString(reader["OtherHeader1"]);
                item.OtherHeader2 = Convert.ToString(reader["OtherHeader2"]);
                item.FooterMessage1 = Convert.ToString(reader["FooterMessage1"]);
                item.FooterMessage2 = Convert.ToString(reader["FooterMessage2"]);
                item.FooterMessage3 = Convert.ToString(reader["FooterMessage3"]);
                if (reader["VoucherLogo"].ToString().Length != 0)
                    item.Base64Photo = Convert.ToBase64String((byte[])(reader["VoucherLogo"]));
            }
            reader.Close();

            return item;
        }

        public List<AdjustTypeModels> selectAdjustType(object connection)
        {
            List<AdjustTypeModels> list = new List<AdjustTypeModels>();
            AdjustTypeModels item;

            SqlCommand cmd = new SqlCommand(TextQuery.adjustTypeQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new AdjustTypeModels();
                item.AdjustTypeID = Convert.ToInt32(reader["AdjustTypeID"]);
                item.ShortName = Convert.ToString(reader["ShortName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public List<DivisionModels> selectDivision(object connection)
        {
            List<DivisionModels> list = new List<DivisionModels>();
            DivisionModels item;

            SqlCommand cmd = new SqlCommand(TextQuery.divisionQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new DivisionModels();
                item.DivisionID = Convert.ToInt32(reader["DivisionID"]);
                item.DivisionName = Convert.ToString(reader["DivisionName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }

        public List<ClientModels> selectClientSalePerson(object connection)
        {
            List<ClientModels> list = new List<ClientModels>();
            ClientModels item;

            SqlCommand cmd = new SqlCommand(TextQuery.clientSalePersonQuery, (SqlConnection)connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = new ClientModels();
                item.ClientID = Convert.ToInt32(reader["ClientID"]);
                item.ClientName = Convert.ToString(reader["ClientName"]);
                list.Add(item);
            }
            reader.Close();

            return list;
        }
    }
}
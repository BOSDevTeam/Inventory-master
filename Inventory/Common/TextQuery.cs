using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class TextQuery
    {
        public const string customerQuery = "Select CustomerID,CustomerName From S_Customer Order By IsDefault DESC";
        public const string locationQuery = "Select LocationID,ShortName From S_Location";
        public const string mainMenuQuery = "Select MainMenuID,MainMenuName From S_MainMenu Order By SortCode";
        public const string unitQuery = "Select UnitID,Keyword From S_Unit Order By ULID";
        public const string currencyQuery = "Select CurrencyID,Keyword From Sys_Currency Order By IsDefault DESC";
        public const string paymentQuery = "Select PaymentID,Keyword From Sys_Payment";
        public const string payMethodQuery = "Select PayMethodID,PayMethodName From Sys_PayMethod";
        public const string limitedDayQuery = "Select LimitedDayID,LimitedDayName From Sys_LimitedDay";
        public const string bankPaymentQuery = "Select BankPaymentID,BankPaymentName From S_BankPayment";

        public string getSubMenuQuery(int mainMenuId)
        {
            return "Select SubMenuID,SubMenuName"
            + " From S_SubMenu Where MainMenuID=" + mainMenuId + " Order By SortCode";
        }

        public string getProductQuery(int subMenuId)
        {
            return "Select ProductID,ProductName,Code,isnull(SalePrice,0) AS SalePrice,isnull(DisPercent,0) AS DisPercent"
            + " From S_Product Where SubMenuID=" + subMenuId + " And IsStock=1 Order By SortCode";
        }

        public string getProductByCodeQuery(string productCode)
        {
            return "Select ProductID,ProductName,isnull(SalePrice,0) AS SalePrice,isnull(DisPercent,0) AS DisPercent"
            + " From S_Product Where Code='" + productCode + "' And IsStock=1";
        }

        public string getVoucherSettingQuery(int locationId)
        {
            return "Select HeaderName,HeaderDesp,HeaderPhone,HeaderAddress,OtherHeader1,OtherHeader2,FooterMessage1,FooterMessage2,FooterMessage3,isnull(VoucherLogo,'') AS VoucherLogo"
            + " From S_VoucherSetting Where LocationID=" + locationId;
        }

        public string deleteSaleQuery(int saleId)
        {
            return "Delete From T_TranSale Where SaleID=" + saleId + " Delete From T_MasterSale Where SaleID=" + saleId;           
        }
    }
}
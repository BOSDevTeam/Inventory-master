using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class TextQuery
    {
        public const string customerQuery = "Select CustomerID,CustomerName From SCustomer Order By IsDefault DESC";
        public const string supplierQuery = "Select SupplierID,SupplierName From SSupplier Order By IsDefault DESC";
        public const string locationQuery = "Select LocationID,ShortName From SLocation";
        public const string mainMenuQuery = "Select MainMenuID,MainMenuName From SMainMenu Order By SortCode";
        public const string unitQuery = "Select UnitID,Keyword From SUnit Order By ULID";
        public const string currencyQuery = "Select CurrencyID,Keyword From SysCurrency Order By IsDefault DESC";
        public const string paymentQuery = "Select PaymentID,Keyword From SysPayment";
        public const string payMethodQuery = "Select PayMethodID,PayMethodName From SysPayMethod";
        public const string limitedDayQuery = "Select LimitedDayID,LimitedDayName From SysLimitedDay";
        public const string bankPaymentQuery = "Select BankPaymentID,BankPaymentName From SBankPayment";
        public const string adjustTypeQuery = "Select AdjustTypeID,ShortName From SAdjustType";
        public const string divisionQuery = "Select DivisionID,DivisionName From SDivision";
        public const string clientSalePersonQuery = "Select ClientID,ClientName From SClient Where IsSalePerson=1";

        public string getSubMenuQuery(int mainMenuId)
        {
            return "Select SubMenuID,SubMenuName"
            + " From SSubMenu Where MainMenuID=" + mainMenuId + " Order By SortCode";
        }
    
        public string getProductQuery(int subMenuId)
        {
            return "Select ProductID,ProductName,Code,isnull(SalePrice,0) AS SalePrice,isnull(DisPercent,0) AS DisPercent,isnull(PurPrice,0) AS PurPrice"
            + " From SProduct Where SubMenuID=" + subMenuId + " And IsStock=1 Order By SortCode";
        }

        public string getProductByCodeQuery(string productCode)
        {
            return "Select ProductID,ProductName,isnull(SalePrice,0) AS SalePrice,isnull(DisPercent,0) AS DisPercent,isnull(PurPrice,0) AS PurPrice"
            + " From SProduct Where Code='" + productCode + "' And IsStock=1";
        }

        public string getSalePriceQuery(int productId)
        {
            return "Select isnull(SalePrice,0) AS SalePrice"
            + " From SProduct Where ProductID=" + productId;
        }

        public string getPurPriceQuery(int productId)
        {
            return "Select isnull(PurPrice,0) AS PurPrice"
            + " From SProduct Where ProductID=" + productId;
        }

        public string getVoucherSettingQuery(int locationId)
        {
            return "Select HeaderName,HeaderDesp,HeaderPhone,HeaderAddress,OtherHeader1,OtherHeader2,FooterMessage1,FooterMessage2,FooterMessage3,isnull(VoucherLogo,'') AS VoucherLogo"
            + " From SVoucherSetting Where LocationID=" + locationId;
        }

        public string deleteSaleQuery(int saleId)
        {
            return "Delete From TTranSale Where SaleID=" + saleId + " Delete From TMasterSale Where SaleID=" + saleId;           
        }

        public string deleteOpenBillQuery(int openBillId)
        {
            return "Delete From TTranOpenBill Where OpenBillID=" + openBillId + " Delete From TMasterOpenBill Where OpenBillID=" + openBillId;
        }

        public string getMasterOpenBillQuery(int openBillId)
        {
            return "Select UserVoucherNo,VoucherID,Subtotal,TaxAmt,ChargesAmt,Total,LocationID,CustomerID"
            + " From TMasterOpenBill Where OpenBillID=" + openBillId;
        }

        public string getCLMasterSaleOrderQuery(int clSaleOrderId)
        {
            return "Select Subtotal,TaxAmt,ChargesAmt,Total,isnull(CustomerID,0) AS CustomerID"
            + " From TCLMasterSaleOrder Where SaleOrderID=" + clSaleOrderId;
        }

        public string getCLTranSaleOrderQuery(int clSaleOrderId)
        {
            return "Select p.ProductName,ts.Quantity,ts.SalePrice,ts.Amount,ts.ProductID,p.Code"
            + " From TCLTranSaleOrder ts INNER JOIN SProduct p ON ts.ProductID=p.ProductID"
            + " Where ts.SaleOrderID=" + clSaleOrderId;
        }

        public string deleteSaleOrderQuery(int saleOrderId)
        {
            return "Delete From TTranSaleOrder Where SaleOrderID=" + saleOrderId + " Delete From TMasterSaleOrder Where SaleOrderID=" + saleOrderId;
        }

        public string deletePurchaseQuery(int purchaseId)
        {
            return "Delete From TTranPurchase Where PurchaseID=" + purchaseId + " Delete From TMasterPurchase Where PurchaseID=" + purchaseId;
        }

        public string deletePurchaseOrderQuery(int purchaseOrderId)
        {
            return "Delete From TTranPurchaseOrder Where PurchaseOrderID=" + purchaseOrderId + " Delete From TMasterPurchaseOrder Where PurchaseOrderID=" + purchaseOrderId;
        }

        public string deleteAdjustmentQuery(int adjustmentId)
        {
            return "Delete From TTranAdjustment Where AdjustmentID=" + adjustmentId + " Delete From TMasterAdjustment Where AdjustmentID=" + adjustmentId;
        }

        public string deleteTransferQuery(int transferId)
        {
            return "Delete From TTranTransfer Where TransferID=" + transferId + " Delete From TMasterTransfer Where TransferID=" + transferId;
        }

        public string deleteCustomerConsignQuery(int customerConsignId)
        {
            return "Delete From TTranCustomerConsign Where CustomerConsignID=" + customerConsignId + " Delete From TMasterCustomerConsign Where CustomerConsignID=" + customerConsignId;
        }

        public string deleteOpeningStockQuery(int openingStockId)
        {
            return "Delete From TTranOpeningStock Where OpeningStockID=" + openingStockId + " Delete From TMasterOpeningStock Where OpeningStockID=" + openingStockId;
        }
    }
}
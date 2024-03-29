﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class TextQuery
    {
        public const string customerQuery = "Select CustomerID,CustomerName From SCustomer Order By IsDefault DESC";
        public const string supplierQuery = "Select SupplierID,SupplierName From SSupplier Order By IsDefault DESC";
        public const string locationQuery = "Select LocationID,ShortName From SLocation Order By LocationID";
        public const string mainMenuQuery = "Select MainMenuID,MainMenuName From SMainMenu Order By SortCode";
        public const string unitQuery = "Select UnitID,Keyword From SUnit Order By ULID";
        public const string currencyQuery = "Select CurrencyID,Keyword From SysCurrency Order By IsDefault DESC";
        public const string paymentQuery = "Select PaymentID,Keyword From SysPayment";
        public const string payMethodQuery = "Select PayMethodID,PayMethodName From SysPayMethod";
        public const string limitedDayQuery = "Select LimitedDayID,LimitedDayName From SysLimitedDay";
        public const string bankPaymentQuery = "Select BankPaymentID,BankPaymentName From SBankPayment";
        public const string adjustTypeQuery = "Select AdjustTypeID,ShortName From SAdjustType";
        public const string divisionQuery = "Select DivisionID,DivisionName From SDivision Order By Code";
        public const string townshipQuery = "Select TownshipID,TownshipName From STownship Order By Code";
        public const string clientSalePersonQuery = "Select ClientID,ClientName From SClient Where IsSalePerson=1";
        public const string clientTokenQuery = "Select Token From SClient";     
        public const string allUserQuery = "Select UserID,UserName,isnull(IsTechnician,0) AS IsTechnician From SUser Order by IsTechnician";
        public const string userQuery = "Select UserID,UserName From SUser Where IsTechnician!=1";
        public const string setupModuleQuery = "Select SetupModuleID,SetupModuleName From SysSetupModule";
        public const string entryModuleQuery = "Select EntryModuleID,EntryModuleName From SysEntryModule";
        public const string reportModuleQuery = "Select ReportModuleID,ReportModuleName From SysReportModule Order by ReportModuleGroupCode";
        public const string permissionQuery = "Select PermissionID,PermissionName From SysPermission";
        public const string staffQuery = "Select StaffID,StaffName From SStaff";

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
            return "Select HeaderName,HeaderDesp,HeaderName2,HeaderDesp2,HeaderPhone,HeaderAddress,OtherHeader1,OtherHeader2,FooterMessage1,FooterMessage2,FooterMessage3,isnull(VoucherLogo,'') AS VoucherLogo"
            + " From SVoucherSetting Where LocationID=" + locationId;
        }

        public string deleteOpenBillQuery(int openBillId)
        {
            return "Delete From TTranOpenBill Where OpenBillID=" + openBillId + " Delete From TMasterOpenBill Where OpenBillID=" + openBillId;
        }

        public string getMasterOpenBillQuery(int openBillId)
        {
            return "Select UserVoucherNo,VoucherID,Subtotal,TaxAmt,ChargesAmt,Total,LocationID,CustomerID,CurrencyID,isnull(StaffID,0) AS StaffID"
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

        public string deletePurchaseReturnQuery(int purchaseReturnId)
        {
            return "Delete From TTranPurchaseReturn Where PurchaseReturnID=" + purchaseReturnId + " Delete From TMasterPurchaseReturn Where PurchaseReturnID=" + purchaseReturnId;
        }

        public string deleteSaleReturnQuery(int saleReturnId)
        {
            return "Delete From TTranSaleReturn Where SaleReturnID=" + saleReturnId + " Delete From TMasterSaleReturn Where SaleReturnID=" + saleReturnId;
        }

        public string deleteCustomerOpeningQuery(int customerOpeningId)
        {
            return "Delete From TTranCustomerOpening Where CustomerOpeningID=" + customerOpeningId + " Delete From TMasterCustomerOpening Where CustomerOpeningID=" + customerOpeningId + " Delete From TMasterLedger Where TranID=" + customerOpeningId + " And AccountCode=" + AppConstants.CustomerOpeningAccountCode;
        }

        public string deleteSupplierOpeningQuery(int supplierOpeningId)
        {
            return "Delete From TTranSupplierOpening Where SupplierOpeningID=" + supplierOpeningId + " Delete From TMasterSupplierOpening Where SupplierOpeningID=" + supplierOpeningId + " Delete From TMasterLedger Where TranID=" + supplierOpeningId + " And AccountCode=" + AppConstants.SupplierOpeningAccountCode;
        }

        public string getSetupModuleAccessQuery(int userId)
        {
            return "Select ur.SetupModuleID,SetupModuleName,IsAllow"
            + " From SSetupUserRight ur Inner Join SysSetupModule sm ON ur.SetupModuleID=sm.SetupModuleID Where UserID=" + userId;
        }

        public string getAllowSetupModuleQuery(int loginUserId)
        {
            return "Select rm.SetupModuleID,SetupModuleName"
            + " From SysSetupModule rm Inner Join SSetupUserRight ur On rm.SetupModuleID=ur.SetupModuleID Where UserID=" + loginUserId + " And IsAllow=1";
        }

        public string getEntryModuleAccessQuery(int userId)
        {
            return "Select ur.EntryModuleID,EntryModuleName,IsAllow"
            + " From SEntryUserRight ur Inner Join SysEntryModule sm ON ur.EntryModuleID=sm.EntryModuleID Where UserID=" + userId;
        }

        public string getAllowEntryModuleQuery(int loginUserId)
        {
            return "Select rm.EntryModuleID,EntryModuleName"
            + " From SysEntryModule rm Inner Join SEntryUserRight ur On rm.EntryModuleID=ur.EntryModuleID Where UserID=" + loginUserId + " And IsAllow=1";
        }

        public string getReportModuleAccessQuery(int userId)
        {
            return "Select ur.ReportModuleID,ReportModuleName,IsAllow"
            + " From SReportUserRight ur Inner Join SysReportModule sm ON ur.ReportModuleID=sm.ReportModuleID Where UserID=" + userId + " Order by ReportModuleGroupCode";
        }

        public string getAllowReportModuleQuery(int loginUserId)
        {
            return "Select rm.ReportModuleID,ReportModuleName"
            + " From SysReportModule rm Inner Join SReportUserRight ur On rm.ReportModuleID=ur.ReportModuleID Where UserID=" + loginUserId + " And IsAllow=1 Order by ReportModuleGroupCode";
        }

        public string getPermissionAccessQuery(int userId)
        {
            return "Select ur.PermissionID,PermissionName,IsAllow"
            + " From SPermissionUserRight ur Inner Join SysPermission sm ON ur.PermissionID=sm.PermissionID Where UserID=" + userId;
        }

        public string getAllowPermissionQuery(int loginUserId)
        {
            return "Select rm.PermissionID,PermissionName"
            + " From SysPermission rm Inner Join SPermissionUserRight ur On rm.PermissionID=ur.PermissionID Where UserID=" + loginUserId + " And IsAllow=1";
        }

        public string updateSaleVoucherDesignQuery(short saleVoucherDesignType)
        {
            return "Update SCompanySetting Set SaleVoucherDesignType=" + saleVoucherDesignType;
        }

        public string getSalePaymentQuery(int saleId)
        {
            return "Select PaymentID,PayMethodID,BankPaymentID,LimitedDayID,Remark,VouDisPercent,VouDisAmount,AdvancedPay,PaymentPercent,isnull(IsVouFOC,0) AS IsVouFOC"
            + " From TMasterSale Where SaleID=" + saleId;
        }

        public string getTownshipQuery(int divisionId)
        {
            return "Select TownshipID,TownshipName"
            + " From STownship Where DivisionID=" + divisionId + " Order By Code";
        }

        public string getMultiPayCashInHand(int saleId)
        {
            return "Select Amount"
            + " From TMultiPayMethodSale Where PayMethodID=1 And SaleID=" + saleId;
        }

        public string getMultiPayBanking(int saleId)
        {
            return "Select ms.BankPaymentID,PaymentPercent,Amount,BankPaymentName"
            + " From TMultiPayMethodSale ms Inner Join SBankPayment bp On ms.BankPaymentID=bp.BankPaymentID Where PayMethodID=2 And SaleID=" + saleId;
        }

        public string getTranPurchaseAccessory(int tranPurchaseId)
        {
            return "Select Gold,Pearl,Diamond,Stone,Palatinum,Photo"
            + " From TTranPurchaseAccessory Where TranPurchaseID=" + tranPurchaseId;
        }
    }
}
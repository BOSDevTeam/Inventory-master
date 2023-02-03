﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class Procedure
    {
        public const string PrcSaveShopType = "PrcSaveShopType";
        public const string PrcGetCompanySetting = "PrcGetCompanySetting";
        public const string PrcSaveOtherSetting = "PrcSaveOtherSetting";
        public const string PrcSearchBank = "PrcSearchBank";
        public const string PrcInsertBank = "PrcInsertBank";
        public const string PrcUpdateBank = "PrcUpdateBank";
        public const string PrcDeleteBank = "PrcDeleteBank";
        public const string PrcSearchBankPayment = "PrcSearchBankPayment";
        public const string PrcInsertBankPayment = "PrcInsertBankPayment";
        public const string PrcUpdateBankPayment = "PrcUpdateBankPayment";
        public const string PrcDeleteBankPayment = "PrcDeleteBankPayment";
        public const string PrcSearchProductByCodeName = "PrcSearchProductByCodeName";     
        public const string PrcValidateUser = "PrcValidateUser";
        public const string PrcGetUser = "PrcGetUser";
        public const string PrcSearchUser = "PrcSearchUser";
        public const string PrcUpdateUser = "PrcUpdateUser";
        public const string PrcInsertUser = "PrcInsertUser";        
        public const string PrcInsertProduct = "PrcInsertProduct";
        public const string PrcUpdateProduct = "PrcUpdateProduct";
        public const string PrcSearchProduct = "PrcSearchProduct";
        public const string PrcGetProduct = "PrcGetProduct";       
        public const string PrcDeleteProduct = "PrcDeleteProduct";
        public const string PrcGetDivision = "PrcGetDivision";
        public const string PrcSearchDivision = "PrcSearchDivision";
        public const string PrcDeleteDivision = "PrcDeleteDivision";
        public const string PrcInsertDivision = "PrcInsertDivision";
        public const string PrcUpdateDivision = "PrcUpdateDivision";
        public const string PrcGetTownship = "PrcGetTownship";
        public const string PrcUpdateCLMasterSaleOrder = "PrcUpdateCLMasterSaleOrder";
        public const string PrcUpdateCLTranSaleOrder = "PrcUpdateCLTranSaleOrder";
        public const string PrcGetUserVoucherNo = "PrcGetUserVoucherNo";
        public const string PrcInsertSale = "PrcInsertSale";
        public const string PrcUpdateSale = "PrcUpdateSale";
        public const string PrcGetMasterSaleBySystemVoucherNo = "PrcGetMasterSaleBySystemVoucherNo";
        public const string PrcGetTranSaleBySaleID = "PrcGetTranSaleBySaleID";
        public const string PrcGetMasterSaleList = "PrcGetMasterSaleList";
        public const string PrcGetMasterSaleBySaleID = "PrcGetMasterSaleBySaleID";
        public const string PrcInsertOpenBill = "PrcInsertOpenBill";
        public const string PrcGetMasterOpenBill = "PrcGetMasterOpenBill";
        public const string PrcGetTranOpenBillByID = "PrcGetTranOpenBillByID";
        public const string PrcDeleteTownship = "PrcDeleteTownship";
        public const string PrcDeleteUser = "PrcDeleteUser";
        public const string PrcGetClient = "PrcGetClient";
        public const string PrcSearchClient = "PrcSearchClient";
        public const string PrcDeleteCustomer = "PrcDeleteCustomer";
        public const string PrcDeleteLocation = "PrcDeleteLocation";
        public const string PrcInsertSaleOrder = "PrcInsertSaleOrder";
        public const string PrcGetMasterSaleOrderList = "PrcGetMasterSaleOrderList";
        public const string PrcGetMasterSaleOrderBySaleOrderID = "PrcGetMasterSaleOrderBySaleOrderID";
        public const string PrcGetTranSaleOrderBySaleOrderID = "PrcGetTranSaleOrderBySaleOrderID";
        public const string PrcUpdateSaleOrder = "PrcUpdateSaleOrder";
        public const string PrcInsertPurchaseOrder = "PrcInsertPurchaseOrder";
        public const string PrcGetMasterPurchaseOrderList = "PrcGetMasterPurchaseOrderList";
        public const string PrcGetMasterPurOrderByPurOrderID = "PrcGetMasterPurOrderByPurOrderID";
        public const string PrcGetTranPurOrderByPurOrderID = "PrcGetTranPurOrderByPurOrderID";
        public const string PrcUpdatePurchaseOrder = "PrcUpdatePurchaseOrder";
        public const string PrcInsertPurchase = "PrcInsertPurchase";
        public const string PrcGetMasterPurchaseList = "PrcGetMasterPurchaseList";
        public const string PrcGetMasterPurchaseByPurchaseID = "PrcGetMasterPurchaseByPurchaseID";
        public const string PrcGetTranPurchaseByPurchaseID = "PrcGetTranPurchaseByPurchaseID";
        public const string PrcUpdatePurchase = "PrcUpdatePurchase";
        public const string PrcUpdateClient = "PrcUpdateClient";
        public const string PrcDeleteClient = "PrcDeleteClient";
        public const string PrcInsertAdjustment = "PrcInsertAdjustment";
        public const string PrcGetMasterAdjustmentList = "PrcGetMasterAdjustmentList";
        public const string PrcGetMasterAdjustmentByAdjustmentID = "PrcGetMasterAdjustmentByAdjustmentID";
        public const string PrcGetTranAdjustmentByAdjustmentID = "PrcGetTranAdjustmentByAdjustmentID";
        public const string PrcUpdateAdjustment = "PrcUpdateAdjustment";
        public const string PrcInsertOpeningStock = "PrcInsertOpeningStock";
        public const string PrcUpdateOpeningStock = "PrcUpdateOpeningStock";
        public const string PrcGetMasterOpeningStockList = "PrcGetMasterOpeningStockList";
        public const string PrcGetMasterOpeningStockByID = "PrcGetMasterOpeningStockByID";
        public const string PrcGetTranOpeningStockByID = "PrcGetTranOpeningStockByID";
        public const string PrcInsertTransfer = "PrcInsertTransfer";
        public const string PrcGetMasterTransferList = "PrcGetMasterTransferList";
        public const string PrcGetMasterTransferByTransferID = "PrcGetMasterTransferByTransferID";
        public const string PrcGetTranTransferByTransferID = "PrcGetTranTransferByTransferID";
        public const string PrcUpdateTransfer = "PrcUpdateTransfer";
        public const string PrcInsertCustomerConsign = "PrcInsertCustomerConsign";
        public const string PrcGetMasterCustomerConsignList = "PrcGetMasterCustomerConsignList";
        public const string PrcGetMasterCustomerConsignByCustomerConsignID = "PrcGetMasterCustomerConsignByCustomerConsignID";
        public const string PrcGetTranCustomerConsignByCustomerConsignID = "PrcGetTranCustomerConsignByCustomerConsignID";
        public const string PrcUpdateCustomerConsign = "PrcUpdateCustomerConsign";
        public const string PrcGetStockStatusByLocation = "PrcGetStockStatusByLocation";
        public const string PrcGetMasterANDTranSaleByPaymentID = "PrcGetMasterANDTranSaleByPaymentID";
        public const string PrcGetMasterPurchaseByPurchaseVoucherNo = "PrcGetMasterPurchaseByPurchaseVoucherNo";
        public const string PrcInsertPurchaseReturn = "PrcInsertPurchaseReturn";
        public const string PrcGetMasterPurchaseReturnList = "PrcGetMasterPurchaseReturnList";
        public const string PrcGetMasterPurchaseReturnByPurchaseReturnID = "PrcGetMasterPurchaseReturnByPurchaseReturnID";
        public const string PrcGetTranPurchaseReturnByPurchaseReturnID = "PrcGetTranPurchaseReturnByPurchaseReturnID";
        public const string PrcDeletePurchaseReturn = "PrcDeletePurchaseReturn";
        public const string PrcUpdatePurchaseReturn = "PrcUpdatePurchaseReturn";
        public const string PrcInsertSaleReturn = "PrcInsertSaleReturn";
        public const string PrcGetMasterSaleReturnBySaleReturnID = "PrcGetMasterSaleReturnBySaleReturnID";
        public const string PrcGetTranSaleReturnBySaleReturnID = "PrcGetTranSaleReturnBySaleReturnID";
        public const string PrcGetMasterSaleReturnList = "PrcGetMasterSaleReturnList";
        public const string PrcGetRptSaleItemList = "PrcGetRptSaleItem";
        public const string PrcUpdateSaleReturn = "PrcUpdateSaleReturn";
        public const string PrcGetRptMasterSaleList = "PrcGetRptMasterSaleList";
        public const string PrcGetRptTranSaleList = "PrcGetRptTranSaleList";
        public const string PrcGetMenuData = "PrcGetMenuData";
        public const string PrcGetRptSaleItemSimple = "PrcGetRptSaleItemSample";
        public const string PrcGetRptSaleAmountOnly = "PrcGetRptSaleAmountOnly";
        public const string PrcGetRptSaleAmountSummary = "PrcGetRptSaleAmountSummary";
        public const string PrcInsertCustomerOpening = "PrcInsertCustomerOpening";
        public const string PrcGetMasterCustomerOpeningList = "PrcGetMasterCustomerOpeningList";
        public const string PrcGetMasterCustomerOpeningByID = "PrcGetMasterCustomerOpeningByID";
        public const string PrcGetTranCustomerOpeningByID = "PrcGetTranCustomerOpeningByID";
        public const string PrcUpdateCustomerOpening = "PrcUpdateCustomerOpening";
        public const string PrcGetCustomerOutstanding = "PrcGetCustomerOutstanding";
        public const string PrcGetTranCustomerOutstanding = "PrcGetTranCustomerOutstanding";
        public const string PrcGetRptSaleItemByCustomer = "PrcGetRptSaleItemByCustomer";
        public const string PrcGetRptSaleAmountByCustomer = "PrcGetRptSaleAmountByCustomer";
        public const string PrcGetRptBankPayment = "PrcGetRptBankPayment";
        public const string PrcGetRptTopSaleItem = "PrcGetRptTopSaleItem";
        public const string PrcGetRptBottomSale = "PrcGetRptBottomSale";
        public const string PrcInsertCustomerOutstandingPayment = "PrcInsertCustomerOutstandingPayment";
        public const string PrcGetCustomerOutstandingHistory = "PrcGetCustomerOutstandingHistory";
        public const string PrcGetTranCustomerOutstandingHistory = "PrcGetTranCustomerOutstandingHistory";
        public const string PrcGetRptPurchaseItemSimple = "PrcGetRptPurchaseItemSimple";
        public const string PrcGetRptMasterPurchaseList = "PrcGetRptMasterPurchaseList";
        public const string PrcGetRptTranPurchaseList = "PrcGetRptTranPurchaseList";
    }
}
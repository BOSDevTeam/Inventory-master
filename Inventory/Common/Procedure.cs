using System;
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
    }
}
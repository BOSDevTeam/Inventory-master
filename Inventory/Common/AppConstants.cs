using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class AppConstants
    {
        //App Modules
        public const int SaleModule = 1;  //SysModule table value
        public const int PurchaseModule = 2;
        public const int SaleOrderModule = 3;
        public const int PurchaseOrderModule = 4;
        public const int SaleReturnModule = 5;
        public const int PurchaseReturnModule = 6;
        public const int OpeningStockModule = 7;
        public const int AdjustmentModule = 8;
        public const int TransferModule = 9;
        public const int CustomerConsignModule = 10;
        public const int CustomerOpeningModule = 11;
        public const int CustomerOutstandingModule = 12;
        public const int SupplierOpeningModule = 13;
        public const int SupplierOutstandingModule = 14;

        //Account Codes
        public const int CustomerOpeningAccountCode = 110;  //SysAccount table value
        public const int SaleAccountCode = 210;         
        public const int ARAccountCode = 310;
        public const int SupplierOpeningAccountCode = 410;
        public const int PurchaseAccountCode = 510;
        public const int APAccountCode = 610;

        //Action Codes and Names
        public const string NewActionCode = "1";  //SysAction table value
        public const string EditActionCode = "2"; 
        public const string DeleteActionCode = "3";
        public const string NewActionName = "New";
        public const string EditActionName = "Edit";
        public const string DeleteActionName = "Delete";

        //Firebase Notification
        public const string CloudMessagingServerKey = "AAAAm-35J2M:APA91bG7cMqe-P2pkXZsNPn81sz9e0vwEWNKmiPPtblzfGhQYvqE8_JOBCm76cCI9lFyI0Gcglr104CfxkvKqdxs0vITi2sn759ytppjtNQOF3Kyy-A6RATXwmdgLvVEQIj3GSWWQ7PS";  
        public static Uri FirebasePushNotificationsURL = new Uri("https://fcm.googleapis.com/fcm/send");
        public const string UpdateOrderTitle = "Updated";
        public const string UpdateOrderBody = "Updated Your Order #";
        public const string NewProductTitle = "New Product";
        public const string NewProductBody = " Available Now";

        //Session Values
        public const string SQLConnection = "SQLConnection";

        //Drop Down Default Values
        public const string AllCustomer = "All Customer";
        public const string AllSupplier = "All Supplier";
        public const string SelectLocation = "Select Location";
        public const string Division = "Division";
        public const string SalePerson = "Sale Person";

        public enum RequestUnSuccessful
        {
            SessionExpired, InCompletedData, UnExpectedError
        }

        public class Message
        {
            public const string SaveSuccessOpenBill = "Saved Successfully to Opened Bills!";
            public const string DeleteSuccess = "Deleted Successfully!";
            public const string SaveSuccess = "Saved Successfully!";
            public const string NoProductByCode = "Not found product with this code!";
            public const string NoProduct = "Not found product!";
            public const string NoTransferSameLocation = "Not allow to transfer by same location!";
            public const string NoOpeningStockSameLocation = "Already have opening stock with this location!";
            public const string NoOpeningStockSameSubMenu = "Already added products for this submenu!";
            public const string PurReturnQtyCheck = "Return Quantity should not greater than Purchase Quantity";
            public const string SaleReturnQtyCheck = "Return Quantity should not greater than Sale Quantity";
            public const string NoReturnFOCVoucher = "Not allow to return FOC Voucher!";
            public const string NoReturnCreditVoucher = "Not allow to return Credit Voucher!";
            public const string AlreadyAddCustomer = "Already added customer!";
            public const string InvalidPayment = "Invalid Payment";
            public const string FillOutstandingPayment = "Fill Outstanding Payment";
            public const string PaymentSuccess = "Payment Successful";
            public const string SessionExpired = "Session Expired";
            public const string UpdateSuccess = "Updated Successfully!";
            public const string NoDeleteByOutstanding = "Not allow to delete! This voucher has payments in outstanding!";
            public const string NoEditByOutstanding = "Not allow to edit! This voucher has payments in outstanding!";
            public const string AlreadyAddSupplier = "Already added supplier!";
        }
    }
}
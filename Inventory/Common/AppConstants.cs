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

        //Firebase Notification
        public const string CloudMessagingServerKey = "AAAAm-35J2M:APA91bG7cMqe-P2pkXZsNPn81sz9e0vwEWNKmiPPtblzfGhQYvqE8_JOBCm76cCI9lFyI0Gcglr104CfxkvKqdxs0vITi2sn759ytppjtNQOF3Kyy-A6RATXwmdgLvVEQIj3GSWWQ7PS";  //SysModule table value
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
        }
    }
}
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

        //Session Values
        public const string SQLConnection = "SQLConnection";

        //Drop Down Default Values
        public const string AllCustomer = "All Customer";
        public const string AllSupplier = "All Supplier";
        public const string SelectLocation = "Select Location";
        public const string Division = "Division";
        public const string SalePerson = "Sale Person";
    }
}
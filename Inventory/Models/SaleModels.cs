using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class SaleModels
    {
        public class SaleModel
        {
            public SaleModel()
            {
                this.Customers = new List<SelectListItem>();
                this.Payments = new List<SelectListItem>();
                this.Currencies = new List<SelectListItem>();
                this.Locations = new List<SelectListItem>();
                this.Units = new List<SelectListItem>();
                this.Variants = new List<SelectListItem>();
                this.MainMenus = new List<SelectListItem>();
                this.SubMenus = new List<SelectListItem>();
                this.Products = new List<SelectListItem>();
                this.SaleList = new List<SaleListModel>();
                this.CurrentSaleList = new List<CurrentSaleModel>();
                this.DeliveryList = new List<DeliveryModel>();
                this.PayMethods = new List<SelectListItem>();
                this.BankPayments = new List<SelectListItem>();
            }
            public List<CurrentSaleModel> CurrentSaleList { get; set; }
            public List<DeliveryModel> DeliveryList { get; set; }
            public List<SaleListModel> SaleList { get; set; }
            public DateTime Date { get; set; }
            public string Voucher { get; set; }
            public int UserID { get; set; }
            public int CustomerID { get; set; }
            public int LocationID { get; set; }
            public int PaymentID { get; set; }
            public int CurrencyID { get; set; }
            public int MainMenuID { get; set; }
            public int SubMenuID { get; set; }
            public bool IsDelivery { get; set; }
            public double VoucherDis { get; set; }
            public double AdvancedPayAmt { get; set; }
            public double PaidAmt { get; set; }
            public double Paid { get; set; }
            public double ChangeAmt { get; set; }
            public double FOCAmt { get; set; }
            public double TaxAmt { get; set; }
            public double ChargesAmt { get; set; }
            public double TotalAmt { get; set; }
            public double NetAmt { get; set; }
            public int CreditLimitDay { get; set; }
            public List<SelectListItem> Customers { get; set; }
            public List<SelectListItem> Locations { get; set; }
            public List<SelectListItem> Payments { get; set; }
            public List<SelectListItem> Currencies { get; set; }
            public List<SelectListItem> Units { get; set; }
            public List<SelectListItem> Variants { get; set; }
            public List<SelectListItem> MainMenus { get; set; }
            public List<SelectListItem> SubMenus { get; set; }
            public List<SelectListItem> Products { get; set; }
            public List<SelectListItem> PayMethods { get; set; }
            public List<SelectListItem> BankPayments { get; set; }
            public int CurrentSale { get; set; }
            public string Code { get; set; }
            public string SearchCode { get; set; }
            public string ProductName { get; set; }
            public int? UnitID { get; set; }
            public string Keyword { get; set; }
            public string Variant { get; set; }
            public double SalePrice { get; set; }
            public int Quantity { get; set; }
            public int DisPercent { get; set; }
            public string SubMenuName { get; set; }
            public double PayDisAmount { get; set; }
            public int PayDisPercent { get; set; }

            [Required(ErrorMessage = "Please enter Delivery Date.")]
            public DateTime DeliveryDate { get; set; }

            [Required(ErrorMessage = "Please enter Recipient.")]
            public string DeliveryRecipient { get; set; }

            [Required(ErrorMessage = "Please enter Contact Phone.")]
            public string DeliveryPhone { get; set; }

            [Required(ErrorMessage = "Please enter Delivery Address.")]
            public string DeliveryAddress { get; set; }

            public bool IsUnit { get; set; }
            public string UnitName { get; set; }
            public int BankPaymentID { get; set; }
            public int PayMethodID { get; set; }
        }

        public class ProductModel
        {
            public ProductModel()
            {
                this.ProductUnitList = new List<ProductUnitModel>();
                this.ProductVariantList = new List<ProductVariantModel>();
            }
            public int ProductID { get; set; }
            public string Code { get; set; }
            public string ProductName { get; set; }
            public byte[] Photo { get; set; }
            public string Base64Photo { get; set; }
            public double SalePrice { get; set; }
            public int Quantity { get; set; }
            public int DisPercent { get; set; }
            public List<SelectListItem> Units { get; set; }
            public List<SelectListItem> Variants { get; set; }
            public bool? IsUnit { get; set; }
            public List<ProductUnitModel> ProductUnitList { get; set; }
            public bool? IsVariant { get; set; }
            public List<ProductVariantModel> ProductVariantList { get; set; }
            public bool IsPhoto { get; set; }
        }

        public class MainMenuModel
        {
            public int MainMenuID { get; set; }
            public string MainMenuName { get; set; }
        }

        public class SubMenuModel
        {
            public int SubMenuID { get; set; }
            public string SubMenuName { get; set; }
        }

        public class CustomerModel
        {
            public int CustomerID { get; set; }
            public string CustomerName { get; set; }
        }

        public class PaymentModel
        {
            public int PaymentID { get; set; }
            public string PayKeyword { get; set; }
        }

        public class LocationModel
        {
            public int LocationID { get; set; }
            public string LocShortName { get; set; }
        }

        public class CurrencyModel
        {
            public int CurrencyID { get; set; }
            public string CurKeyword { get; set; }
        }

        public class CurrentSaleModel
        {
            public int SortID { get; set; }
            public int? ProductID { get; set; }
            public string Code { get; set; }
            public string ProductName { get; set; }
            public int? UnitID { get; set; }
            public string UnitKeyword { get; set; }
            public string Variant { get; set; }
            public double SalePrice { get; set; }
            public int? Quantity { get; set; }
            public Int16 DisPercent { get; set; }
            public double Amount { get; set; }
            public double DiscountAmt { get; set; }
            public string UnitVariant { get; set; }
            public int VariantID { get; set; }
            public int ID { get; set; }
        }

        public class SaleListModel
        {
            public int TranID { get; set; }
            public DateTime? Date { get; set; }
            public string StrDate { get; set; }
            public DateTime? ReturnDate { get; set; }
            public string Voucher { get; set; }
            public int UserID { get; set; }
            public int CustomerID { get; set; }
            public int LocationID { get; set; }
            public int PaymentID { get; set; }
            public int CurrencyID { get; set; }
            public double NetAmt { get; set; }
            public string UserName { get; set; }
            public string CustomerName { get; set; }
            public string LocShortName { get; set; }
            public string PayKeyword { get; set; }
            public string CurrencyKeyword { get; set; }
            public int? CreditLimitDay { get; set; }
            public bool? IsDelivery { get; set; }
            public double TotalAmt { get; set; }
            public double TaxAmt { get; set; }
            public double ChargesAmt { get; set; }
            public double FOCAmt { get; set; }
            public double AdvancedPayAmt { get; set; }
            public double VoucherDis { get; set; }
            public int? PaymentPercent { get; set; }
        }

        public class ProductUnitModel
        {
            public int? UnitID { get; set; }
            public string Keyword { get; set; }
            public double SalePrice { get; set; }
            public short? DisPercent { get; set; }
        }

        public class ProductVariantModel
        {
            public int? ID { get; set; }
            public string Variant { get; set; }
        }

        public class VoucherSettingModel
        {
            public int ID { get; set; }
            public int BranchID { get; set; }
            public int LocationID { get; set; }
            public byte[] VoucherLogo { get; set; }
            public string HeaderName { get; set; }
            public string Base64Photo { get; set; }
            public string HeaderDesp { get; set; }
            public string HeaderPhone { get; set; }
            public string HeaderAddress { get; set; }
            public string OtherHeader1 { get; set; }
            public string OtherHeader2 { get; set; }
            public string FooterMessage1 { get; set; }
            public string FooterMessage2 { get; set; }
            public string FooterMessage3 { get; set; }
        }

        public class DeliveryModel
        {
            public int DeliveryID { get; set; }
            public int TranID { get; set; }
            public DateTime? Date { get; set; }
            public string Recipient { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Models
{
    public class MasterSaleModels
    {
        public int SaleID { get; set; }
        public string SystemVoucherNo { get; set; }
        public string UserVoucherNo { get; set; }
        public string VoucherID { get; set; }
        public string SaleDateTime { get; set; }
        public int UserID { get; set; }
        public int CustomerID { get; set; }
        public int LocationID { get; set; }
        public int CurrencyID { get; set; }
        public int PaymentID { get; set; }
        public int VoucherDiscount { get; set; }
        public int AdvancedPay { get; set; }
        public int Tax { get; set; }
        public int TaxAmt { get; set; }
        public int Charges { get; set; }
        public int ChargesAmt { get; set; }
        public int Subtotal { get; set; }
        public int Total { get; set; }
        public int Grandtotal { get; set; }
        public int VouDisPercent { get; set; }
        public int VouDisAmount { get; set; }
        public int PayMethodID { get; set; }
        public int BankPaymentID { get; set; }
        public int PaymentPercent { get; set; }
        public bool IsClientSale { get; set; }
        public int ClientID { get; set; }
        public int LimitedDayID { get; set; }
        public int PayPercentAmt { get; set; }
        public string Remark { get; set; }
        public int SlipID { get; set; }
        public bool IsVouFOC { get; set; }
        public int StaffID { get; set; }
        public bool IsMultiPay { get; set; }
        public int CashInHand { get; set; }
        public int BankingTotal { get; set; }
    }
}
﻿using Inventory.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class AppSetting
    {       
        public class Paging
        {
            public int eachItemCount = 20; // item count in each paging
            public int lastItemCount; // item count in last paging
            public int startItemIndex, endItemIndex;
        }
        
        public DateTime convertStringToDate(string date)  // input dd/MM/yyyy
        {
            string inputDate = @date;
            DateTime dateTime = DateTime.ParseExact(inputDate, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dateOnly = dateTime.Date;

            return dateOnly;
        }

        public string convertDateToString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");  // output yyyy-MM-dd
        }

        public DateTime getLocalDate()
        {
            DateTime MyanmarStd = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Myanmar Standard Time");
            return MyanmarStd.Date;
        }

        public DateTime getLocalDateTime()
        {
            DateTime MyanmarStd = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Myanmar Standard Time");
            return MyanmarStd;
        }

    }
}
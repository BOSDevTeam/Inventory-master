using Inventory.ViewModels;
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
            public int eachItemCount = 15; // item count in each paging
            public int lastItemCount; // item count in last paging
            public int startItemIndex, endItemIndex;
        }
        
        public DateTime convertStringToDate(string date)  // input dd/MM/yyyy
        {
            // change input to MM/dd/yyyy        
            string[] arr = date.Split('/');
            string day = arr[0];
            string month = arr[1];
            string year = arr[2];
            date = month + "/" + day + "/" + year;

            // parse string to datetime format
            DateTime dateTime = DateTime.Parse(date);
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
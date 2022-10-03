using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class Setting
    {
        // paging
        public int pageSize = 30;
        public int left, startRowIndex, endRowIndex;

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

    }
}
using Inventory.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Inventory.Common
{
    public class AppSetting
    {
        public SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString());

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

        public async Task<bool> sendPushNotification(string deviceToken, string title, string body, object data)
        {
            bool isSent = false;

            var messageInformation = new NotificationData()
            {
                to = deviceToken,
                notification = new Notification()
                {
                    title = title,
                    body = body
                }
                //data = data,
            };

            //Object to JSON STRUCTURE => using Newtonsoft.Json;
            string jsonMessage = JsonConvert.SerializeObject(messageInformation);
            var request = new HttpRequestMessage(HttpMethod.Post, AppConstants.FirebasePushNotificationsURL);

            request.Headers.TryAddWithoutValidation("Authorization", "key=" + AppConstants.CloudMessagingServerKey);
            request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                result = await client.SendAsync(request);
                isSent = isSent && result.IsSuccessStatusCode;
            }
            return isSent;
        }

        public async Task<bool> sendPushNotification(List<string> deviceToken, string title, string body, object data)
        {
            bool isSent = false;

            var messageInformation = new NotificationData()
            {
                registration_ids = deviceToken,
                notification = new Notification()
                {
                    title = title,
                    body = body
                }
                //data = data,
            };

            //Object to JSON STRUCTURE => using Newtonsoft.Json;
            string jsonMessage = JsonConvert.SerializeObject(messageInformation);
            var request = new HttpRequestMessage(HttpMethod.Post, AppConstants.FirebasePushNotificationsURL);

            request.Headers.TryAddWithoutValidation("Authorization", "key=" + AppConstants.CloudMessagingServerKey);
            request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                result = await client.SendAsync(request);
                isSent = isSent && result.IsSuccessStatusCode;
            }
            return isSent;
        }

    }
}
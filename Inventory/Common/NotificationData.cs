using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class NotificationData
    {
        public string to { get; set; }
        public Notification notification { get; set; }
        public object data { get; set; }
        public List<string> registration_ids { get; set; }
    }

    public class Notification
    {
        public string title { get; set; }
        public string body { get; set; }
    }
}

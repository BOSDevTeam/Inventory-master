using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class ReportViewModel
    {
        public string ReportName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string HtmlGroupElementID { get; set; }
        public string HtmlActiveElementID { get; set; }
        public bool IsShowMenuFilter { get; set; }
        public bool IsShowPaymentFilter { get; set; }
        public bool IsShowUserClientFilter { get; set; }
        public bool IsShowLogFilter { get; set; }
    }
}
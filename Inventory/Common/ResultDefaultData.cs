using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class ResultDefaultData
    {
        public ResultDefaultData()
        {
            IsRequestSuccess = false;
            UnSuccessfulReason = AppConstants.RequestUnSuccessful.UnExpectedError.ToString();
            Message = AppConstants.Message.SomethingWentWrong;
        }
        public bool IsRequestSuccess { get; set; }
        public string UnSuccessfulReason { get; set; }
        public string Message { get; set; }
    }
}

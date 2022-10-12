using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.ViewModels
{
    public class PagingViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPageNum { get; set; }
        public int StartItemIndex { get; set; }
        public int EndItemIndex { get; set; }
    }
}
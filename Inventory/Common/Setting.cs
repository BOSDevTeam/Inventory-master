using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class Setting
    {
        // paging
        public int pageSize = 30;
        public int left, startRowIndex, endRowIndex;
    }
}
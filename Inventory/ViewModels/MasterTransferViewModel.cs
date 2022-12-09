using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class MasterTransferViewModel
    {
        public MasterTransferViewModel()
        {
            this.MasterTransferModel = new MasterTransferModels();
        }
        public MasterTransferModels MasterTransferModel { get;set;}
        public string UserName { get; set; }
        public int FromLocationID { get; set; }
        public string FromLocationName { get; set; }
        public int ToLocationID { get; set; }
        public string ToLocationName { get; set; }
        public string TransferDateTime { get; set; }
        public string UserVoucherNo { get; set; }
        public int VoucherID { get; set; }
        public string Remark { get; set; }
    }
}
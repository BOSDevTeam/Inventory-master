using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class BankModels
    {
        public BankModels()
        {
            this.LstBank = new List<BankModels>();
        }
        public int BankID { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public List<BankModels> LstBank { get; set; }
    }
}
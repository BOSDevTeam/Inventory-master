//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inventory
{
    using System;
    
    public partial class PrcGetUser_Result
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> LocationID { get; set; }
        public bool IsDefaultLocation { get; set; }
        public string BranchName { get; set; }
        public string LocationName { get; set; }
    }
}

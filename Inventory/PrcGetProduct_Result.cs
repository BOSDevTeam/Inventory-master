
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
    
public partial class PrcGetProduct_Result
{

    public int ProductID { get; set; }

    public string ProductName { get; set; }

    public string Code { get; set; }

    public int SortCode { get; set; }

    public string Description { get; set; }

    public int SubMenuID { get; set; }

    public Nullable<decimal> PurPrice { get; set; }

    public Nullable<decimal> SalePrice { get; set; }

    public Nullable<decimal> WholeSalePrice { get; set; }

    public bool IsUnit { get; set; }

    public bool IsStock { get; set; }

    public byte[] Photo { get; set; }

    public Nullable<int> AlertQty { get; set; }

    public Nullable<short> DisPercent { get; set; }

    public string Barcode { get; set; }

    public string QRcode { get; set; }

    public bool IsVariant { get; set; }

    public int MainMenuID { get; set; }

    public string MainMenuName { get; set; }

    public string SubMenuName { get; set; }

}

}

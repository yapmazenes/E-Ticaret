//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZeonTicaret.WebUI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SatisDetay
    {
        public int SatisID { get; set; }
        public int UrunID { get; set; }
        public Nullable<int> Adet { get; set; }
        public Nullable<decimal> Fiyat { get; set; }
        public Nullable<double> Indirim { get; set; }
    
        public virtual Sati Sati { get; set; }
        public virtual Urun Urun { get; set; }
    }
}

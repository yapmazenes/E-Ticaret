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
    
    public partial class Bildirim
    {
        public int BildirimId { get; set; }
        public string Adi { get; set; }
        public string Detay { get; set; }
        public System.Guid KullaniciID { get; set; }
        public Nullable<System.DateTime> BildirimTarihi { get; set; }
        public Nullable<bool> OkunduMu { get; set; }
    
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}

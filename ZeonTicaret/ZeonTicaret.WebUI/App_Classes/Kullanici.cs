using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _191117Mvc_Template.App_Classes
{
    public class Kullanici
    {
        public string KullaniciAdi { get; set; }
        public Guid Idkey { get; set; }
        public string Parola { get; set; }
        public string Email { get; set; }
        public string Gizlisoru { get; set; }
        public string Gizlicevap { get; set; }
        public int ResimID { get; set; }
    }
}
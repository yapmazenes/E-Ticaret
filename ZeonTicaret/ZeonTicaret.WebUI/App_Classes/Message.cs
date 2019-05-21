using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZeonTicaret.WebUI.App_Classes
{
    public class Message
    {
        public Guid ToID { get; set; }
        public string Baslik { get; set; }
        public string Konu { get; set; }
        public string Mesaj { get; set; }
    }
}
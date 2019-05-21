using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZeonTicaret.WebUI.Models;
namespace ZeonTicaret.WebUI
{
    public class Context
    {
        private static Entities baglanti;

        public static Entities Baglanti
        {
            get
            {
                if (baglanti == null)
                    baglanti = new Entities();
                return baglanti;
            }
            set { baglanti = value; }
        }

    }
}
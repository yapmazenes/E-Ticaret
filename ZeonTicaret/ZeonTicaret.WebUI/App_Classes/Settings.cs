using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ZeonTicaret.WebUI.App_Classes
{
    public class Settings
    {
        public static Size UrunOrtaBoyut
        {
            get
            {
                Size sz = new Size();
                sz.Width = Convert.ToInt32(ConfigurationManager.AppSettings["UrunOrtaWidth"]);
                sz.Height = Convert.ToInt32(ConfigurationManager.AppSettings["UrunOrtaHeight"]);
                return sz;
                ;

            }
        }
        public static Size UrunBuyukBoyut
        {
            get
            {
                Size szb = new Size();
                szb.Width = Convert.ToInt32(ConfigurationManager.AppSettings["UrunBuyukWidth"]);
                szb.Height = Convert.ToInt32(ConfigurationManager.AppSettings["UrunBuyukHeight"]);
                return szb;
            }
        }

        public static Size SliderResimBoyut
        {
            get
            {
                Size szb = new Size();
                szb.Width = Convert.ToInt32(ConfigurationManager.AppSettings["SliderWidth"]);
                szb.Height = Convert.ToInt32(ConfigurationManager.AppSettings["SliderHeight"]);
                return szb;
            }
        }
    }
}
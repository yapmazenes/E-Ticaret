using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZeonTicaret.WebUI.App_Classes;
using ZeonTicaret.WebUI.Models;

namespace ZeonTicaret.WebUI.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Sepet()
        {
            if (HttpContext.Session["AktifSepet"] != null)

                return PartialView((Sepet)HttpContext.Session["AktifSepet"]);
            else

                return PartialView();
        }
        public PartialViewResult Slider()
        {
            var data = Context.Baglanti.Resims.Where(x => x.BuyukYol.Contains("SliderResim"));
            return PartialView(data);

        }
        public PartialViewResult YeniUrunler()
        { //1.36 dk 
            var data = Context.Baglanti.Uruns.ToList();

            return PartialView(data);
        }
        public PartialViewResult Servis()
        {
            return PartialView();
        }
        //public PartialViewResult Moda()
        //{
        //    return PartialView();
        //}
        public PartialViewResult Markalar()
        {
            var data = Context.Baglanti.Markas.ToList();
            return PartialView(data);
        }
        [HttpPost]
        public ActionResult SliderResimEkle(HttpPostedFileBase fileUpload)
        {
            if (fileUpload != null)
            {
                Image img = Image.FromStream(fileUpload.InputStream);
                Bitmap bmp = new Bitmap(img, App_Classes.Settings.SliderResimBoyut);
                string yol = "/Content/SliderResim/" + Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);
                bmp.Save(Server.MapPath(yol));
                Resim rsm = new Resim();
                rsm.BuyukYol = yol;
                Context.Baglanti.Resims.Add(rsm);
                Context.Baglanti.SaveChanges();
            } return RedirectToAction("SliderResim", "Admin");


        }
        public decimal SepeteEkle(int id, int adet = 1, int oncekiAdet = 0)
        {

            SepetItem si = new SepetItem();
            Urun u = Context.Baglanti.Uruns.FirstOrDefault(x => x.Id == id);
            si.Urun = u;
            si.Adet = adet;
            adet = adet - oncekiAdet;
            si.Indirim = 0;
            Sepet s = new Sepet();
            for (int i = 1; i <= adet; i++)
            {
                s.SepeteEkle(si);
            }

            return si.Tutar;



        }
        public decimal SepetUrunAdetDusur(int id, int adet = 1)
        {
            Sepet s = (Sepet)HttpContext.Session["AktifSepet"];
            SepetItem si = s.Urunler.FirstOrDefault(x => x.Urun.Id == id);
            int oncekiadet = s.Urunler.FirstOrDefault(x => x.Urun.Id == id).Adet;
            if (oncekiadet > 1)
            {
                int fark = oncekiadet - adet;
                for (int i = 0; i < fark; i++)
                {
                    s.Urunler.FirstOrDefault(x => x.Urun.Id == id).Adet--;

                }
                return si.Tutar;

            }
            else
            {

                s.Urunler.Remove(si);
                return 0;
            }

        }
        public ActionResult UrunDetay(int id)
        {

            Urun u = Context.Baglanti.Uruns.FirstOrDefault(x => x.Id == id);
            List<UrunOzellik> uoz = Context.Baglanti.UrunOzelliks.Where(x => x.UrunID == id).ToList();
            HashSet<OzellikTip> tips = new HashSet<OzellikTip>();
            HashSet<OzellikDeger> degers = new HashSet<OzellikDeger>();
            foreach (UrunOzellik item in uoz)
            {
                OzellikTip ot = Context.Baglanti.OzellikTips.FirstOrDefault(x => x.Id == item.OzellikTipID);
                tips.Add(ot);
                OzellikDeger od = Context.Baglanti.OzellikDegers.FirstOrDefault(x => x.OzellikTipID == item.OzellikTipID && x.Id == item.OzellikDegerID);
                degers.Add(od);
            }

            ViewBag.OzellikTip = tips;
            ViewBag.OzellikDeger = degers;
            return View(u);

        }
        public ActionResult SatisTamamla()
        {
            if (HttpContext.Session["AktifSepet"] != null)
            {
                Sepet s = (Sepet)HttpContext.Session["AktifSepet"];
                List<SepetItem> sepetItems = s.Urunler;
                ViewBag.ToplamTutar = Convert.ToDouble(s.ToplamTutar);
                ViewBag.ToplamKDV = Convert.ToDouble(s.ToplamTutar) * (0.18);
                ViewBag.Toplam = ViewBag.ToplamTutar + ViewBag.ToplamKDV;
                return View(sepetItems);
            }
            else
            {
                return View();
            }

        }
        public void BildirimOkundu()
        {
            Guid id=KullaniciID();
            List<Bildirim> BildirimList = Context.Baglanti.Bildirims.Where(x => x.OkunduMu == false && x.KullaniciID == id).ToList();
            foreach (Bildirim bld in BildirimList)
            {
                Bildirim b = bld;
                b.OkunduMu = true;
                Context.Baglanti.SaveChanges();
            }
           
        }
        public PartialViewResult Bildirims()
        {
            Guid id = KullaniciID();
            var bildirim = Context.Baglanti.Bildirims.Where(x => x.KullaniciID == id).OrderByDescending(x => x.BildirimTarihi).ToList();
            ViewBag.Count = Context.Baglanti.Bildirims.Count(x => x.KullaniciID == id && x.OkunduMu == false);
           
            return PartialView(bildirim);
        }
        public PartialViewResult Mesajs()
        {
            Guid id = KullaniciID();
            ViewBag.Count = Context.Baglanti.Mesajs.Count(x => x.MesajAlanID == id && x.OkunduMu == false);
            return PartialView();
        }

        private static Guid KullaniciID()
        {
            MembershipUser mu = Membership.GetUser();
            Guid id = (Guid)mu.ProviderUserKey;
            return id;
        }
    }
}
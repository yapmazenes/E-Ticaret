using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZeonTicaret.WebUI.Controllers
{
    using Models;
    using ZeonTicaret.WebUI.App_Classes;
    public class AdminKargoController : Controller
    {
        //
        // GET: /AdminKargo/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OnayBekleyenSiparisler(string id = "0")
        {

            List<Sati> data;
            if (id != "0")
            {
                Guid m = Guid.Parse(id);
                data = Context.Baglanti.Satis.Where(x => x.MusteriID == m && x.SiparisDurumID == (int)SiparisOnay.OnayBekliyor).ToList();
            }
            else
            {
                data = Context.Baglanti.Satis.Where(x => x.SiparisDurumID == (int)SiparisOnay.OnayBekliyor).ToList();
            }
            return View(data);
        }

        public ActionResult SiparisDetay(int id)
        {
            var data = Context.Baglanti.SatisDetays.Where(x => x.SatisID == id).ToList();
            Sati f = Context.Baglanti.Satis.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.SatisDurum = f.SiparisDurumID;
            ViewBag.ToplamFiyat = f.ToplamTutar;
            ViewBag.SatisTarih = f.SatisTarihi;
            ViewBag.KargoAdi = f.Kargo.SirketAdi;
            return View(data);
        }
        public ActionResult SatisOnayla(int id)
        {
            Sati s = Context.Baglanti.Satis.FirstOrDefault(x => x.Id == id);
            s.SiparisDurumID = (int)SiparisOnay.KargoMerkezeUlaştırılıyor;
            Bildirim b = new Bildirim();
            b.Adi = "Sipariş Onaylandı";
            b.BildirimTarihi = DateTime.Now;
            b.Detay = "Sipariş Kargoda";
            b.KullaniciID = (Guid)s.MusteriID;
            b.OkunduMu = false;
            String KID = s.Id.ToString();
            List<Bildirim> adminBild = Context.Baglanti.Bildirims.Where(x => x.Detay == KID).ToList();
            foreach (Bildirim item in adminBild)
            {
                Bildirim bild = item;
                bild.OkunduMu = true;
                Context.Baglanti.SaveChanges();
            }
            Context.Baglanti.Bildirims.Add(b);
            Context.Baglanti.SaveChanges();
            return RedirectToAction("OnayBekleyenSiparisler");
        }
        public ActionResult Satisİptal(int id)
        {
            Sati s = Context.Baglanti.Satis.FirstOrDefault(x => x.Id == id);
            s.SiparisDurumID = (int)SiparisOnay.Reddedildi;
            String KID = s.Id.ToString();
            Bildirim b = new Bildirim();
            b.Adi = "Sipariş İptal Edildi";
            b.BildirimTarihi = DateTime.Now;
            b.Detay = "Sipariş İptal Edildi";
            b.KullaniciID = (Guid)s.MusteriID;
            b.OkunduMu = false;
            Context.Baglanti.Bildirims.Add(b);
            List<Bildirim> adminBild = Context.Baglanti.Bildirims.Where(x => x.Detay == KID).ToList();
            foreach (Bildirim item in adminBild)
            {
                Bildirim bild = item;
                bild.OkunduMu = true;
                Context.Baglanti.SaveChanges();
            }
            Context.Baglanti.SaveChanges();
            return RedirectToAction("OnayBekleyenSiparisler");
        }
        public ActionResult KargoyaVerilenSiparisler()
        {
            var s = Context.Baglanti.Satis.Where(x => x.SiparisDurumID == (int)SiparisOnay.KargoMerkezeUlaştırılıyor).ToList();
            return View(s);
        }
        public ActionResult TamamlananSiparisler()
        {
            var s = Context.Baglanti.Satis.Where(x => x.SiparisDurumID == (int)SiparisOnay.KargoUlasti).ToList();
            return View(s);
        }
        public ActionResult ReddedilenSiparisler()
        {
            var s = Context.Baglanti.Satis.Where(x => x.SiparisDurumID == (int)SiparisOnay.Reddedildi).ToList();
            return View(s);
        }
    }
}
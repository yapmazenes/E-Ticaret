using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeonTicaret.WebUI.App_Classes;

namespace ZeonTicaret.WebUI.Controllers
{
    using Models;
    using System.Web.Security;
    [Authorize]
    public class SatisController : Controller
    {

        //
        // GET: /Satis/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SonKontrol()
        {
            List<string> CountryList = new List<string>();
            CultureInfo[] CInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo CInfo in CInfoList)
            {
                RegionInfo R = new RegionInfo(CInfo.LCID);
                if (!(CountryList.Contains(R.EnglishName)))
                {
                    CountryList.Add(R.EnglishName);
                }
            }

            CountryList.Sort();
            ViewBag.CountryList = CountryList;
            ViewBag.Kargo = Context.Baglanti.Kargoes.ToList();

            if (HttpContext.Session["AktifSepet"] != null)
            {
                Sepet s = (Sepet)HttpContext.Session["AktifSepet"];
                List<SepetItem> sepetItems = s.Urunler;
                ViewBag.ToplamTutar = Convert.ToDouble(s.ToplamTutar);
                ViewBag.ToplamKDV = Convert.ToDouble(s.ToplamTutar) * (0.18);
                ViewBag.Toplam = ViewBag.ToplamTutar + ViewBag.ToplamKDV; ;
                return View(sepetItems);
            }
            else
            {
                return View();
            }

        }
        [HttpPost]
        public ActionResult SonKontrol(Musteri m, Sati sts, MusteriAdre musAdr, string payment_method)
        {

            Sepet s = (Sepet)HttpContext.Session["AktifSepet"];
            List<SepetItem> sepetItems = s.Urunler;
            Double ToplamTutar = Convert.ToDouble(s.ToplamTutar);
            Double ToplamKDV = Convert.ToDouble(s.ToplamTutar) * (0.18);
            Double Toplam = ToplamTutar + ToplamKDV;
            MembershipUser mu = Membership.GetUser();
            Guid mId = (Guid)mu.ProviderUserKey;

            if (!Context.Baglanti.Musteris.Any(x => x.Id == mId))
            {
                m.Id = mId;
                m.KullaniciAdi = mu.UserName;
                m.Email = mu.Email;
                Context.Baglanti.Musteris.Add(m);

            }
            musAdr.MusteriID = mId;
            musAdr.Sehir = musAdr.Sehir.ToUpper();
            Context.Baglanti.MusteriAdres.Add(musAdr);
            sts.MusteriID = mId;
            sts.SatisTarihi = DateTime.Now;
            sts.Sepettemi = true;
            sts.ToplamTutar = Convert.ToInt32(Toplam);
            sts.KargoTakipNo = Guid.NewGuid().ToString().Substring(0, 10);
            sts.SiparisDurumID = 1;
            Context.Baglanti.Satis.Add(sts);
            Context.Baglanti.SaveChanges();
            List<string> adminsname = Roles.GetUsersInRole("Admin").ToList();
            List<MembershipUser> admins = new List<MembershipUser>();
            
            foreach (string item in adminsname)
            {
                admins.Add(Membership.GetUser(item));
            }
            foreach (MembershipUser admin in admins)
            {
                Bildirim b = new Bildirim();
                b.Adi = "Sipariş İsteği Var";
                b.BildirimTarihi = DateTime.Now;
                b.KullaniciID = (Guid)admin.ProviderUserKey;
                b.OkunduMu = false;
                b.Detay = sts.Id.ToString();
                Context.Baglanti.Bildirims.Add(b);
                Context.Baglanti.SaveChanges();
            }


          

            foreach (SepetItem sItem in sepetItems)
            {
                SatisDetay sd = new SatisDetay();
                sd.UrunID = sItem.Urun.Id;
                sd.SatisID = sts.Id;
                sd.Adet = sItem.Adet;
                sd.Fiyat = sItem.Tutar;
                sd.Indirim = 0;
                Context.Baglanti.SatisDetays.Add(sd);
                Context.Baglanti.SaveChanges();
            }
            HttpContext.Session["AktifSepet"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}
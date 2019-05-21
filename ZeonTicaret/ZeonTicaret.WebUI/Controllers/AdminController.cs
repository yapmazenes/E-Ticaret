using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeonTicaret.WebUI.App_Classes;
using ZeonTicaret.WebUI.Models;


namespace ZeonTicaret.WebUI.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
       
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Urunler(int Id=0)
        {
            ViewBag.Sonuc = Id;
            return View(Context.Baglanti.Uruns.ToList());

        }
        public void UrunSil(int id)
        {
            Urun u = Context.Baglanti.Uruns.FirstOrDefault(x => x.Id == id);
            Context.Baglanti.Uruns.Remove(u);
            Context.Baglanti.SaveChanges();
        }
        public ActionResult UrunEkle()
        {
            ViewBag.Kategoriler = Context.Baglanti.Kategoris.ToList();
            ViewBag.Markalar = Context.Baglanti.Markas.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult UrunEkle(Urun u)
        {
            if (ModelState.IsValid)
            {
                u.EklenmeTarihi = DateTime.Now;
                Context.Baglanti.Uruns.Add(u);
                Context.Baglanti.SaveChanges();
                return RedirectToAction("Urunler");
            }
            else
            {
                return HttpNotFound();
            }
        }
        public ActionResult UrunGuncelle(int id)
        {
            Urun u = Context.Baglanti.Uruns.FirstOrDefault(x=>x.Id==id);
            ViewBag.Kategoriler = Context.Baglanti.Kategoris.ToList();
            ViewBag.Markalar = Context.Baglanti.Markas.ToList();
            return View(u);
        }
        [HttpPost]
        public ActionResult UrunGuncelle(Urun urn)
        {
            Urun u = Context.Baglanti.Uruns.FirstOrDefault(x => x.Id == urn.Id);
            u.Adi = urn.Adi;
            u.Aciklama = urn.Aciklama;
            u.AlisFiyati = urn.AlisFiyati;
            u.SatisFiyati = urn.SatisFiyati;
            u.KategoriID = urn.KategoriID;
            u.MarkaID = urn.MarkaID;
            Context.Baglanti.SaveChanges();
            return RedirectToAction("Urunler","Admin",new {Id=1});
        }
        public ActionResult Markalar(int Id=0)
        {
            ViewBag.Sonuc = Id;
            return View(Context.Baglanti.Markas.ToList());
        }
        public ActionResult MarkaEkle()
        {
            return View();
        }
        public void MarkaResimSil(Marka u)
        {
            int resimID = Convert.ToInt32(u.ResimID);
            Resim rsm = Context.Baglanti.Resims.FirstOrDefault(x => x.Id == resimID);
            char krktr = '\\';
            string rsmyol = rsm.OrtaYol.Replace('/', krktr);
            string resimYol = @"C:\Users\pc\Documents\Visual Studio 2013\Projects\ADO.NET\191117MVCDERS\E-Ticaret\ZeonTicaret\ZeonTicaret.WebUI" + rsmyol;
            Context.Baglanti.Resims.Remove(rsm);
            if (System.IO.File.Exists(resimYol))
            {
                System.IO.File.Delete(resimYol);
            }
        }
        public int MarkaResimEkle(Marka m, HttpPostedFileBase fileUpload)
        {
            int resimId = -1;
            if (fileUpload != null)
            {
                Image img = Image.FromStream(fileUpload.InputStream);
                int width = int.Parse(ConfigurationManager.AppSettings["MarkaWidth"].ToString());
                int height = int.Parse(ConfigurationManager.AppSettings["MarkaHeight"].ToString());
                string name = "/Content/MarkaResim/" + Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);
                Bitmap bm = new Bitmap(img, width, height);
                bm.Save(Server.MapPath(name));
                Resim rsm = new Resim();
                rsm.OrtaYol = name;
                Context.Baglanti.Resims.Add(rsm);
                Context.Baglanti.SaveChanges();
                if (rsm.Id > 0)
                    resimId = rsm.Id;
            }
            if (resimId != -1)
                m.ResimID = resimId;
            return resimId;
        }
        public void MarkaSil(int id)
        {
            Marka u = Context.Baglanti.Markas.FirstOrDefault(x => x.Id == id);
            Context.Baglanti.Markas.Remove(u);
            MarkaResimSil(u);
            Context.Baglanti.SaveChanges();
        }
        [HttpPost]
        public ActionResult MarkaEkle(Marka m, HttpPostedFileBase fileUpload)
        {
            MarkaResimEkle(m, fileUpload);
            Context.Baglanti.Markas.Add(m);
            Context.Baglanti.SaveChanges();
            return RedirectToAction("Markalar");
        }
        [HttpPost]
        public ActionResult MarkaGuncelle(Marka marka, HttpPostedFileBase fileUpload)
        {
            if (fileUpload!=null)
            {
                MarkaResimSil(marka);
              marka.ResimID=  MarkaResimEkle(marka, fileUpload);
                
            }
          

            if (ModelState.IsValid)
            {
                Marka m = Context.Baglanti.Markas.FirstOrDefault(x => x.Id == marka.Id);
                m.Adi = marka.Adi;
                m.Aciklama = marka.Aciklama;
                m.ResimID = marka.ResimID;
                Context.Baglanti.SaveChanges();
                return RedirectToAction("Markalar", "Admin", new { Id = 1 });
            }

            return View(marka);
        }
        public ActionResult Kategori(int Id=0)
        {
            ViewBag.Sonuc = Id;
            return View(Context.Baglanti.Kategoris.ToList());
        }
        public ActionResult KategoriGuncelle(Kategori k)
        {
            Kategori kt = Context.Baglanti.Kategoris.FirstOrDefault(x=>x.Id==k.Id);
            kt.Adi = k.Adi;
            kt.Aciklama = k.Aciklama;
            return RedirectToAction("Kategori","Admin",new {Id=1});
        }



        public ActionResult KategoriEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkle(Kategori ktg)
        {
            Context.Baglanti.Kategoris.Add(ktg);
            Context.Baglanti.SaveChanges();
            return RedirectToAction("Kategori");
        }
        public void KategoriSil(int id)
        {
            Kategori u = Context.Baglanti.Kategoris.FirstOrDefault(x => x.Id == id);
            Context.Baglanti.Kategoris.Remove(u);
            Context.Baglanti.SaveChanges();
        }
        public ActionResult OzellikTipleri(int Id =0)
        {
           
           
            ViewBag.Sonuc = Id;
            ViewBag.Kategori = Context.Baglanti.Kategoris.ToList();
            return View(Context.Baglanti.OzellikTips.OrderBy(x => x.KategoriID).ToList());
        }
        public ActionResult OzellikTipEkle()
        {
            ViewBag.Kategori = Context.Baglanti.Kategoris.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult OzellikTipEkle(OzellikTip ozt)
        {
            if (ModelState.IsValid)
            {
                Context.Baglanti.OzellikTips.Add(ozt);
                Context.Baglanti.SaveChanges();
                return RedirectToAction("OzellikTipleri");
            }
            else
            {
                return HttpNotFound();
            }
        }
        public void OzellikTipSil(int id)
        {
            OzellikTip u = Context.Baglanti.OzellikTips.FirstOrDefault(x => x.Id == id);
            Context.Baglanti.OzellikTips.Remove(u);
            Context.Baglanti.SaveChanges();
        }

        [HttpPost]
        public ActionResult OzellikTipGuncelle(OzellikTip ozt)
        {

            OzellikTip ot = Context.Baglanti.OzellikTips.FirstOrDefault(x => x.Id == ozt.Id);
            ot.Adi = ozt.Adi;
            ot.Aciklama = ozt.Aciklama;
            ot.KategoriID = ozt.KategoriID;
            Context.Baglanti.SaveChanges();
            return RedirectToAction("OzellikTipleri", "Admin", new { Id =1});
        }
      
        public ActionResult OzellikDeger(int Id=0)
        {
            ViewBag.Sonuc = Id;
            return View(Context.Baglanti.OzellikDegers.OrderBy(x => x.OzellikTipID).ThenBy(x => x.Adi).ToList());
        }
        public ActionResult OzellikDegerGuncelle(int id)
        {
            OzellikDeger od = Context.Baglanti.OzellikDegers.FirstOrDefault(x => x.Id == id);
            ViewBag.OzellikTip = Context.Baglanti.OzellikTips.ToList();
            return View(od);
        }
        [HttpPost]
        public ActionResult OzellikDegerGuncelle(OzellikDeger ozd)
        {
            if (ModelState.IsValid)
            {
                OzellikDeger od = Context.Baglanti.OzellikDegers.FirstOrDefault(x => x.Id == ozd.Id);
                od.Aciklama = ozd.Aciklama;
                od.Adi = ozd.Adi;
                od.OzellikTipID = ozd.OzellikTipID;
                Context.Baglanti.SaveChanges();
                return RedirectToAction("OzellikDeger", new { Id = 1 });

            }
            else
            {
                return HttpNotFound();
            }
         
            }

        public ActionResult OzellikDegerEkle()
        {
            return View(Context.Baglanti.OzellikTips.ToList());
        }
        [HttpPost]
        public ActionResult OzellikDegerEkle(OzellikDeger od)
        {
            if (ModelState.IsValid)
            {
                Context.Baglanti.OzellikDegers.Add(od);
                Context.Baglanti.SaveChanges();
                return RedirectToAction("OzellikDeger");
            }
            else
            {
                return HttpNotFound();
            }

        }
        public void OzellikDegerSil(int id)
        {
            OzellikDeger u = Context.Baglanti.OzellikDegers.FirstOrDefault(x => x.Id == id);
            Context.Baglanti.OzellikDegers.Remove(u);
            Context.Baglanti.SaveChanges();
        }
        public ActionResult UrunOzellikleri()
        {
            return View(Context.Baglanti.UrunOzelliks.ToList());
        }

        public ActionResult UrunOzellikSil(int urunId, int tipId, int degerId)
        {
            UrunOzellik uo = Context.Baglanti.UrunOzelliks.FirstOrDefault(x => x.UrunID == urunId && x.OzellikTipID == tipId && x.OzellikDegerID == degerId);
            Context.Baglanti.UrunOzelliks.Remove(uo);
            Context.Baglanti.SaveChanges();
            return RedirectToAction("UrunOzellikleri");
        }




        public ActionResult UrunOzellikEkle()
        {
            ViewBag.Uruns = Context.Baglanti.Uruns;
            var model = new UrunOzellik();

            return View(model);
        }
        [HttpPost]
        public ActionResult UrunOzellikEkle(UrunOzellik model)
        {
            if (ModelState.IsValid)
            {
                Context.Baglanti.UrunOzelliks.Add(model);
                Context.Baglanti.SaveChanges();
                return RedirectToAction("UrunOzellikleri");
            }
            else
            {
                return HttpNotFound();
            }


        }
        [HttpPost]
        public JsonResult FillUrunTip(int u)
        {
            List<OzellikTip> JsonTipModels = new List<OzellikTip>();
            Urun urn = Context.Baglanti.Uruns.Where(x => x.Id == u).FirstOrDefault();

            List<OzellikTip> ozl = Context.Baglanti.OzellikTips.Where(x => x.KategoriID == urn.KategoriID).ToList();
            foreach (OzellikTip item in ozl)
            {
                var JsonModel = new OzellikTip
                {

                    Id = item.Id,
                    Adi = item.Adi,
                    Aciklama = item.Aciklama,
                };
                JsonTipModels.Add(JsonModel);
            }

            return Json(JsonTipModels, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult FillUrunDeger(int? tipID)
        {
            List<OzellikDeger> JsonDegerModels = new List<OzellikDeger>();
            List<OzellikDeger> ozldeger = Context.Baglanti.OzellikDegers.Where(x => x.OzellikTipID == tipID).OrderBy(x => x.Adi).ToList();
            foreach (OzellikDeger item in ozldeger)
            {
                var JsonDegerModel = new OzellikDeger
                {
                    Id = item.Id,
                    Adi = item.Adi,
                    Aciklama = item.Aciklama,
                };
                JsonDegerModels.Add(JsonDegerModel);
            }
            return Json(JsonDegerModels, JsonRequestBehavior.AllowGet);


        }

        public ActionResult UrunResimEkle(int id)
        {
            Urun u = Context.Baglanti.Uruns.Where(x => x.Id == id).FirstOrDefault();

            ViewBag.UrunAdi = u.Adi;
            ViewBag.Aciklama = u.Aciklama;
            return View(id);
        }

        [HttpPost]
        public ActionResult UrunResimEkle(int uId, HttpPostedFileBase fileUpload)
        {
            Urun u = Context.Baglanti.Uruns.Where(x => x.Id == uId).FirstOrDefault();
            if (fileUpload != null)
            {
                Image img = Image.FromStream(fileUpload.InputStream);
                Bitmap ortaResim = new Bitmap(img, Settings.UrunOrtaBoyut);
                Bitmap buyukResim = new Bitmap(img, Settings.UrunBuyukBoyut);
                string ortayol = "/Content/UrunResim/Orta/" + Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);
                string buyukyol = "/Content/UrunResim/Buyuk/" + Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);
                ortaResim.Save(Server.MapPath(ortayol));
                buyukResim.Save(Server.MapPath(buyukyol));
                Resim rsm = new Resim();
                rsm.BuyukYol = buyukyol;
                rsm.OrtaYol = ortayol;
                rsm.UrunID = uId;
                if (Context.Baglanti.Resims.FirstOrDefault(x => x.UrunID == uId && x.Varsayilan == false) != null)

                    rsm.Varsayilan = true;
                else
                {
                    rsm.Varsayilan = false;
                }
                Context.Baglanti.Resims.Add(rsm);
                Context.Baglanti.SaveChanges();
                ViewBag.Sonuc = 1;
                ViewBag.UrunAdi = u.Adi;
                ViewBag.Aciklama = u.Aciklama;
                return View(uId);
            }
            return HttpNotFound();

        }

        public ActionResult SliderResim()
        {
            return View();
        }
        public ActionResult Musteri()
        {
         
            return View(Context.Baglanti.Musteris.ToList());
        }       



    }
}
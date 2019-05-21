using _191117Mvc_Template.App_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZeonTicaret.WebUI;

namespace _191117Mvc_Template.Controllers
{
    using ZeonTicaret.WebUI.App_Classes;
    using ZeonTicaret.WebUI.Models;
    [Authorize]
    public class UyeController : Controller
    {
        //
        // GET: /Uye/
        [AllowAnonymous]
        public ActionResult GirisYap()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult GirisYap(Kullanici k, string hatirla)
        {
            bool KullaniciVarmi = Membership.ValidateUser(k.KullaniciAdi, k.Parola);
            if (KullaniciVarmi)
            {
                if (hatirla == "on")

                    FormsAuthentication.RedirectFromLoginPage(k.KullaniciAdi, true);
                else
                {
                    FormsAuthentication.RedirectFromLoginPage(k.KullaniciAdi, false);
                }
                ViewBag.hosgeldin = k.KullaniciAdi;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.mesaj = "Kullanıcı adı veya parola hatalı";
                return View();
            }

        }

        public ActionResult CikisYap()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("GirisYap", "Uye");
        }
        [AllowAnonymous]
        public ActionResult ParolamiUnuttum()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ParolamiUnuttum(Kullanici k, string reParola)
        {
            MembershipUser mu = Membership.GetUser(k.KullaniciAdi);
            if (k.Parola == reParola)
            {
                try
                {
                    if (mu.PasswordQuestion == k.Gizlisoru)
                    {
                        string pwd = mu.ResetPassword(k.Gizlicevap);
                        mu.ChangePassword(pwd, k.Parola);
                        return RedirectToAction("GirisYap");

                    }
                    else
                    {
                        ViewBag.Mesaj = "Girilen bilgiler yanlıştı";
                        return View();
                    }
                }
                catch (Exception)
                {
                    ViewBag.Mesaj = "Girilen bilgiler yanlıştı";
                    return View();

                }

            }
            ViewBag.Mesaj = "Parolalar birbiriyle uyuşmuyor";
            return View();

        }
        public PartialViewResult kullanici()
        {
            MembershipUser usr = Membership.GetUser();
            string id = usr.ProviderUserKey.ToString();
            if (Context.Baglanti.Resims.Any(x => x.userKey == id))
            {
                ViewBag.Img = Context.Baglanti.Resims.FirstOrDefault(x => x.userKey == id).KucukYol;
            }

            return PartialView();
        }
        public PartialViewResult MesajlarAdmin()
        {
            List<Mesaj> mesajlar = MesajlarMethod();
            ViewBag.MesajSayi = mesajlar.Where(x => x.MesajAtanID != KullaniciID()).Count(x => x.OkunduMu == false);
            return PartialView(mesajlar);
        }
        public ActionResult Mesajlar()
        {
            List<Mesaj> mesajlar = MesajlarMethod();
            return View(mesajlar);
        }

        private List<Mesaj> MesajlarMethod()
        {
            Guid usrID = KullaniciID();
            var oturum = Context.Baglanti.Mesajs.Where(x => x.MesajAlanID == usrID || x.MesajAtanID == usrID).OrderByDescending(x => x.MesajOturumId).Select(x => x.Oturum).Distinct().ToList();
            List<Mesaj> mesajlar = new List<Mesaj>();
            foreach (int item in oturum)
            {
                mesajlar.Add(Context.Baglanti.Mesajs.Where(x => x.Oturum == item && (x.MesajAlanID == usrID || x.MesajAtanID == usrID)).OrderByDescending(x => x.MesajOturumId).FirstOrDefault());
            }
            List<Kullanici> kullanicilar = new List<Kullanici>();
            String[] adminName = Roles.GetUsersInRole("Admin");
            foreach (string usrName in adminName)
            {
                Kullanici kullanici = new Kullanici();
                kullanici.KullaniciAdi = usrName;
                kullanici.Idkey = (Guid)Membership.GetUser(usrName).ProviderUserKey;
                kullanicilar.Add(kullanici);
            }
            ViewBag.Admins = kullanicilar;
            return mesajlar;
        }

        private static Guid KullaniciID()
        {
            MembershipUser usr = Membership.GetUser();
            Guid usrID = (Guid)usr.ProviderUserKey;
            return usrID;
        }
        public ActionResult MesajDetay(int id)
        {
            List<Mesaj> data = Context.Baglanti.Mesajs.Where(x => x.Oturum == id).OrderBy(x => x.SendDate).ToList();
            if (data.Any(x => x.OkunduMu == false && x.MesajAlanID == KullaniciID()))
            {
                foreach (Mesaj item in data)
                {
                    Mesaj m = item;
                    m.OkunduMu = true;
                    Context.Baglanti.SaveChanges();
                }
            }

            return View(data);
        }
        public string MesajCevap(int id, string msg, Guid toID, string admin = "false")
        {
            string sonuc;
            MembershipUser usr = Membership.GetUser();
            Guid usrID = (Guid)usr.ProviderUserKey;
            Mesaj mesaj = new Mesaj();
            mesaj.MesajAlanID = toID;
            mesaj.MesajAtanID = usrID;
            mesaj.Baslik = "Cevap";
            mesaj.Konu = "Cevap";
            mesaj.Mesaj1 = msg;
            mesaj.SendDate = DateTime.Now;
            mesaj.Oturum = id;
            mesaj.OkunduMu = false;
            Context.Baglanti.Mesajs.Add(mesaj);
            Context.Baglanti.SaveChanges();
            if (admin == "false")
            {
                sonuc = "<div class='c'><p class='isim'>" + usr.UserName + "</p><span>" + msg + "</span><span class='time-right'>" + mesaj.SendDate + "</span></div>";
            }


            else
            {
                sonuc = " <div class='msg-time-chat'><a href='#'class='message-img'></a><div class='message-body msg-in <span class='arrow'></span><div class='text'><p class='attribution'><a href='#'>" + usr.UserName + "</a>" + mesaj.SendDate + "</p><p>" + msg + "</p></div></div></div>";
            }

            return sonuc;
        }
        public ActionResult MesajAt(Message m)
        {
            MembershipUser usr = Membership.GetUser();
            Guid usrID = (Guid)usr.ProviderUserKey;
            Mesaj mesaj = new Mesaj();
            mesaj.MesajAlanID = m.ToID;
            mesaj.MesajAtanID = usrID;
            mesaj.Baslik = m.Baslik;
            mesaj.Konu = m.Konu;
            mesaj.Mesaj1 = m.Mesaj;
            mesaj.SendDate = DateTime.Now;
            mesaj.OkunduMu = false;
            Context.Baglanti.Mesajs.Add(mesaj);
            Context.Baglanti.SaveChanges();
            mesaj.Oturum = mesaj.MesajOturumId;
            Context.Baglanti.SaveChanges();
            return RedirectToAction("Mesajlar");
        }
        public ActionResult AdminMesajDty(int id = 0)
        {
            List<Mesaj> data = new List<Mesaj>();
            if (Context.Baglanti.Mesajs.Any(x => x.Oturum == id))
            {
                data = Context.Baglanti.Mesajs.Where(x => x.Oturum == id).OrderBy(x => x.SendDate).ToList();
                if (data.Any(x => x.OkunduMu == false && x.MesajAlanID == KullaniciID()))
                {
                    foreach (Mesaj item in data)
                    {
                        Mesaj m = item;
                        m.OkunduMu = true;
                        Context.Baglanti.SaveChanges();
                    }
                }
                MesajlarAdmin();
                return View(data);
            }
            else
                return View(data);
        }
        public PartialViewResult BildirimlerAdmin()
        {
            Guid kID = KullaniciID();
            List<Bildirim> bildirims = new List<Bildirim>();
            bildirims = Context.Baglanti.Bildirims.Where(x => x.KullaniciID == kID && x.OkunduMu == false).ToList();
            ViewBag.Count = bildirims.Count;
            return PartialView(bildirims);
        }
    }
}